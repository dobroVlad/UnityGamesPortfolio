using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SplineGenerator : MonoBehaviour
{
    [SerializeField]
    SpriteShapeController _spriteShapeController;
    [SerializeField, Range(0,1000)]
    private int _levelLength = 50;
    [SerializeField, Range(0, 100)]
    private int _bottom = 20;
    [SerializeField, Range(0f, 1f)]
    private float _noiseY = 0.7f;
    [SerializeField, Range(0f, 10f)]
    private float _multiplyY = 2f;
    [SerializeField, Range(0f, 1f)]
    private float _splineSmoothness = 0.5f;
    [ContextMenu("Create Spline")]
    public void CreateSpline()
    {
        _spriteShapeController.spline.Clear();
        Vector3 point = Vector3.zero;
        for(int i =0; i < _levelLength; i++)
        {
            point = new Vector3(i, Mathf.PerlinNoise(0,i* _noiseY) *_multiplyY) +transform.position;
            _spriteShapeController.spline.InsertPointAt(i, point);
            if(i!=0 && i < _levelLength - 1)
            {
                _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                _spriteShapeController.spline.SetLeftTangent(i,Vector3.left * _splineSmoothness);
                _spriteShapeController.spline.SetRightTangent(i, Vector3.right * _splineSmoothness);
            }
        }
        _spriteShapeController.spline.InsertPointAt(_levelLength, new Vector3(point.x,transform.position.y-_bottom));
        _spriteShapeController.spline.InsertPointAt(_levelLength+1,transform.position+Vector3.down*_bottom);
    }
}
