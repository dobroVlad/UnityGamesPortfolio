using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassedCheckPoint : MonoBehaviour
{
    [SerializeField] private int _heroLayerNum;
    [SerializeField] private Animator _myFireAnimator;
    private bool _used;
    private void Awake()
    {
        _used = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_used && collision.gameObject.layer ==_heroLayerNum)
        {
            collision.gameObject.GetComponent<SpawnHero>().UpdateCheckPoint(transform.position);
            _used = true;
            _myFireAnimator.SetTrigger(AnimationParameters.SpawnPortal.triggerUsed.ToString());
        }
    }
}

