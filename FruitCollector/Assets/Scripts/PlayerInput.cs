using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour, IPointerDownHandler
{
    private Camera _camera;
    private bool _firstTouch;
    private void Start()
    {
        _camera = Camera.main;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_firstTouch)
        {
            EventAggregator.Post(this, new GameStartEvent());
            _firstTouch = true;
            AudioManager.Instance.PlaySfx(SfxType.Click);
        }

        var hits = Physics.RaycastAll(_camera.ScreenPointToRay(eventData.position));
        foreach ( var hit in hits )
        {
            if(hit.collider.gameObject.TryGetComponent<FoodUnit>(out var food) && !food.Taken)
            {
                EventAggregator.Post(this, new TakeFoodEvent(food));
            }
        }
    }

}
