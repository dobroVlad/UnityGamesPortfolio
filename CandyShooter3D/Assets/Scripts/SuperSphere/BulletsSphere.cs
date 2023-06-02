using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsSphere : SuperSphereSpecialization
{
    [SerializeField] private WeaponType _typeOfWeapon;
    [SerializeField] private int _number;
    public override void InitializeSphereMethod()
    {
    }
    public override void UseSphereMethod(Collider other)
    {
        if (!_used && other.gameObject.TryGetComponent<CharacterAction>(out var action))
        {
            if (action.FillBulletsStorage(_typeOfWeapon, _number))
            {
                _used = true;
                _disappearCoroutine?.Invoke();
            }
        }
    }
}
