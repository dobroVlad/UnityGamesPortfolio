using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CameraScroll : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float _scrollSpeed;
    private bool _active;
    public bool Active => _active;
    private RectTransform _baseRect;
    private Vector2 _prevInput;
    private Vector2 _deltaInput;
    public float Horizontal => _deltaInput.x;
    public float Vertical =>_deltaInput.y;
    public Vector2 Direction => _scrollSpeed * _deltaInput;


    private void Start()
    {
        _active = false;
        _prevInput = Vector2.zero;
        _baseRect = GetComponent<RectTransform>();
    }
    private void LateUpdate()
    {
        if (_active)
        {
            _deltaInput = Vector2.zero;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _prevInput = ScreenPointToRectViewport(eventData.position);
        _active = true;
        OnDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        var newPos = ScreenPointToRectViewport(eventData.position);
        _deltaInput = _prevInput-newPos;
        _prevInput = newPos;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _deltaInput = Vector2.zero;
        _active = false;
    }
    private Vector2 ScreenPointToRectViewport(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition,null, out Vector2 localPoint))
        {
            return new Vector2( localPoint.x / _baseRect.rect.size.x, localPoint.y / _baseRect.rect.size.y);
        }
        return Vector2.zero;
    }
}