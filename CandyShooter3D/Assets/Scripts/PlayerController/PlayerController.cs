using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Canvas _controllerCanvas;
    [SerializeField] private MovementJoyStick _myMovementJoystick;
    [SerializeField] private CameraScroll _myCameraScroll;
    [SerializeField] private FireJoyStick _myFireJoystick;
    [SerializeField] private Camera _myCamera;
    [SerializeField] private Button _jumpButton;
    [SerializeField] private Button _aimButton;
    [SerializeField] private Button _crouchButton;
    [SerializeField] private Button _reloadButton;
    [SerializeField] private WeaponSlotButton _weapon1Button;
    [SerializeField] private WeaponSlotButton _weapon2Button;
    [SerializeField] private GrenadeSlotButton _grenadeButton; 
    [SerializeField] private RectTransform _fireOffsettUi;
    [SerializeField] private HealthBar _healthBarUI;
    [SerializeField] private Slider _sprintSlider;
    [SerializeField] private TextMeshProUGUI _sprintText;
    [SerializeField] private float _defaultFOV;
    private CharacterController _myCharacter;
    private CharacterMovement _myMovement;
    private CharacterAction _myAction;
    private CharacterHealth _myHealth;
    private bool _active;
    private bool _switchZoom;
    private float _cameraZoomTimer;
    private bool _displaySprintState;
    public Vector2 MoveDirection => _myMovementJoystick.Direction;
    private void Start()
    {
        _active = false;
        _jumpButton.onClick.AddListener(()=> _myMovement.TryJump(true));
        _crouchButton.onClick.AddListener(() => _myMovement.TrySwitchCrouch(true));
        _aimButton.onClick.AddListener(() => ZoomSwitch());
        _reloadButton.onClick.AddListener(() => _myAction.TryReloadWeapon());
        _weapon1Button.SetBulletsNum(0, 0);
        _weapon2Button.SetBulletsNum(0, 0);
        _grenadeButton.SetNumber(0);
        SetActiveUI(false);
    }
    void Update()
    {
        if (_active)
        {
            _myMovement.SetMoveDirection(MoveDirection, _myMovementJoystick.Sprint);
            _myMovement.SetViewDirection(_myCameraScroll.Direction);
            _myMovement.SetFireDirection(_myFireJoystick.Direction);
            _myAction.ActiveFire = _myFireJoystick.Active;
            _jumpButton.interactable = _myMovement.TryJump();
            _crouchButton.interactable = _myMovement.TrySwitchCrouch();
            _sprintSlider.gameObject.SetActive(_myMovement.CheckForSprint || _myMovement.Sprint);
            if (_myMovement.CheckForSprint && !_displaySprintState)
            {
                StartCoroutine(DisplaySprinttate());
            }
            _sprintText.enabled = _myMovement.Sprint;
            if (_switchZoom)
            {
                ZoomSwitchLerp();
                _fireOffsettUi.gameObject.SetActive(_myCamera.fieldOfView > _myAction.CurentWeaponData.AimZoomAngle);
            }
            else
            {
                _switchZoom = _myCamera.fieldOfView != (_myAction.Aim ? _myAction.CurentWeaponData.AimZoomAngle : _defaultFOV);
            }
            if (_myAction.Weapons.Count > 1 && _weapon2Button.Type != _myAction.CurentWeaponData.Type)
            {
                SetWeaponSlot(_weapon1Button, _weapon2Button.Type);
                SetWeaponSlot(_weapon2Button, _myAction.CurentWeaponData.Type);
                SetFireOffsetUI();
            }
            else if (_myAction.Weapons.Count == 1 && _weapon1Button.Type != _myAction.CurentWeaponData.Type)
            {
                SetWeaponSlot(_weapon1Button, _myAction.CurentWeaponData.Type);
                SetWeaponSlot(_weapon2Button, WeaponType.NoWeapon);
                SetFireOffsetUI();
            }
            if (_myAction.CurentGrenadeType != _grenadeButton.Type)
            {
                SetGrenadeSlot(_grenadeButton, _myAction.CurentGrenadeType);
            }
            if (_weapon1Button.Type != WeaponType.NoWeapon)
            {
                var mag1 = _myAction.Weapons[_weapon1Button.Type].CurentBullets;
                var store1 = _myAction.BulletStorage[_weapon1Button.Type];
                if (_weapon1Button.PrevMagazineBullets != mag1 || _weapon1Button.PrevStorageBullets != store1)
                {
                    _weapon1Button.SetBulletsNum(mag1, store1);
                }
            }
            if (_weapon2Button.Type != WeaponType.NoWeapon)
            {
                var mag2 = _myAction.Weapons[_weapon2Button.Type].CurentBullets;
                var store2 = _myAction.BulletStorage[_weapon2Button.Type];
                if (_weapon2Button.PrevMagazineBullets != mag2 || _weapon2Button.PrevStorageBullets != store2)
                {
                    _weapon2Button.SetBulletsNum(mag2, store2);
                }
            }
            var grenades = _grenadeButton.Type != GrenadeType.NoGrenade ? _myAction.GrenadeStorage[_grenadeButton.Type] : 0;
            if (grenades != _grenadeButton.PrevNumber)
            {
                _grenadeButton.SetNumber(grenades);
            }
            if (_healthBarUI.Max != _myHealth.MaxHealth)
            {
                _healthBarUI.SetMaxValue(_myHealth.MaxHealth);
                _healthBarUI.UpdateHealthPoints(_myHealth.CurentHealth);
            }
            if (_healthBarUI.Value != _myHealth.CurentHealth)
            {
                _healthBarUI.UpdateHealthPoints(_myHealth.CurentHealth);
            }
        }
    }
    public void Activate(CharacterController character)
    {
        _myCharacter = character;
        _myAction = _myCharacter.Action;
        _myMovement = _myCharacter.Movement;
        _myHealth = _myCharacter.Health;
        _displaySprintState = false;
        _switchZoom = false;
        _cameraZoomTimer = 0f;
        transform.parent = _myCharacter.ViewTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        SetActiveUI(true);
        SetWeaponSlot(_weapon1Button, WeaponType.NoWeapon);
        SetWeaponSlot(_weapon2Button, WeaponType.NoWeapon);
        SetGrenadeSlot(_grenadeButton, GrenadeType.NoGrenade);
        _healthBarUI.SetMaxValue(_myHealth.MaxHealth);
        _healthBarUI.UpdateHealthPoints(_myHealth.CurentHealth);
        SetFireOffsetUI();
        _active = true;
    }
    private void SetActiveUI(bool active)
    {
        _controllerCanvas.gameObject.SetActive(active);
    }
    private void ZoomSwitch()
    {

        if (_myAction.TrySwitchAimPosition)
        {
            _switchZoom = true;
        }
    }
    private void SetWeaponSlot(WeaponSlotButton slot, WeaponType type)
    {
        slot.SetWeapon(type, () => _myAction.TryChangeWeapon(type));
    }
    private void SetGrenadeSlot(GrenadeSlotButton slot, GrenadeType type)
    {
        slot.SetGrenade(type, () => _myAction.TryUseGrenade());
    }
    private void ZoomSwitchLerp()
    {
        _myCamera.fieldOfView = Mathf.Lerp(_myCamera.fieldOfView, _myAction.Aim ? _myAction.CurentWeaponData.AimZoomAngle : _defaultFOV, 4*Mathf.Clamp(_cameraZoomTimer, 0f, 0.25f));
        _cameraZoomTimer += Time.deltaTime;
        if (_myCamera.fieldOfView ==( _myAction.Aim ? _myAction.CurentWeaponData.AimZoomAngle : _defaultFOV)) 
        {
            _cameraZoomTimer = 0f;
            _switchZoom = false;
        }
    }
    private void SetFireOffsetUI()
    {
        var center = _myCamera.transform.position + _myCamera.transform.forward * WeaponManager.DistanceToOffsetTarget;
        var screenCenter = _myCamera.WorldToScreenPoint(center);
        _fireOffsettUi.sizeDelta = 2*(_myCamera.WorldToScreenPoint(center + (_myCamera.transform.right+ _myCamera.transform.up) * _myAction.CurentWeaponData.Offset) - screenCenter);
    }
    private IEnumerator DisplaySprinttate()
    {
        _displaySprintState = true;
        var timer = 0f;
        var duration = _myMovement.SptrintAccelerationTime;
        while (timer < duration)
        {
            if (!_myMovement.CheckForSprint) { timer = duration; }
            else
            {
                _sprintSlider.value = Mathf.Lerp(_sprintSlider.minValue, _sprintSlider.maxValue, timer / duration);
                timer += Time.deltaTime;
            }
            yield return null;
        }
        _sprintSlider.value = _myMovement.Sprint ? _sprintSlider.maxValue : _sprintSlider.minValue;
       _displaySprintState = false;
    }
}
