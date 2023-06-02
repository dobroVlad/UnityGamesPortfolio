using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAction : MonoBehaviour
{
    [SerializeField] private int _maxWeaponsNum = 2;
    [SerializeField] private CharacterMovement _myMovement;
    [SerializeField] private Transform _targetObject;
    [SerializeField] private Transform _myViewObject;
    [SerializeField] private MultiAimConstraint _weaponAimConstraint;
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private WeaponType _defaultWeapon;
    [SerializeField] private LayerMask _aimTargetLayers;
    [SerializeField] private Transform _granadeHolder;
    private bool _active=false;
    private SelectWeaponSystem _selectWeaponSystem;
    private AimWeaponSystem _aimSystem;
    private BulletsReloadSystem _bulletsReloadSystem;
    private GrenadeSystem _grenadeSystem;
    public Transform MyView => _myViewObject;
    public Transform MyTarget => _targetObject;
    public Transform WeaponHolder => _weaponHolder;
    public Transform GrenadeHolder => _granadeHolder;
    public MovementState MoveState => _myMovement.State;
    public bool Sprint => _myMovement.Sprint;
    public bool Aim => _aimSystem?.Aim??false;
    public bool TrySwitchAimPosition => _aimSystem.TrySwitchAimPosition();
    public WeaponBehaviour CurentWeapon => _selectWeaponSystem.CurentWeapon;
    public WeaponData CurentWeaponData => _selectWeaponSystem.CurentWeaponData;
    public Dictionary<WeaponType, WeaponBehaviour> Weapons => _selectWeaponSystem.Weapons;
    public ChangeWeaponSate ChangeWeaponState => _selectWeaponSystem?.ChangeWeaponState??ChangeWeaponSate.Passive;
    public Dictionary<WeaponType, int> BulletStorage => _bulletsReloadSystem.BulletStorage;
    public ReloadWeaponSate ReloadState => _bulletsReloadSystem.ReloadState;
    public GrenadeType CurentGrenadeType => _grenadeSystem.CurentGrenadeType;
    public Dictionary<GrenadeType, int> GrenadeStorage => _grenadeSystem.GrenadeStorage;
    public ThrowGrenadeSate ThrowGrenadeState => _grenadeSystem.ThrowGrenadeState;
    public bool ActiveFire { get; set; }
    public void SetStartValues()
    {
        _active = true;
        _selectWeaponSystem = _selectWeaponSystem?? new SelectWeaponSystem(this, _maxWeaponsNum);
        _aimSystem = _aimSystem?? new AimWeaponSystem(this, _weaponAimConstraint,_aimTargetLayers);
        _bulletsReloadSystem = _bulletsReloadSystem?? new BulletsReloadSystem(this);
        _grenadeSystem = _grenadeSystem??new GrenadeSystem(this);
        ActiveFire = false;
        FillBulletsStorage(WeaponType.CandyRifle, 120);
        FillBulletsStorage(WeaponType.MarshmallowGun, 40);
        StartCoroutine(TakeDefaultWeapon());
        AddGrenade(GrenadeType.Cupcake, 3);
    }
    private void FixedUpdate()
    {
        if (_active)
        {
            var aimPosition = _aimSystem.AimWeapon();
            _targetObject.position = aimPosition == Vector3.zero ? _targetObject.position : aimPosition;
        }
    }
    void Update()
    {
        if (_active)
        {
            if (ActiveFire)
            {
                if (ChangeWeaponState != ChangeWeaponSate.Execution && ReloadState == ReloadWeaponSate.Passive && ThrowGrenadeState != ThrowGrenadeSate.Execution)
                {
                    if (CurentWeapon.Fire(MyView, MyTarget.position))
                    {
                        _myMovement.RecoilFire(new Vector2(0f, CurentWeaponData.RecoilAngle));
                    }
                    else if (CurentWeapon.CurentBullets == 0)
                    {
                        ActiveFire = false;
                        TryReloadWeapon();
                    }
                }
                else
                {
                    if (ChangeWeaponState == ChangeWeaponSate.Initial) { CloseWeaponChanging(); }
                    if (ThrowGrenadeState == ThrowGrenadeSate.Initial) { CloseGrenadeThrowing(); }
                }
            }
            if (ChangeWeaponState == ChangeWeaponSate.Initial)
            {
                TryChangeWeapon(_selectWeaponSystem.WeaponToChange);
            }
            if (ThrowGrenadeState == ThrowGrenadeSate.Initial)
            {
                TryUseGrenade();
            }
            if (CurentGrenadeType != GrenadeType.NoGrenade && GrenadeStorage[CurentGrenadeType] == 0)
            {
                _grenadeSystem.SetGrenadeType(GrenadeType.NoGrenade);
            }
            _myMovement.Action = ChangeWeaponState != ChangeWeaponSate.Passive || ThrowGrenadeState != ThrowGrenadeSate.Passive || ActiveFire || ReloadState != ReloadWeaponSate.Passive || Aim;
        }
    }
    public bool AddWeapon(WeaponType type) => _selectWeaponSystem.AddWeapon(type);
    public void TryChangeWeapon(WeaponType type) => _selectWeaponSystem.TryChangeWeapon(type);
    public void ChangeCurentWeapon() => _selectWeaponSystem.ChangeCurentWeapon(); //used by animation event
    public void CloseWeaponChanging() => _selectWeaponSystem.CloseWeaponChanging(); //used by animation event
    public bool FillBulletsStorage(WeaponType type, int num) => _bulletsReloadSystem.FillBulletsStorage(type, num);
    public void TryReloadWeapon()
    {
        if (ReloadState == ReloadWeaponSate.Passive)
        {
            _bulletsReloadSystem.TryReloadWeapon();
        }
    }
    public void ContinueReloadWeapon()=> _bulletsReloadSystem.TryReloadWeapon(); //used by animation event
    public void ReloadAmmo() => _bulletsReloadSystem.ReloadAmmo(); //used by animation event
    public void CloseReload() => _bulletsReloadSystem.CloseReload(); //used by animation event
    public bool AddGrenade(GrenadeType type, int num) => _grenadeSystem.AddGrenade(type, num);
    public void TryUseGrenade() => _grenadeSystem.TryUseGrenade();
    public void TakeGrenade() => _grenadeSystem.TakeGrenade();//used by animation event
    public void ActivateGrenade() => _grenadeSystem.ActivateGrenade();//used by animation event
    public void ThrowGrenade() => _grenadeSystem.ThrowGrenade();//used by animation event
    public void CloseGrenadeThrowing() => _grenadeSystem.CloseGrenadeThrowing();//used by animation event
    public void StopAiming()
    {
        _aimSystem.Aim = false;
    }
    private IEnumerator TakeDefaultWeapon()
    {
        var timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
       AddWeapon(_defaultWeapon);
    }
}
public enum ChangeWeaponSate
{
    Passive,
    Initial,
    Execution
}
public class SelectWeaponSystem
{
    private CharacterAction _characterActions;
    private WeaponType _curentWeapon;
    private Dictionary<WeaponType, WeaponBehaviour> _weapons;
    private int _maxWeaponsNum;
    private WeaponType _weaponToChange;
    private ChangeWeaponSate _changeWeaponState;
    public Dictionary<WeaponType, WeaponBehaviour> Weapons => _weapons;
    public WeaponBehaviour CurentWeapon { get { if (_weapons.Count == 0 || _weapons == null || _curentWeapon == WeaponType.NoWeapon) { return null; } else { return _weapons[_curentWeapon]; } } }
    public WeaponData CurentWeaponData => CurentWeapon?.Data ?? WeaponManager.NoWeapon;
    public ChangeWeaponSate ChangeWeaponState => _changeWeaponState;
    public WeaponType WeaponToChange => _weaponToChange;

    public bool AddWeapon(WeaponType type)
    {
        if (_weapons.Count < _maxWeaponsNum && !_weapons.ContainsKey(type) && type != WeaponType.NoWeapon)
        {
            var instance = WeaponManager.Instance.GetWeaponData(type).WeaponInstance;
            instance = PoolManager.GetObject(instance);
            instance.SetActive(false);
            WeaponBehaviour weapon;
            instance.TryGetComponent(out weapon);
            instance.transform.parent = _characterActions.WeaponHolder;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _weapons.Add(type, weapon);
            TryChangeWeapon(type);
            return true;
        }
        return false;
    }
    public void TryChangeWeapon(WeaponType type)
    {
        if (_changeWeaponState != ChangeWeaponSate.Execution)
        {
            if (_weapons.ContainsKey(type) && type != CurentWeaponData.Type)
            {
                if (_characterActions.ReloadState == ReloadWeaponSate.Passive && _characterActions.ThrowGrenadeState != ThrowGrenadeSate.Execution)
                {
                    _weaponToChange = type;
                    _characterActions.StopAiming();
                    _characterActions.ActiveFire = false;
                    _changeWeaponState = ChangeWeaponSate.Execution;
                }
                else
                {
                    _weaponToChange = type;
                    _changeWeaponState = ChangeWeaponSate.Initial;
                }
                if (_characterActions.ThrowGrenadeState == ThrowGrenadeSate.Initial) { _characterActions.CloseGrenadeThrowing(); }
            }
            else
            {
                _changeWeaponState = ChangeWeaponSate.Passive;
            }
        }
    }
    public void ChangeCurentWeapon() //used by animation event
    {
        if (_weapons.ContainsKey(_weaponToChange))
        {
            _curentWeapon = _weaponToChange;
            foreach (var weap in _weapons)
            {
                weap.Value.gameObject.SetActive(false);
            }
            CurentWeapon.gameObject.SetActive(true);
        }
    }
    public void CloseWeaponChanging() //used by animation event
    {
        _changeWeaponState = ChangeWeaponSate.Passive;
    }
    public SelectWeaponSystem(CharacterAction action, int maxNum)
    {
        _characterActions = action;
        _maxWeaponsNum = maxNum;
        _curentWeapon = WeaponType.NoWeapon;
        _weapons = new Dictionary<WeaponType, WeaponBehaviour>();
        _changeWeaponState = ChangeWeaponSate.Passive;
    }
}
public class AimWeaponSystem
{
    private CharacterAction _characterActions;
    private MultiAimConstraint _weaponAimConstraint;
    private LayerMask _aimTargetLayers;
    public bool Aim { get; set; }

    public bool TrySwitchAimPosition()
    {
        if (_characterActions.ChangeWeaponState== ChangeWeaponSate.Passive && _characterActions.ThrowGrenadeState==ThrowGrenadeSate.Passive &&
            _characterActions.ReloadState==ReloadWeaponSate.Passive)
        {
            Aim = !Aim;
            return true;
        }
        return false;
    }
    public Vector3 AimWeapon()
    {
        if (_characterActions.Sprint || _characterActions.ChangeWeaponState!=ChangeWeaponSate.Passive ||
            _characterActions.ReloadState != ReloadWeaponSate.Passive ||_characterActions.ThrowGrenadeState!=ThrowGrenadeSate.Passive)
        {
            _weaponAimConstraint.weight = 0f;
            return Vector3.zero;
        }
        else
        {
            _weaponAimConstraint.weight = 1f;
            RaycastHit hit;
            return Physics.Raycast(_characterActions.MyView.position+ _characterActions.MyView.forward, _characterActions.MyView.forward, out hit, WeaponManager.DistanceToOffsetTarget, _aimTargetLayers) ?
                hit.point : _characterActions.MyView.position + _characterActions.MyView.forward * WeaponManager.DistanceToOffsetTarget;
        }
    }
    public AimWeaponSystem(CharacterAction action, MultiAimConstraint aimConstr, LayerMask targetLayers)
    {
        _characterActions = action;
        _aimTargetLayers = targetLayers;
        _weaponAimConstraint = aimConstr;
        Aim = false;
    }
}
public enum ReloadWeaponSate
{
    Passive,
    Execution,
    Ending
}
public class BulletsReloadSystem
{
    private CharacterAction _characterActions;
    private Dictionary<WeaponType, int> _myBulletStorage;
    private ReloadWeaponSate _reloadState;
    private int _bulletsToReload;
    public Dictionary<WeaponType, int> BulletStorage => _myBulletStorage;
    public ReloadWeaponSate ReloadState => _reloadState;

    public bool FillBulletsStorage(WeaponType type, int num)
    {
        if (type != WeaponType.NoWeapon && num > 0)
        {
            _myBulletStorage[type] = _myBulletStorage.ContainsKey(type) ? _myBulletStorage[type] + num : num;
            return true;
        }
        return false;
    }
    public void TryReloadWeapon() //used by animation event
    {
        if (_characterActions.MoveState != MovementState.Fall && _characterActions.ChangeWeaponState== ChangeWeaponSate.Passive && 
            !_characterActions.ActiveFire && _characterActions.ThrowGrenadeState==ThrowGrenadeSate.Passive&&
            _characterActions.CurentWeapon.ReloadBullets > 0 && _myBulletStorage[_characterActions.CurentWeaponData.Type] > 0)
        {
            _bulletsToReload = _characterActions.CurentWeapon.ReloadBullets > _myBulletStorage[_characterActions.CurentWeaponData.Type] ?
                _myBulletStorage[_characterActions.CurentWeaponData.Type] : _characterActions.CurentWeapon.ReloadBullets;
            _reloadState = ReloadWeaponSate.Execution;
            _characterActions.StopAiming();
        }
        else
        {
            _bulletsToReload = 0;
            _reloadState = _reloadState == ReloadWeaponSate.Execution ? ReloadWeaponSate.Ending : ReloadWeaponSate.Passive;
        }
    }
    public void ReloadAmmo()//used by animation event
    {
        _myBulletStorage[_characterActions.CurentWeaponData.Type] -= _bulletsToReload;
        _characterActions.CurentWeapon.FillMagazine(_bulletsToReload);
    }
    public void CloseReload()//used by animation event
    {
        _reloadState = ReloadWeaponSate.Passive;
    }
    public BulletsReloadSystem(CharacterAction action)
    {
        _characterActions = action;
        _myBulletStorage = new Dictionary<WeaponType, int>();
        _reloadState = ReloadWeaponSate.Passive;
    }
}
public enum ThrowGrenadeSate
{
    Passive,
    Initial,
    Execution
}
public class GrenadeSystem
{
    private CharacterAction _characterActions;
    private GrenadeType _curentGrenadeType;
    private Dictionary<GrenadeType, int> _myGrenadeStorage;
    private ThrowGrenadeSate _throwGrenadeState;
    private GrenadeBehaviour _grenade;
    public GrenadeType CurentGrenadeType => _curentGrenadeType;
    public Dictionary<GrenadeType, int> GrenadeStorage => _myGrenadeStorage;
    public ThrowGrenadeSate ThrowGrenadeState => _throwGrenadeState;
    
    public bool AddGrenade(GrenadeType type, int num)
    {
        if (type != GrenadeType.NoGrenade && num>0)
        {
            _myGrenadeStorage[type] = _myGrenadeStorage.ContainsKey(type) ? _myGrenadeStorage[type] + num : num;
            _curentGrenadeType = _curentGrenadeType == GrenadeType.NoGrenade ? type : _curentGrenadeType;
            return true;
        }
        return false;
    }
    public void SetGrenadeType(GrenadeType type)
    {
        _curentGrenadeType = type;
    }
    public void TryUseGrenade()
    {
        if (_throwGrenadeState != ThrowGrenadeSate.Execution)
        {
            if (_curentGrenadeType != GrenadeType.NoGrenade && _myGrenadeStorage[_curentGrenadeType] > 0)
            {
                if (_characterActions.ReloadState == ReloadWeaponSate.Passive && _characterActions.ChangeWeaponState != ChangeWeaponSate.Execution)
                {
                    _characterActions.StopAiming();
                    _characterActions.ActiveFire = false;
                    _throwGrenadeState = ThrowGrenadeSate.Execution;
                }
                else
                {
                    _throwGrenadeState = ThrowGrenadeSate.Initial;
                }
                if (_characterActions.ChangeWeaponState == ChangeWeaponSate.Initial) { _characterActions.CloseWeaponChanging(); }
            }
            else
            {
                _throwGrenadeState = ThrowGrenadeSate.Passive;
            }
        }
    }
    public void TakeGrenade()//used by animation event
    {
        _myGrenadeStorage[_curentGrenadeType] -= 1;
        var grenade = PoolManager.GetObject(WeaponManager.Instance.GetGrenadeData(_curentGrenadeType).Instance);
        if (grenade != null)
        {
            grenade.transform.parent = _characterActions.GrenadeHolder;
            grenade.transform.localPosition = Vector3.zero;
            grenade.transform.localRotation = Quaternion.Euler(Vector3.zero);
            grenade.TryGetComponent(out _grenade);
            grenade.SetActive(true);
        }
    }
    public void ActivateGrenade()//used by animation event
    {
        _grenade?.Activate(0);
    }
    public void ThrowGrenade()//used by animation event
    {
        _grenade?.Throw(_characterActions.MyTarget.position);
        _grenade = null;
    }
    public void CloseGrenadeThrowing()//used by animation event
    {
        _throwGrenadeState = ThrowGrenadeSate.Passive;
    }
    public GrenadeSystem(CharacterAction action)
    {
        _characterActions = action;
        _curentGrenadeType = GrenadeType.NoGrenade;
        _myGrenadeStorage = new Dictionary<GrenadeType, int>();
        _throwGrenadeState = ThrowGrenadeSate.Passive;
    }
}

