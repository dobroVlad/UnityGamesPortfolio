using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] List<WeaponData> _weaponData;
    [SerializeField] List<GrenadeData> _grenadeData;
    public static WeaponManager Instance;
    public static WeaponData NoWeapon;
    public static GrenadeData NoGrenade;
    public static readonly float DistanceToOffsetTarget = 50f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        NoWeapon = GetWeaponData(WeaponType.NoWeapon);
        NoGrenade = GetGrenadeData(GrenadeType.NoGrenade);
    }
    public WeaponData GetWeaponData(WeaponType type)
    {
        return _weaponData.FirstOrDefault(p => p.Type == type);
    }
    public GrenadeData GetGrenadeData(GrenadeType type)
    {
        return _grenadeData.FirstOrDefault(p => p.Type == type);
    }
}
public enum WeaponType
{
    NoWeapon=0,
    CandyRifle=1,
    MarshmallowGun=2
}
[System.Serializable]
public class WeaponData: DamageSource
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private int _projectileDamage;
    [SerializeField] private int _projectileShot;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _aimZoomAngle;
    [SerializeField] private float _offset;
    [SerializeField] private float _recoilAngle;
    [SerializeField] private int _magazineCapacity;
    [SerializeField] private int _reloadBullets;
    [SerializeField] private GameObject _prefabWeapon;
    [SerializeField] private List<GameObject> _prefabBullets;
    [SerializeField] private Sprite _uiSlotPicture;
    public DamageSourceType DamageType => DamageSourceType.FireWeapon;
    public WeaponType Type => _type;
    public int ProjectileDamage=> _projectileDamage;
    public int ProjectileShot => _projectileShot;
    public float BulletSpeed => _bulletSpeed;
    public float FireRate=>_fireRate;
    public float AimZoomAngle => _aimZoomAngle;
    public float Offset=> _offset;
    public float RecoilAngle => _recoilAngle;
    public int MagazineCapacity=>_magazineCapacity;
    public int ReloadBullets => _reloadBullets;
    public GameObject WeaponInstance => _prefabWeapon;
    public GameObject BulletInstance => _prefabBullets[Random.Range(0, _prefabBullets.Count)];
    public Sprite UiSlotPicture => _uiSlotPicture;
    public override AttackInfo GiveAttackInfo(int playerId)
    {
        return new AttackInfo(playerId, DamageType, (int)_type, _projectileDamage);
    }
}
public enum GrenadeType
{
    NoGrenade,
    Cupcake
}
[System.Serializable]
public class GrenadeData: DamageSource
{
    [SerializeField] private GrenadeType _type;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _explosionDamage;
    [SerializeField] private float _damageRange;
    [SerializeField] private float _explosionDelay;
    [SerializeField] private Sprite _uiSlotPicture;
    public DamageSourceType DamageType => DamageSourceType.Grenade;
    public GrenadeType Type => _type;
    public GameObject Instance => _prefab;
    public float Damage => _explosionDamage;
    public float DamageRange => _damageRange;
    public float ExplosionDelay => _explosionDelay;
    public Sprite UiSlotPicture => _uiSlotPicture;
    public override AttackInfo GiveAttackInfo(int playerId)
    {
        return new AttackInfo(playerId, DamageType, (int)_type, _explosionDamage);
    }
}
public enum DamageSourceType
{
    FireWeapon,
    Grenade
}
public struct AttackInfo
{
    private int _attackerId;
    public int AttackerId => _attackerId;
    private DamageSourceType _type;
    public DamageSourceType Type => _type;
    private int _weaponModel;
    public int WeaponModel => _weaponModel;
    private float _damage;
    public float Damage => _damage;
    public void MultiplyDamage(float koef)
    {
        _damage *= koef;
    }
    public AttackInfo (int id, DamageSourceType type, int model, float damage)
    {
        _attackerId = id;
        _type = type;
        _weaponModel = model;
        _damage = damage;
    }
}
public abstract class DamageSource
{
    public abstract AttackInfo GiveAttackInfo(int playerId);  
}
