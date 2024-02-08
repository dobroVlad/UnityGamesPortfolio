using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _placeForFood;
    [SerializeField] private float _foodSizeScale;
    [SerializeField] private ParticleSystem _putInParticles;
    private Collider _myCollider;
    private void Awake()
    {
        EventAggregator.Subscribe<GameFinishEvent>(LevelCompleted);
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameFinishEvent>(LevelCompleted);
    }
    private void Start()
    {
        _myCollider = GetComponent<Collider>();
    }
    private void LevelCompleted(object s, GameFinishEvent finish)
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<FoodUnit>(out var food)&&food.Taken)
        {
            food.transform.parent = transform;
            food.transform.position = _placeForFood.position;
            other.isTrigger = false;
            food.Body.isKinematic = false;
            food.transform.localScale *= _foodSizeScale;
            _putInParticles.Play();
            AudioManager.Instance.PlaySfx(SfxType.Take);
            EventAggregator.Post(this, new PackedFoodEvent());
        }
    }
}
