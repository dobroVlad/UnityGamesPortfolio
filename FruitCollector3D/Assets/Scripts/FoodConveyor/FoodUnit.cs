using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodUnit : MonoBehaviour
{
    [SerializeField] private Rigidbody _myBody;
    [SerializeField] private FoodType _myType;
    private bool _taken = false;
    public FoodType Type => _myType;
    public bool Taken => _taken;
    public Rigidbody Body => _myBody;
    
    public void SetTaken()
    {
        _taken = true;
    } 
}
