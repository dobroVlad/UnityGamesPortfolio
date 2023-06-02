using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private List<PoolInitializeData> _poolInitializeDatas = new List<PoolInitializeData>();
    [SerializeField] private Transform _parentForPool;
    [SerializeField] private TextMeshProUGUI _tabToPlayText;

    void Awake()
    {
        InitializePool();
        EventAggregator.Subscribe<GameStartEvent>(StartGame);
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameStartEvent>(StartGame);
    }
    private void StartGame(object sender, GameStartEvent start)
    {
        _tabToPlayText.gameObject.SetActive(false);
    }
    private void InitializePool()
    {
        PoolManager.SetParentForObject(_parentForPool);

        foreach (var data in _poolInitializeDatas)
        {
            PoolManager.InitializePool(data.Prefab, data.Count);
        }
    }
}
[System.Serializable]
public class PoolInitializeData
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _count;
    public GameObject Prefab => _prefab;
    public int Count => _count;
}