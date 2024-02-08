using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class FireJoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float _deadZone;
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private Vector2 _defaultAnchoredPosition;
    [SerializeField] private Vector2 _defaultSize;
    [SerializeField] private Vector2 _activeSize;
    private bool _active;
    public bool Active => _active;
    private RectTransform _baseRect;
    private Canvas _canvas;
    private Camera _cam;
    private Vector2 _input;
    private Vector2 _prevInput;
    private Vector2 _deltaInput;
    public float Horizontal => _deltaInput.x;
    public float Vertical => _deltaInput.y;
    public Vector2 Direction =>  _deltaInput;

    private void Start()
    {
        _active = false;
        _input = Vector2.zero;
        _prevInput = Vector2.zero;
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        Vector2 center = new Vector2(0.5f, 0.5f);
        _background.anchorMin = _defaultAnchoredPosition;
        _background.anchorMax = _defaultAnchoredPosition;
        _background.anchoredPosition = Vector2.zero;
        _background.pivot = center;
        _handle.anchorMin = center;
        _handle.anchorMax = center;
        _handle.pivot = center;
        _handle.anchoredPosition = Vector2.zero;
    }
    private void Update()
    {
        _background.sizeDelta = _active ? _activeSize : _defaultSize;
        _handle.sizeDelta = _active ? _activeSize : _defaultSize;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _background.anchorMin = Vector2.zero;
        _background.anchorMax = Vector2.zero;
        _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        _active = true;
        _prevInput = ScreenPointToRectViewport(eventData.position);
        OnDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        _cam = null;
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera) { _cam = _canvas.worldCamera; }
        Vector2 position = RectTransformUtility.WorldToScreenPoint(_cam, _background.position);
        Vector2 radius = _background.sizeDelta / 2;
        _input = (eventData.position - position) / (radius * _canvas.scaleFactor);
        _input = _input.magnitude > _deadZone ? (_input.magnitude > 1 ? _input.normalized : _input) : Vector2.zero;
        _handle.anchoredPosition = _input * radius;
        var newPos = ScreenPointToRectViewport(eventData.position);
        _deltaInput = newPos - _prevInput;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _active = false;
        _input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
        _background.anchorMin = _defaultAnchoredPosition;
        _background.anchorMax = _defaultAnchoredPosition;
        _background.anchoredPosition = Vector2.zero;
        _deltaInput = Vector2.zero;
    }
    private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out Vector2 localPoint))
        {           
            return localPoint;
        }
        return Vector2.zero;
    }
    private Vector2 ScreenPointToRectViewport(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out Vector2 localPoint))
        {
            return new Vector2(localPoint.x / _baseRect.rect.size.x, localPoint.y / _baseRect.rect.size.y);
        }
        return Vector2.zero;
    }
}