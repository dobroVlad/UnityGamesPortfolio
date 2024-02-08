using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayThreeBlocksMovement : MonoBehaviour
{
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _movementTime;
    [SerializeField] [Range(0,1)] private float _changeSpeedPath; 
    [SerializeField] private float _pauseTime;
    [SerializeField] private GameObject _stairBlock0;
    [SerializeField] private GameObject _stairBlock1;
    [SerializeField] private GameObject _stairBlock2;
    [SerializeField] private List<int> _indexOrder0;
    [SerializeField] private List<int> _indexOrder1;
    [SerializeField] private List<int> _indexOrder2;
    private void Awake()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 1; i > -2; i--)
        {
            for (int k = -1; k <2; k++)
            {
                //if (i == -1 && k == 1) { continue; }
                points.Add(new Vector3(k, i, 0) * _moveDistance);
            }
        }
        List<GameObject> stairs = new List<GameObject>() { _stairBlock0, _stairBlock1, _stairBlock2 };
        List<List<int>> orders = new List<List<int>>() { _indexOrder0, _indexOrder1, _indexOrder2 };
        for (int i = 0; i < 3; i++)
        {
            stairs[i].GetComponent<StairwayMove>().SetMovementProperties(_movementTime, _changeSpeedPath, _pauseTime);
            List<Vector3> list0 = new List<Vector3>();
            foreach (int index in orders[i])
            {
                list0.Add(points[index]);
            }
            stairs[i].GetComponent<StairwayMove>().SetPoints(list0);
        }
    }
}
