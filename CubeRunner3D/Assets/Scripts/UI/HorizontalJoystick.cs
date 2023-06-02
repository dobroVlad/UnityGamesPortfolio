using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] [Range(0f,0.1f)] private float _deadZone;
    [SerializeField]
    [Range(0.1f, 0.5f)] private float _screenRadius;
    private float _radiusInPixels;
    Vector2 _startPosition;
    private Vector2 _input;
    private bool _firstTouch;

    private void Start()
    {
        _startPosition = Vector2.zero;
        _radiusInPixels = Screen.width *_screenRadius;
        _input = Vector2.zero;
        _firstTouch = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _startPosition = eventData.position;
        if (!_firstTouch) 
        {
            EventAggregator.Post(this, new GameStartEvent());
            _firstTouch = true; 
        }
        OnDrag(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        _input = (eventData.position - _startPosition) / _radiusInPixels;
        _input = _input.magnitude > _deadZone ? _input : Vector2.zero;
        _startPosition = eventData.position;
        EventAggregator.Post(this, new JoystickInputEvent(_input.x));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _input = Vector2.zero;
        _startPosition = Vector2.zero;
    }
}
