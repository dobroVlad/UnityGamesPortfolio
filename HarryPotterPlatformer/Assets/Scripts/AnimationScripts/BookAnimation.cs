using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    [SerializeField] Rigidbody2D _myBody;
    [SerializeField] SpriteRenderer _myRender;
    [SerializeField] Animator _myAnimator;
    [SerializeField] BookMovement _myMoves;
    private float _attackSpeed;
    private float _patrolSpeed;
    private float _turnSpeed;
    private void Awake()
    {
        _attackSpeed = _myMoves.AttackJumpFrequency;
        _patrolSpeed = _myMoves.JumpFrequency;
        _turnSpeed = 1/ _myMoves.TurnAroundTime;
    }
    void FixedUpdate()
    {
        _myRender.flipX = _myBody.velocity.x < -0.01f ? true : _myBody.velocity.x > 0.01f ? false : _myRender.flipX;
        _myAnimator.SetBool(AnimationParameters.BookMonster.boolTurn.ToString(), _myMoves.CurentBookState == 2);
        _myAnimator.SetFloat(AnimationParameters.BookMonster.floatSpeed.ToString(), _myMoves.CurentBookState == 2 ? _turnSpeed : _myMoves.CurentBookState == 1 ? _attackSpeed : _patrolSpeed);
    }  
}
