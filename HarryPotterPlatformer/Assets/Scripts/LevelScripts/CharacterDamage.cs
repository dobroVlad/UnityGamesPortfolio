using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamage : MonoBehaviour
{
    [SerializeField] private int _topSideDamage;
    [SerializeField] private int _downSideDamage;
    [SerializeField] private int _leftSideDamage;
    [SerializeField] private int _rightSideDamage;
    public int Top => _topSideDamage;
    public int Down => _downSideDamage;
    public int Right => _rightSideDamage;
    public int Left => _leftSideDamage;

    public void SetDamage(int damage)
    {
        _topSideDamage = damage;
        _rightSideDamage = damage;
        _leftSideDamage = damage;
        _downSideDamage = damage;
    }
}
