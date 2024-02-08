using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSphere : SuperSphereSpecialization
{
    [SerializeField] private int _number;
    public override void InitializeSphereMethod()
    {
    }
    public override void UseSphereMethod(Collider other)
    {
        if (!_used && other.gameObject.TryGetComponent<CharacterHealth>(out var health))
        {
            if (health.Heal(_number))
            {
                _used = true;
                _disappearCoroutine?.Invoke();
            }
        }
    }
}
