using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StairwayMove : MonoBehaviour
{
    [SerializeField] private List<Vector3> _positions;
    [SerializeField] private float _movementTime; //T
    [SerializeField] [Range(0,1)] private float _changeSpeedPath; // k - part of the path for acceleration after start and to slow down before the finish.
    [SerializeField] private float _pauseTime;
    private Rigidbody2D _myBody;
    private int _nextPositionNum; //Finish (pos. B)
    private int _previousPositionNum; //Start (pos. A)
    private float _curentPathLength; // S - the path between A and B.
    private Vector3 _curentPathVelocity; // V - needed velocity to go through path S in time T.
    private float _waitTime;

    private void Awake()
    {
        if (_positions != null)
        {
            transform.localPosition = _positions[0];
            _previousPositionNum = 0;
        }
        _myBody = gameObject.GetComponent<Rigidbody2D>();
        if (_positions.Count > 1)
        {
            _nextPositionNum = 1;
        }
        _waitTime = 0;
        _curentPathLength = (_positions[_nextPositionNum] - _positions[_previousPositionNum]).magnitude;
        //S = S*k + (S-2S*k) + S*k = V*T1/2 + V*(T-2T1) + V*T1/2 = V*(T-T1); => V = S/(T-T1); V = 2*S*k/T1; =>
        //=> T-T1 = T1/2k => T1 = 2k*T/(2k+1); => V = 2Sk *(2k+1)/(2k*T) = S(2k+1)/T;
        _curentPathVelocity = (_positions[_nextPositionNum] - _positions[_previousPositionNum]) * (2 *_changeSpeedPath +1) / _movementTime;
    }
    private void FixedUpdate()
    {
        if (_waitTime <= 0)
        {
            if (_positions.Count > 1)
            {   float passedLength = (transform.localPosition - _positions[_previousPositionNum]).magnitude;
                if (_curentPathLength>passedLength)
                {
                    if (passedLength < _curentPathLength*_changeSpeedPath)
                    {
                        _myBody.velocity = Vector2.Lerp(_curentPathVelocity*0.1f, _curentPathVelocity, passedLength/(_curentPathLength * _changeSpeedPath));
                    }
                    else if (passedLength >= _curentPathLength * (1-_changeSpeedPath))
                    {
                        _myBody.velocity = Vector2.Lerp(_curentPathVelocity, _curentPathVelocity * 0.1f, 
                            (passedLength - _curentPathLength*(1-_changeSpeedPath))/(_curentPathLength* _changeSpeedPath));
                    }
                }
                else
                {
                    _previousPositionNum = _nextPositionNum;
                    _myBody.velocity = Vector2.zero;
                    transform.localPosition = _positions[_nextPositionNum];
                    if (_nextPositionNum == _positions.Count - 1)
                    {
                        _nextPositionNum = 0;
                    }
                    else
                    {
                        _nextPositionNum += 1;
                    }
                    _waitTime = _pauseTime;
                    _curentPathLength = (_positions[_nextPositionNum] - _positions[_previousPositionNum]).magnitude;
                    _curentPathVelocity = (_positions[_nextPositionNum] - _positions[_previousPositionNum]) * (2 + _changeSpeedPath) / (2 * _movementTime);
                }
            }
        }
        else
        {
            _waitTime -= Time.deltaTime;
        }
    }
    public void SetPoints(List<Vector3> list)
    {
        _positions = list;
    }
    public void SetMovementProperties(float period, float path, float pause)
    {
        _pauseTime = pause;
        _movementTime = period;
        _changeSpeedPath = path;
    }
}
