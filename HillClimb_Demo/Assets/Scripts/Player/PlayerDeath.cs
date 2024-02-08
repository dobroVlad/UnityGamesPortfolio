using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundLayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_groundLayer & (1 << collision.gameObject.layer)) != 0)
        {
            EventAggregator.Post<GameOverEvent>(this, new GameOverEvent(false));
        }
    }
}
