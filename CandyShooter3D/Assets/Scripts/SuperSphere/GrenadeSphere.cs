using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSphere : SuperSphereSpecialization
{
    [SerializeField] private GrenadeType _typeOfGrenade;
    [SerializeField] private int _number;
    private GameObject _grenade;
    public override void InitializeSphereMethod()
    {
    }
    public override void UseSphereMethod(Collider other)
    {
        if (!_used && other.gameObject.TryGetComponent<CharacterAction>(out var action))
        {
            if (action.AddGrenade(_typeOfGrenade, _number))
            {
                _used = true;
                _disappearCoroutine?.Invoke();
            }
        }
    }
}
