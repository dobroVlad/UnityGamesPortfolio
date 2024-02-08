using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterBodyPart : MonoBehaviour
{
    [SerializeField] BodyPartType _type;
    private System.Action<AttackInfo, BodyPartType> _tookDamage;
    public void SetDamagedCallback(System.Action<AttackInfo, BodyPartType> action)
    {
        _tookDamage = action;
    }
    public void TakeDamage(AttackInfo attack)
    { 
        _tookDamage?.Invoke(attack, _type);
    }
}
