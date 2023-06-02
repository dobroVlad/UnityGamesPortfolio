using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int _healthPoints;
    [SerializeField] private int _heroLayerNum;
    [SerializeField] private Animator _myAnimator;
    private bool _used;
    void Start()
    {
        _used = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _heroLayerNum&&!_used)
        {
            _used = true;
            collision.gameObject.GetComponent<CharacterHealth>().HealthPoints += _healthPoints;
            _myAnimator.SetTrigger(AnimationParameters.ItemsActions.triggerUsedHealing.ToString());
        }
    }
}
