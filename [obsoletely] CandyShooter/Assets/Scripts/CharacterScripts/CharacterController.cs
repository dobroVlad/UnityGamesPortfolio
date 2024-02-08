using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterMovement _myMovement;
    [SerializeField] private CharacterAction _myAction;
    [SerializeField] private CharacterHealth _myHealth;
    public Transform ViewTransform => _myAction.MyView;
    public CharacterMovement Movement => _myMovement;
    public CharacterAction Action => _myAction;
    public CharacterHealth Health => _myHealth;
}
