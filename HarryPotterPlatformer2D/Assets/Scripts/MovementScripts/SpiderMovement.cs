using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    [SerializeField] SpiderState _myState;
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;
    [SerializeField] private float _waitTime;
    [SerializeField] private SpriteRenderer _mySpriteRenderer;
    [SerializeField] private Rigidbody2D _myBody;
    private Vector3 _position1;
    private Vector3 _position2;
    private Vector2 _spriteSize;
    private enum SpiderState
    {
        Falling,
        Runiing
    }

    private void Awake()
    {
        _position1 = transform.position;
        _position2 = _myState == SpiderState.Falling ? 
            new Vector3(_position1.x, _position1.y - Mathf.Abs(_distance), 0) :
            new Vector3(_position1.x + _distance, _position1.y, 0);
        _mySpriteRenderer.drawMode = _myState == SpiderState.Falling ? SpriteDrawMode.Sliced : SpriteDrawMode.Simple;
        _spriteSize = _mySpriteRenderer.size;
    }
    void Start()
    {
        if (_myState == SpiderState.Falling)
        {
            StartCoroutine(MoveUpDown(_position1, _position2));
        }
        else if (_myState == SpiderState.Runiing)
        {
            StartCoroutine(MoveLeftRight(_position1, _position2));
        }
    }
    private IEnumerator MoveUpDown(Vector3 pos1, Vector3 pos2)
    {
        var way = 0f;
        var acceleration = _distance / 4;
        while (way < acceleration)
        {
            _myBody.velocity = new Vector2(0f, Mathf.Lerp(0.1f, _speed, way / acceleration) * (pos2 - pos1).normalized.y);
            way = (transform.position - pos1).magnitude;
            _mySpriteRenderer.size = new Vector2(_spriteSize.x, _spriteSize.y - way * (pos2 - pos1).normalized.y);
            yield return null;
        }
        while (way < _distance - acceleration)
        {
            way = (transform.position - pos1).magnitude;
            _mySpriteRenderer.size = new Vector2(_spriteSize.x, _spriteSize.y - way * (pos2 - pos1).normalized.y);
            yield return null;
        }
        while (way < _distance)
        {
            _myBody.velocity = new Vector2(0f, Mathf.Lerp(_speed, 0.1f, (way - _distance + acceleration) / acceleration) * (pos2 - pos1).normalized.y);
            way = (transform.position - pos1).magnitude;
            _mySpriteRenderer.size = new Vector2(_spriteSize.x, _spriteSize.y - way * (pos2 - pos1).normalized.y);
            yield return null;
        }
        _mySpriteRenderer.size = new Vector2(_spriteSize.x, _spriteSize.y - _distance * (pos2 - pos1).normalized.y);
        _myBody.velocity = Vector2.zero;
        transform.position = pos2;
        _spriteSize = _mySpriteRenderer.size;
        var timer = 0f;
        while (timer < _waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(MoveUpDown(pos2, pos1));
    }
    private IEnumerator MoveLeftRight (Vector3 pos1, Vector3 pos2)
    {
        var way = 0f;
        _myBody.velocity = new Vector2( _speed * (pos2 - pos1).normalized.x, 0f);
        _mySpriteRenderer.flipX = _myBody.velocity.x < -0.01f;
        while (way <Mathf.Abs( _distance))
        {
            way = (transform.position - pos1).magnitude;
            yield return null;
        }
        _myBody.velocity = Vector2.zero;
        transform.position = pos2;
        var timer = 0f;
        while (timer < _waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(MoveLeftRight(pos2, pos1));
    }
}
