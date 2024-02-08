using JetBrains.Rider.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject _ourHero;
    [SerializeField] private float _dinamicOffsetX;
    [SerializeField] private float _dinamicOffsetY;
    [SerializeField] private float _staticOffsetX;
    [SerializeField] private float _staticOffsetY;
    [SerializeField] private float _dinamicOffsetSmoothingX;
    [SerializeField] private float _dinamicOffsetSmoothingY;
    private Rigidbody2D _heroBody;

    private void Awake()
    {
        _heroBody = _ourHero.GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (_ourHero.GetComponent<PlayerMovement>().OnPlatform!=null && _ourHero.GetComponent<PlayerMovement>().MyState!=2)
        {
            transform.localPosition = new Vector3(
               Mathf.Lerp(transform.localPosition.x, _heroBody.velocity.normalized.x * _dinamicOffsetX + _staticOffsetX, _dinamicOffsetSmoothingX * Time.fixedDeltaTime),
               Mathf.Lerp(transform.localPosition.y, _heroBody.velocity.normalized.y * _dinamicOffsetY + _staticOffsetY, _dinamicOffsetSmoothingY * Time.fixedDeltaTime),
               transform.position.z);
        }
        else
        {
            
            transform.localPosition = new Vector3(
                Mathf.Lerp(transform.localPosition.x, 0, _dinamicOffsetSmoothingX * Time.fixedDeltaTime),
                Mathf.Lerp(transform.localPosition.y, 0, _dinamicOffsetSmoothingY * Time.fixedDeltaTime),
                transform.position.z);
        }
    }
}
