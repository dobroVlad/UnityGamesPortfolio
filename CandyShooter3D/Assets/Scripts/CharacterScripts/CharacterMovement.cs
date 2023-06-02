using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MovementState
{
    Idle,
    Run,
    Fall
}
public enum LayersNum
{
    Ground = 3,
    Bullet=6,
    Character=7,
    ItemSphere=8,
    Walls =10
}
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _maxSprintSpeed;
    [SerializeField] private float _maxRunSpeed;
    [SerializeField] private float _maxBackwardSpeed;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;
    [SerializeField]
    [Range(0.1f,1f)] private float _crouchSpeedKoef;
    [SerializeField] private float _recoilForce;
    [SerializeField] private float _recoilReturnSpeed;
    [SerializeField] private Transform _myViewContainer;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _boxCastHight;
    [SerializeField] private LayerMask _obstacleForwardLayers;
    private MovementState _state;
    private float _speed;
    private bool _sprint;
    private bool _ableSprint;
    private bool _crouch;
    private bool _tryJump;
    private Vector2 _moveDirection;
    private Vector2 _snapedMoveDirection;
    private Vector2 _viewDirection;
    private Vector2 _snapedViewDirection;
    private Vector2 _fireDirection;
    private Vector2 _fireRecoil;
    private Vector2 _recoilTarget;
    private float _headRotaion;
    private Vector3 _groundedCastSize;
    private Vector3 _fallingCastSize;
    private Rigidbody _myBody;
    private CapsuleCollider _myCollider;
    private bool _checkForSprint;
    private bool _fallingCheckGround;
    private bool _action;
    private bool _trySprint;
    public bool CheckForSprint => _checkForSprint;
    public float SptrintAccelerationTime => 0.8f;
    public MovementState State => _state;
    public float Speed => _speed;
    public bool Sprint => _sprint;
    public bool Crouch => _crouch;
    public bool Action { set { _action = value; } }
    public Vector2 MoveDirection => _moveDirection;
    public Vector2 SnapedViewDirection => _snapedViewDirection;
    public bool JumpTrigger { get; set; }
    public bool FallingCheckGround => _fallingCheckGround;
    void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<CapsuleCollider>();
        _speed = 0f;
        _moveDirection = Vector2.zero;
        _state = MovementState.Idle;
        _checkForSprint = false;
        _ableSprint = false;
        _fallingCheckGround = false;
        _sprint = false;
        _crouch = false;
        _tryJump = false;
        _headRotaion = 0f;
        _fireRecoil = Vector2.zero;
        _recoilTarget = Vector2.zero;
        _groundedCastSize = new Vector3(_myCollider.radius, _boxCastHight / 2, _myCollider.radius);
        _fallingCastSize = new Vector3(_myCollider.radius, _boxCastHight * 4f, _myCollider.radius);
    }
    void FixedUpdate()
    {
        Movement();
        CheckGround();
        RotateView();
        _sprint = _ableSprint ? _sprint : false;
    }
    private void Update()
    {
        if (_moveDirection != Vector2.zero)
        {
            _speed += _acceleration * Time.deltaTime;
        }
        else
        {
            _speed -= _decceleration * Time.deltaTime;
        }
        _snapedMoveDirection = SnapDirection(_moveDirection);

        if(_snapedMoveDirection== Vector2.zero || _sprint)
        {
            _speed = Mathf.Clamp(_speed, _minSpeed,_maxSprintSpeed);
        }
        else if (_snapedMoveDirection == Vector2.down)
        {
            _speed = Mathf.Clamp(_speed, _minSpeed, _maxBackwardSpeed);
        }
        else
        {
            _speed = Mathf.Clamp(_speed, _minSpeed, _maxRunSpeed);
        }
        _ableSprint = _trySprint && _snapedMoveDirection == Vector2.up && !_crouch && _state != MovementState.Fall && !_action&&
            Physics.RaycastAll(transform.position + transform.TransformVector(_myCollider.center), transform.forward, 1f, _obstacleForwardLayers).Length==0;

        if (_ableSprint && !_checkForSprint && !_sprint)
        {
            StartCoroutine(StartSptint());
        }
    }
    public void SetMoveDirection(Vector2 direction, bool trySprint)
    {
        _moveDirection = direction;
        _trySprint = trySprint;
    }
    public void SetViewDirection(Vector2 direction)
    {
        _viewDirection = direction;
    }
    public void SetFireDirection(Vector2 direction)
    {
        _fireDirection = direction;
    }
    private void Movement()
    {
        var direction = transform.forward * _moveDirection.y + transform.right * _moveDirection.x;
        if (_tryJump)
        {
            _myBody.AddForce((Vector3.up + direction) * _jumpForce, ForceMode.Impulse);
            JumpTrigger = true;
        }
        _tryJump = false;
        if (_state != MovementState.Fall)

        {
            direction *= _speed * (_crouch ? _crouchSpeedKoef : 1f);
            _myBody.velocity = new Vector3(direction.x, _myBody.velocity.y,direction.z );
        }
    }
    private void RotateView()
    {
        _snapedViewDirection = SnapDirection(_fireDirection+_viewDirection);
         transform.Rotate(0f, _fireDirection.x +_viewDirection.x, 0f);
        _headRotaion -= _fireDirection.y + _viewDirection.y;
        _headRotaion= Mathf.Clamp(_headRotaion, (float)CameraRotationLimitsX.Max - 360f, (float)CameraRotationLimitsX.Min);
        _fireRecoil = Vector2.Lerp(_fireRecoil, _recoilTarget, _recoilForce * Time.fixedDeltaTime);
        _recoilTarget = Vector2.Lerp(_recoilTarget, Vector2.zero, _recoilReturnSpeed * Time.fixedDeltaTime);
        _myViewContainer.localRotation = Quaternion.Euler(Mathf.Clamp(_headRotaion - _fireRecoil.y, (int)CameraRotationLimitsX.Max - 360f, (int)CameraRotationLimitsX.Min), 0f, 0f);
    }
    public void RecoilFire(Vector2 recoil)
    {
        _recoilTarget = recoil;
        _fireRecoil = Vector2.zero;
        _headRotaion = _myViewContainer.localRotation.eulerAngles.x > 70f ? _myViewContainer.localRotation.eulerAngles.x - 360f : _myViewContainer.localRotation.eulerAngles.x;
    }
    public bool TryJump(bool jump = false)
    {
        if (!_crouch && _state != MovementState.Fall)
        {
            if (!jump) { return true; }
            _tryJump = true;
            return true;
        }
        return false;
    }
    public bool TrySwitchCrouch(bool crouch = false)
    {
        if (_state != MovementState.Fall)
        {
            if (crouch)
            {
                _crouch = !_crouch;
            }
            return true;
        }
        return false;
    }
    public void CheckGround()
    {
        var hits = Physics.BoxCastAll(transform.position,_groundedCastSize, -transform.up, transform.rotation, 0.01f);
        if (hits.Where(p => p.normal==Vector3.up).Count(p => p.collider.gameObject.layer == (int)LayersNum.Ground) > 0)
        {
            _state = _moveDirection == Vector2.zero ? MovementState.Idle : MovementState.Run;
        }
        else
        {
            _state = MovementState.Fall;
            _crouch = false;
        }
        if (!_fallingCheckGround&& _state == MovementState.Fall)
        {
            StartCoroutine(FarCheckGround());
        }
        if (_fallingCheckGround)
        {
            var futureHits = Physics.BoxCastAll(transform.position, _fallingCastSize, -transform.up, transform.rotation, 0.01f);
            if (futureHits.Where(p => p.normal == Vector3.up).Count(p => p.collider.gameObject.layer == (int)LayersNum.Ground) > 0)
            {
                _fallingCheckGround = false;
            }
        }
    }
    private Vector2 SnapDirection(Vector2 value)
    {
        if (value == Vector2.zero)
        {
            return value;
        }
        Vector2 snapDirection;
        float angle = Vector2.Angle(value, Vector2.up);
        switch ((int)(angle / 22.5f))
        {
            case 0:
                snapDirection = new Vector2(0, 1);
                break;
            case 1:
                snapDirection = value.x > 0 ? new Vector2(0.5f, 0.87f) : new Vector2(-0.5f, 0.87f);
                break;
            case 2:
                snapDirection = value.x > 0 ? new Vector2(0.87f, 0.5f) : new Vector2(-0.87f, 0.5f);
                break;
            case 3:
            case 4:
                snapDirection = value.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                break;
            case 5:
                snapDirection = value.x > 0 ? new Vector2(0.87f, -0.5f) : new Vector2(-0.87f, -0.5f);
                break;
            case 6:
                snapDirection = value.x > 0 ? new Vector2(0.5f, -0.87f) : new Vector2(-0.5f, -0.87f);
                break;
            default:
                snapDirection = new Vector2(0, -1);
                break;
        }
        return snapDirection;
    }
    private IEnumerator StartSptint()
    {

        _checkForSprint = true;
        var timer = SptrintAccelerationTime;
        while (timer > 0)
        {
            if (_ableSprint)
            {
                timer -= Time.deltaTime;
                _sprint = timer <= 0f;
                yield return null;
            }
            else
            {
                timer = 0f;
                yield return null;
            }
        }
        _checkForSprint = false;
    }
    private IEnumerator FarCheckGround()
    {
        var timer = 0.3f;
        while (timer > 0)
        {
          timer -= Time.deltaTime;
          yield return null;
        }
        _fallingCheckGround = _state == MovementState.Fall; ;
    }
}
