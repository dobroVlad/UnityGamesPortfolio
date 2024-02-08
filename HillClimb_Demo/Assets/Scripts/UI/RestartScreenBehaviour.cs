using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestartScreenBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _levelScreen;
    [SerializeField]
    private GameObject _gameOverScreen;
    [SerializeField]
    private TextMeshProUGUI _restartScreenMessage;
    [SerializeField]
    private string _victoryMessage;
    [SerializeField]
    private string _lossMessage;
    private void Awake()
    {
        EventAggregator.Subscribe<GameOverEvent>(GameOver);
        _levelScreen.SetActive(true);
        _gameOverScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameOverEvent>(GameOver);
    }
    private void GameOver(object sender, GameOverEvent gameOver)
    {
        _levelScreen.SetActive(false);
        _gameOverScreen.SetActive(true);
        _restartScreenMessage.text = gameOver.Victory ? _victoryMessage : _lossMessage;
        Time.timeScale = 0.0f;
    }
}

