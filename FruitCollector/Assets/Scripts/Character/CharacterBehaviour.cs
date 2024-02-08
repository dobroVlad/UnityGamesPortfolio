using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 20f)] private float _throwingForce;
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private MultiAimConstraint _myMultiAimConstraint;
    [SerializeField] private ChainIKConstraint _myChainIKConstraint;
    [SerializeField] private RigBuilder _myRigBuilder;
    [SerializeField] private Transform _ikHelper;
    [SerializeField] private CharacterCatcher _myFoodCatcher;
    private bool _ableToTake=true;
    private FoodType _targetFoodType;
    void Awake()
    {
        EventAggregator.Subscribe<TakeFoodEvent>(GetFood);
        EventAggregator.Subscribe<NewTaskEvent>(SetTargetType);
        EventAggregator.Subscribe<GameFinishEvent>(LevelCompleted);
        _myFoodCatcher.SetTakerAction((FoodUnit food) => TakeFood(food));
    }

    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<TakeFoodEvent>(GetFood);
        EventAggregator.Unsubscribe<NewTaskEvent>(SetTargetType);
        EventAggregator.Unsubscribe<GameFinishEvent>(LevelCompleted);
    }
    private void GetFood(object sender, TakeFoodEvent foodInfo)
    {
        if (_ableToTake)
        {
            SetIkTarget(foodInfo.Food.transform);
            _myAnimator.SetTrigger(CharacterAnimationParametrs.trigger_takeFood.ToString());
            _myFoodCatcher.SetTarget(foodInfo.Food);
            _ableToTake = false;
        }
    }
    private void SetTargetType(object sender, NewTaskEvent task)
    {
        _targetFoodType = task.Type;
    }
    private void LevelCompleted(object s, GameFinishEvent finish)
    {
        _myAnimator.SetTrigger(CharacterAnimationParametrs.trigger_dance.ToString());
    }
    private void TakeFood(FoodUnit food)
    {
        if (food.TryGetComponent<Collider>(out var foodCollider))
        {
            EventAggregator.Post(this, new CaughtFoodEvent(food));
            _ikHelper.position = food.transform.position;
            SetIkTarget(_ikHelper);
            if (food.Type == _targetFoodType) 
            {
                food.SetTaken();
                foodCollider.isTrigger = true;
                food.Body.isKinematic = true;
                food.Body.constraints = RigidbodyConstraints.None;
                food.transform.parent = _myFoodCatcher.transform;
                food.transform.localPosition = Vector3.zero;
                _myAnimator.SetTrigger(CharacterAnimationParametrs.trigger_getFood.ToString());
            }
            else
            {
                var direction = (food.transform.position - transform.position).normalized;
                food.Body.AddForce(new Vector3(direction.x,0f,direction.z)* _throwingForce, ForceMode.Impulse);
                StartCoroutine(DelayedBackInPool(food.gameObject));
                AudioManager.Instance.PlaySfx(SfxType.Throw);
            }
        }
    }
    public void SetAbleToTake()
    {
        _ableToTake = true;
    }
    private void SetIkTarget(Transform target)
    {
        _myChainIKConstraint.data.target = target;
        var newAimTarget = new WeightedTransformArray();
        newAimTarget.Add(new WeightedTransform(target, 1f));
        _myMultiAimConstraint.data.sourceObjects = newAimTarget;
        _myRigBuilder.Build();
    }
    private IEnumerator DelayedBackInPool(GameObject gameOb)
    {
        var timer = 0f;
        var duration = 1f;
        while(timer< duration)
        {
            timer += Time.deltaTime;
            if (!gameOb.activeInHierarchy)
            {
                break;
            }
            if (timer >= duration)
            {
                ObjectPoolManager.PutObject(gameOb); 
            }
            yield return null;
        }

    }
}
public enum CharacterAnimationParametrs
{
    trigger_takeFood,
    trigger_dance,
    trigger_getFood
}
