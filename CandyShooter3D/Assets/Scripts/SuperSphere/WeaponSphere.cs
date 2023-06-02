using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSphere : SuperSphereSpecialization
{
    [SerializeField] private WeaponType _typeOfWeapon;
    private GameObject _weapon;
    public override void InitializeSphereMethod()
    {
        var data = WeaponManager.Instance.GetWeaponData(_typeOfWeapon);
        if (_typeOfWeapon != WeaponType.NoWeapon)
        {
            _weapon = PoolManager.GetObject(data.WeaponInstance);
            _weapon.transform.parent = transform;
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.rotation = Quaternion.Euler(Vector3.zero);
            _weapon.SetActive(true);
        }
    }
    public override void UseSphereMethod(Collider other)
    {
        if (!_used && other.gameObject.TryGetComponent<CharacterAction>(out var action))
        {
            if (action.AddWeapon(_typeOfWeapon))
            {
                _used = true;
                _disappearCoroutine?.Invoke();
            }
        }
    }
}
