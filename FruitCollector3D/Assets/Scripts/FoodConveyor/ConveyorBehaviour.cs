using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FoodType
{
    Apple,
    Banana,
    Orange
}
public class ConveyorBehaviour : MonoBehaviour
{
    [SerializeField] private List<PoolInitializeData> _foodPoolInitializeDatas = new List<PoolInitializeData>();
    [SerializeField] private Transform _foodSpawnPoint;
    [SerializeField] private ConveyorCollector _collector;
    [SerializeField] private Material _conveyorLineMaterial;
    [SerializeField] private Renderer _conveyorLineRenderer;
    [SerializeField] private float _timeToStart;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _conveyorSpeedDefault;
    [SerializeField] private float _conveyorSpeedDelta;
    [SerializeField] private float _conveyorSpeedLimit;
    private float _conveyorSpeedCurrent;
    private List<FoodUnit> _activeFood 
        = new List<FoodUnit>();
    private Material _myLineMaterial;
    bool _started =false;

    void Awake()
    {
        InitializeObjectPool();
        EventAggregator.Subscribe<GameStartEvent>(StartGame);
        EventAggregator.Subscribe<TakeFoodEvent>(SpeedUp);
        EventAggregator.Subscribe<CaughtFoodEvent>(Caughtfood);
        EventAggregator.Subscribe<GameFinishEvent>(LevelCompleted);
        _collector.SetCollectorAction((FoodUnit food)=>CollectFood(food));
        _conveyorSpeedCurrent = _conveyorSpeedDefault;
    }

    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<TakeFoodEvent>(SpeedUp);
        EventAggregator.Unsubscribe<GameStartEvent>(StartGame);
        EventAggregator.Unsubscribe<CaughtFoodEvent>(Caughtfood);
        EventAggregator.Unsubscribe<GameFinishEvent>(LevelCompleted);
    }

    void Start()
    {
        _myLineMaterial = _conveyorLineRenderer.materials.FirstOrDefault(m=>m.mainTexture == _conveyorLineMaterial.mainTexture);
    }

    void FixedUpdate()
    {
        foreach (var food in _activeFood)
        {
            var direction = (_collector.transform.position - food.transform.position).normalized;
            food.Body.velocity = new Vector3(direction.x, food.Body.velocity.y, direction.z) * _conveyorSpeedCurrent;
        }
    }

    void Update()
    {
        if (_started)
        {
            _myLineMaterial.mainTextureOffset += Vector2.up * _conveyorSpeedCurrent * Time.deltaTime;
        }
    }
    private void SpeedUp(object sender, TakeFoodEvent foodInfo)
    {
        if (_conveyorSpeedCurrent < _conveyorSpeedLimit)
        {
            _conveyorSpeedCurrent += _conveyorSpeedDelta;
            _conveyorSpeedCurrent = _conveyorSpeedCurrent > _conveyorSpeedLimit ? _conveyorSpeedLimit : _conveyorSpeedCurrent;
        }
    }
    private void Caughtfood(object sender, CaughtFoodEvent foodInfo)
    {
        _activeFood.Remove(foodInfo.Food);
    }
    private void LevelCompleted(object s, GameFinishEvent finish)
    {
        gameObject.SetActive(false);
    }

    private void StartGame(object sender, GameStartEvent start)
    {
        _started = true;
        StartCoroutine(PrepareConveyor());
    }

    private void InitializeObjectPool()
    {
        ObjectPoolManager.SetParentForObject(_foodSpawnPoint);

        foreach (var data in _foodPoolInitializeDatas)
        {
            ObjectPoolManager.InitializePool(data.Prefab, data.Count);
        }
    }

    void SpawnFood()
    {
        var prefab = _foodPoolInitializeDatas[Random.Range(0, _foodPoolInitializeDatas.Count)].Prefab;
        var food = ObjectPoolManager.GetObject(prefab);
        if(food.TryGetComponent<FoodUnit>(out var foobBehaviour))
        {
            food.SetActive(true);
            _activeFood.Add(foobBehaviour);
            foobBehaviour.transform.position = _foodSpawnPoint.position;
            foobBehaviour.Body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            ObjectPoolManager.PutObject(food);
        }
    }
    void CollectFood(FoodUnit food)
    {
        _activeFood.Remove(food);
        ObjectPoolManager.PutObject(food.gameObject);
    }

    private IEnumerator PrepareConveyor()
    {
        var timer = 0f;
        while (timer < _timeToStart)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(PrepareSpawn());
    }
    private IEnumerator PrepareSpawn()
    {
        var timer = 0f;
        while (timer < _spawnDelay*(_conveyorSpeedDefault/_conveyorSpeedCurrent))
        {
            timer += Time.deltaTime;
            yield return null;
        }
        SpawnFood();
        StartCoroutine(PrepareSpawn());
    }
}
