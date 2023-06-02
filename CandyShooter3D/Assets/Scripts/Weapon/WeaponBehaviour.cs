using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [SerializeField] WeaponType _type;
    [SerializeField] private Animator _weaponAnimator;
    [SerializeField] private Transform _firePoint;
    WeaponData _data;
    private bool _waitForShot;
    private int _curentBullets;
    public WeaponData Data => _data;
    public Animator WeaponAnimator => _weaponAnimator;
    public bool AbleToFire => CurentBullets > 0 && !_waitForShot;
    public int CurentBullets => _curentBullets;
    public int ReloadBullets => _data.MagazineCapacity - _curentBullets > _data.ReloadBullets ? _data.ReloadBullets : _data.MagazineCapacity - _curentBullets;

    void Start()
    {
        _data = WeaponManager.Instance.GetWeaponData(_type);
        _curentBullets = _data.MagazineCapacity;
        _waitForShot = false;
    }
    public void FillMagazine(int num)
    {
        num = num > ReloadBullets ? ReloadBullets : num;
        _curentBullets += num;
    }
    public bool Fire(Transform viewObject, Vector3 targetPosition)
    {
        if (AbleToFire)
        {
            _curentBullets -= 1;
            for (int i = 0; i < Data.ProjectileShot; i++)
            {
                var bulletObject = PoolManager.GetObject(Data.BulletInstance);
                var bullet = bulletObject.GetComponent<BulletBehaviour>();
                bullet.SetStartData(_firePoint.position, Data.BulletSpeed, Data.GiveAttackInfo(0));
                bullet.transform.LookAt(SetDirection(viewObject, targetPosition));
                bullet.gameObject.SetActive(true);
                bullet.Activate();
            }
          
            StartCoroutine(AutoFire());
            return true;
        }
        return false;
    }
    private Vector3 SetDirection(Transform viewObject, Vector3 targetPosition)
    {
        var random = Random.Range(0f, 200 * Mathf.PI) / 100f;
        var randomVector = new Vector2(Mathf.Cos(random), Mathf.Sin(random)) * Random.Range(0f, 100) / 100f;
        var offset = Vector3.Distance(viewObject.position, targetPosition) / WeaponManager.DistanceToOffsetTarget * Data.Offset;
        return targetPosition + viewObject.right * randomVector.x * offset + viewObject.up * randomVector.y * offset;
    }
    private IEnumerator AutoFire()
    {
        _waitForShot = true;
        var timer = Data.FireRate;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _waitForShot = false;
    }
}
