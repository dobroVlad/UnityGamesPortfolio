using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField] private List<PoolInitializeData> _poolInitializeDatas = new List<PoolInitializeData>();
    [SerializeField] private List<TeamData> _teams;
    [SerializeField] private List<PlayerData> _players;
    [SerializeField] private Canvas _chooseHeroCanvas;
    [SerializeField] private Button _hippoButton;
    [SerializeField] private Button _chameloButton;
    [SerializeField] private PlayerData _clientData;
    [SerializeField] private GameObject _hippoPrefab;
    [SerializeField] private GameObject _chameloPrefab;
    [SerializeField] private Transform _parentForPool;
    [SerializeField] private GameObject _playerController;
    [SerializeField] private GameObject _chooseHeroGroup;
    private PlayerController _clientPlayer;
    void Awake()
    {
        InitializePool();
        _clientPlayer = _playerController.GetComponent<PlayerController>();
    }
    private void Start()
    {
        _chooseHeroCanvas.gameObject.SetActive(true);
        _hippoButton.onClick.AddListener(() => StartGame(_hippoPrefab));
        _chameloButton.onClick.AddListener(() => StartGame(_chameloPrefab));
    }
    private void StartGame(GameObject pleyerHero)
    {
        _chooseHeroCanvas.gameObject.SetActive(false);
        _chooseHeroGroup.SetActive(false);
        _clientData.ChooseHero(pleyerHero);
        int clientPlayerId = Random.Range(0, _players.Count/2)+ (_clientData.Character == _hippoPrefab?0:4);
        _players[clientPlayerId] = _clientData;
        int playersInTeam = _players.Count % _teams.Count == 0 ? _players.Count / _teams.Count : _players.Count / _teams.Count + 1;
        var curentTeam = 0;
        for (int i = 0; i < _players.Count; i++)
        {
            curentTeam = i < ((curentTeam + 1) * playersInTeam) ? curentTeam : curentTeam + 1;
            _players[i].SetSessionData(new PlayerSessionData(i, curentTeam));
            if (i == clientPlayerId)
            {
                _clientPlayer.Activate(SetCharacter(_players[i]));
            }
            else
            {
                SetCharacter(_players[i]);
            }
        }
    }
    private CharacterController SetCharacter(PlayerData player)
    {
        var character = PoolManager.GetObject(player.Character);
        if(character.TryGetComponent<CharacterController>(out var controller))
        {
            character.transform.position = _teams[player.SessionData.Team].GetSpawnPlace();
            character.gameObject.SetActive(true);
            controller.Action.SetStartValues();
            return controller;
        }
        else
        {
            PoolManager.PutObject(character);
            return null;
        }
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
[System.Serializable]
public class TeamData
{
    [SerializeField] private Transform _spawnPlace;
    [SerializeField] private int _id;
    private float _spawnRange=>3f;
    public int ID => _id;
    public Vector3 GetSpawnPlace()
    {
        var randomPosition = new Vector3(Random.Range(0, _spawnRange), 0f, Random.Range(0, _spawnRange));
        var hits = Physics.OverlapSphere(_spawnPlace.position + randomPosition, 1f, (int)LayersNum.Character);
        if (hits.Length == 0) { return _spawnPlace.position + randomPosition; }
        else { return GetSpawnPlace(); }
    }
}
[System.Serializable]
public class PlayerData
{
    [SerializeField] private GameObject _playerCharacterPrefab;
    [SerializeField] private string _name;
    private PlayerSessionData _sessionData;
    public string Name => _name;
    public GameObject Character => _playerCharacterPrefab;
    public PlayerSessionData SessionData => _sessionData;
    public void SetSessionData(PlayerSessionData data)
    {
        _sessionData = data;
    }
    public void ChooseHero(GameObject hero)
    {
        _playerCharacterPrefab = hero;
    }
}
public class PlayerSessionData
{
    private int _id;
    private int _teamId;
    private int _kills;
    private int _deaths;
    public int ID => _id;
    public int Team => _teamId;
    public int Kills => _kills;
    public int Deaths => _deaths;
    public PlayerSessionData(int id, int teamId)
    {
        _id = id;
        _teamId = teamId;
        _kills = 0;
        _deaths = 0;
    }
}
