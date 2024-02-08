using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PedalBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Sprite _idleSprite;
    [SerializeField]
    Sprite _pressedSprite;
    [SerializeField]
    Image _pedalImage;
    [SerializeField]
    PedalAction _pedalAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pedalImage.sprite = _pressedSprite;
        SetPedal(true);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _pedalImage.sprite = _idleSprite;
        SetPedal(false);
    }
    private void SetPedal(bool press)
    {
        switch (_pedalAction)
        {
            case PedalAction.Gas:
                PlayerMovement.SetGas(press); break;
            case PedalAction.Break:
                PlayerMovement.SetBreak(press); break;
            default: break;
        }
    }
}
public enum PedalAction
{
    Gas,
    Break
}
