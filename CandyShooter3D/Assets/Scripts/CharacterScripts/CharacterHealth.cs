using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BodyPartType
{
    Hand,
    Leg,
    Body,
    Head
}
public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] private HealthBar _healthBarUI;
     private bool _isAlive;
    private int _maxHealth;
    public int MaxHealth => _maxHealth>0?_maxHealth: _health;
    public int CurentHealth => _health;
    public bool Alive => _isAlive;
    void Start()
    {
        _maxHealth = _health;
        _isAlive = true;
        var bodyParts = transform.GetComponentsInChildren<CharacterBodyPart>();
        foreach (var part in bodyParts)
        {
            part.SetDamagedCallback((attack, part) => TakeDamage(attack, part));
        }
        _healthBarUI.SetMaxValue(_health);
        _healthBarUI.UpdateHealthPoints(_health);
    }
    private void TakeDamage(AttackInfo attack, BodyPartType part)
    {
        float koef;
        switch (part)
        {
            case BodyPartType.Hand:
            case BodyPartType.Leg:
                koef = 0.8f;
                break;
            case BodyPartType.Head:
                koef = 3f;
                break;
            default:
                koef = 1f;
                break;
        }
        _health -= (int)(attack.Damage* koef);
        _healthBarUI.UpdateHealthPoints(_health);
        _isAlive = _health > 0;
        if (!_isAlive)
        {
            Die();
        }
    }
    public bool Heal(int num)
    {
        if (_isAlive&&_health < _maxHealth && num>0)
        {
            _health = Mathf.Clamp(_health + num, _health, _maxHealth);
            _healthBarUI.UpdateHealthPoints(_health);
            return true;
        }
        return false;
    }
    public void Die()
    {
        if (!_isAlive)
        {
        }
    }
}
