using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class MovementJoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float _deadZone;
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private Vector2 _defaultAnchoredPosition;
    private bool _active;
    public bool Active => _active;
    private RectTransform _baseRect;
    private Canvas _canvas;
    private Camera _cam;
    private Vector2 _input;
    private Vector2 _lateInput;
    private bool _sprint;
    public bool Sprint => _sprint;
    public float Horizontal => _lateInput.x;
    public float Vertical => _lateInput.y;
    public Vector2 Direction => _lateInput;

    private void Start()
    {
        _sprint = false;
        _active = false;
        _input = Vector2.zero;
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
    void Update()
    {
        _lateInput = _input;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _background.anchorMin = Vector2.zero;
        _background.anchorMax = Vector2.zero;
        _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        _active = true;
        OnDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        _cam = null;
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera) { _cam = _canvas.worldCamera; }
        Vector2 position = RectTransformUtility.WorldToScreenPoint(_cam, _background.position);
        Vector2 radius = _background.sizeDelta / 2;
        _input = (eventData.position - position) / (radius * _canvas.scaleFactor);
        _sprint = _input.magnitude > 1;
        _input = _input.magnitude > _deadZone ? (_input.magnitude > 1 ? _input.normalized : _input) : Vector2.zero;
        _handle.anchoredPosition = _input * radius;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _active = false;
        _sprint = false;
        _input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
        _background.anchorMin = _defaultAnchoredPosition;
        _background.anchorMax = _defaultAnchoredPosition;
        _background.anchoredPosition = Vector2.zero;
    }
    private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out Vector2 localPoint))
        {           
            return localPoint;
        }
        return Vector2.zero;
    }
}