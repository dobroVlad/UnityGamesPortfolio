using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelCompleted : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((_playerLayer & (1 << collision.gameObject.layer)) != 0)
        {
            EventAggregator.Post<GameOverEvent>(this, new GameOverEvent(true));
        }
    }
}
