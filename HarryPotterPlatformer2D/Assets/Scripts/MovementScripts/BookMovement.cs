using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookMovement : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpFrequency;
    public float JumpFrequency => _jumpFrequency;
    [SerializeField] private float _attackJumpFrequency;
    public float AttackJumpFrequency => _attackJumpFrequency;
    [SerializeField] private float _patrolDistance;
    [SerializeField] private float _turnAroundTime;
    public float TurnAroundTime => _turnAroundTime;
    [SerializeField] private float _attackRunTime;
    [SerializeField]
    [Range(0, 1)] private float _jumpVectorX;
    [SerializeField]
    [Range(0, 1)] private float _jumpVectorY;
    [SerializeField] private float _rayLength;
    [SerializeField] private float _enemyVisionLength;
    [SerializeField] private int _groundLayerNum;
    [SerializeField] private int _platformLayerNum;
    [SerializeField] private int _heroLayerNum;
    CharacterHealth _myHealth;
    Rigidbody2D _myBody;
    BoxCollider2D _myCollider;
    private float _curentJumpTime;
    private float _curentAttackRunTime;
    private Vector3 _patrolPositionSecond;
    private Vector3 _patrolPositionFirst;
    private float _timeToTurn;
    private GameObject _attackTarget;
    Vector2 _patrolJumpVector;
    Vector2 _backOnPositionVector;
    BookState _myState;
    public int CurentBookState => (int)_myState;
    private bool _grounded;
    private enum BookState
    {
        Patroling,
        Attacking,
        Turning,
        BackOnPosition
    }
    void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _myHealth = GetComponent<CharacterHealth>();
        _curentJumpTime =0f;
        _patrolPositionFirst = transform.position;
        _patrolPositionSecond = new Vector2( transform.position.x + _patrolDistance,transform.position.y);
        _patrolJumpVector = new Vector2(_jumpVectorX * _jumpForce, _jumpVectorY * _jumpForce);
        _myState = BookState.Patroling;
    }

    void FixedUpdate()
    {
        if (_myState == BookState.Patroling || _myState == BookState.Turning)
        {
            TurnPatrolAround();
        }
        else if (_myState==BookState.Attacking)
        {
            if (_curentAttackRunTime<=0)
            {
                _backOnPositionVector = (_patrolPositionFirst - transform.position).magnitude < (_patrolPositionSecond - transform.position).magnitude ?
                    _patrolJumpVector : new Vector2(-_patrolJumpVector.x, _patrolJumpVector.y);
                _myState = BookState.BackOnPosition;
            }
        }
        else if (_myState == BookState.BackOnPosition && (transform.position-_patrolPositionFirst).magnitude<_patrolDistance
            && (transform.position - _patrolPositionFirst).magnitude < _patrolDistance)
        {
            _myState = BookState.Patroling;
            if (_backOnPositionVector == -_patrolJumpVector)
            {
                ChangePatrolDirection();
            }
        }
        CheckEnemy();
        Jump();
        _curentAttackRunTime -= Time.fixedDeltaTime;
        _timeToTurn -= Time.fixedDeltaTime;
        _curentJumpTime -= Time.fixedDeltaTime;
    }
    private void Jump()
    {
        CheckGround();
        if (_curentJumpTime <= 0f && _grounded)
        {
            if (_myState==BookState.Turning)
            {
                _myBody.AddForce(Vector2.up * _jumpVectorY * _jumpForce, ForceMode2D.Impulse);
                _curentJumpTime = 1 / _jumpFrequency;
            }
            else if(_myState == BookState.Patroling)
            {
                _myBody.AddForce(_patrolJumpVector, ForceMode2D.Impulse);
                _curentJumpTime = 1 / _jumpFrequency;
            }
            else if (_myState == BookState.Attacking)
            {

                _myBody.AddForce(new Vector2((_attackTarget.transform.position - transform.position).normalized.x*_jumpForce, _patrolJumpVector.y), ForceMode2D.Impulse);
                _curentJumpTime = 1 / _attackJumpFrequency;
            }
            else if (_myState == BookState.BackOnPosition)
            {
                _myBody.AddForce(_backOnPositionVector, ForceMode2D.Impulse);
                _curentJumpTime = 1 / _jumpFrequency;
            }
        }
    }
    private void CheckGround()
    {
        var hit = Physics2D.RaycastAll(_myCollider.bounds.center, Vector2.down, _rayLength);
        if (hit.Count(p => p.collider.gameObject.layer == _groundLayerNum || p.collider.gameObject.layer == _platformLayerNum) != 0)
        {
            _grounded = true;
           _myBody.velocity = Vector2.zero;
        }
        else
        {
            _grounded = false;
        }
    }
    private void CheckEnemy()
    {
        var hit = Physics2D.RaycastAll(new Vector3(_myCollider.bounds.center.x + _enemyVisionLength, _myCollider.bounds.center.y,0), Vector2.left, _enemyVisionLength*2);
        if (hit.Count(p => p.collider.gameObject.layer == _heroLayerNum) != 0)
        {
            foreach (var item in hit.Where(p => p.collider.gameObject.layer == _heroLayerNum))
            {
                _attackTarget = item.collider.gameObject;
            }
            _myState = BookState.Attacking;
            _curentAttackRunTime = _attackRunTime;
        }
    }
    void TurnPatrolAround()
    {
        if ((transform.position - _patrolPositionFirst).magnitude >= _patrolDistance)
        {
            if (_myState != BookState.Turning)
            {
                _myState = BookState.Turning;
                _timeToTurn = _turnAroundTime;
            }
            if (_timeToTurn <= 0)
            {
                ChangePatrolDirection();
                _myState = BookState.Patroling;
            }
        }
    }
    void ChangePatrolDirection()
    {
        _patrolJumpVector = new Vector2(-_patrolJumpVector.x, _patrolJumpVector.y);
        Vector2 pos = _patrolPositionFirst;
        _patrolPositionFirst = _patrolPositionSecond;
        _patrolPositionSecond = pos;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == _heroLayerNum && !_myHealth.Invulnerability)
        {
            Vector2 vector2 = new Vector2(collision.GetContact(0).normal.x, 0.9f);
            _myBody.AddForce(vector2 * _jumpForce, ForceMode2D.Impulse);
        }
    }
}
