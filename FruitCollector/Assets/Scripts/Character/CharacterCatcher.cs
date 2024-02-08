using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCatcher : MonoBehaviour
{
    private System.Action<FoodUnit> _takeFood;
    private FoodUnit _targetFood;

    public void SetTakerAction(System.Action<FoodUnit> action)
    {
        _takeFood = action;
    }
    public void SetTarget(FoodUnit target)
    {
        _targetFood = target;
    }
    private void OnTriggerEnter(Collider other)
    { 
        if(_targetFood!=null && other.gameObject.TryGetComponent<FoodUnit>(out var food)&& food == _targetFood)
        {
            _takeFood?.Invoke(food);
            _targetFood=null;
        }
    }
}
