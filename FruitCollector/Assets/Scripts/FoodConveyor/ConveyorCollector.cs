using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorCollector : MonoBehaviour
{
    System.Action<FoodUnit> _foodCollected;

    public void SetCollectorAction(System.Action<FoodUnit> action)
    {
        _foodCollected = action;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<FoodUnit>(out var food))
        {
            _foodCollected?.Invoke(food);
        }
    }
}
