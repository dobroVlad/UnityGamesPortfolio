using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private CharacterMovement _myMovement;
    [SerializeField] private CharacterAction _myAction;
    private WeaponType _previousType;
    private void Start()
    {
        _previousType = WeaponType.NoWeapon;
    }
    void Update()
    {
        var moveDirection = _myMovement.MoveDirection.normalized;
        var viewDirection = _myMovement.SnapedViewDirection.x;
        if (_myMovement.State != MovementState.Fall)
        {
            _characterAnimator.SetBool(CharacterAnimationParameters.boolFall.ToString(), false);
            _characterAnimator.SetBool(CharacterAnimationParameters.boolRun.ToString(), moveDirection != Vector2.zero);
            if (moveDirection != Vector2.zero)
            {
                _characterAnimator.SetFloat(CharacterAnimationParameters.floatHorizontal.ToString(), moveDirection.x);
                _characterAnimator.SetFloat(CharacterAnimationParameters.floatVertical.ToString(), moveDirection.y);
            }
            _characterAnimator.SetBool(CharacterAnimationParameters.boolTurn.ToString(), viewDirection != 0);
            if (viewDirection != 0)
            {
                _characterAnimator.SetFloat(CharacterAnimationParameters.floatTurn.ToString(), viewDirection);
            }
        }
        else
        {
            _characterAnimator.SetBool(CharacterAnimationParameters.boolRun.ToString(), false);
            _characterAnimator.SetBool(CharacterAnimationParameters.boolTurn.ToString(), false);
        }
        if (_myMovement.JumpTrigger)
        {
            _characterAnimator.SetTrigger(CharacterAnimationParameters.triggerJump.ToString());
            _myMovement.JumpTrigger = false;
        }
        _characterAnimator.SetBool(CharacterAnimationParameters.boolCrouch.ToString(), _myMovement.Crouch);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolFall.ToString(),_myMovement.FallingCheckGround);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolSprint.ToString(), _myMovement.Sprint);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolAim.ToString(), _myAction.Aim);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolChangeWeapon.ToString(), _myAction.ChangeWeaponState == ChangeWeaponSate.Execution);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolReload.ToString(), _myAction.ReloadState==ReloadWeaponSate.Execution);
        _characterAnimator.SetBool(CharacterAnimationParameters.boolGrenade.ToString(), _myAction.ThrowGrenadeState == ThrowGrenadeSate.Execution);
        if (_previousType != _myAction.CurentWeaponData.Type)
        {
            _previousType = _myAction.CurentWeaponData.Type;
            _characterAnimator.SetInteger(CharacterAnimationParameters.intCurentWeapon.ToString(), (int)_myAction.CurentWeaponData.Type);
        }
        if (_myAction.CurentWeapon?.WeaponAnimator != null)
        {
            _myAction.CurentWeapon.WeaponAnimator.SetBool(WeaponAnimationParameters.boolReload.ToString(), _myAction.ReloadState == ReloadWeaponSate.Execution);
        }
    }
}
public enum CharacterAnimationParameters
{
    floatVertical,
    floatHorizontal,
    boolCrouch,
    boolRun,
    floatTurn,
    boolTurn,
    boolFall,
    triggerJump,
    boolSprint,
    boolAim,
    boolChangeWeapon,
    intCurentWeapon,
    boolReload,
    boolGrenade
}
public enum WeaponAnimationParameters
{
    boolReload
}
