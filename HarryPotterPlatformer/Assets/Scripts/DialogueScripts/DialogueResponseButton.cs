using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueResponseButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _myTextComponent;
    [SerializeField] Button _myButton;
    System.Action _onResponseButtonClick;
    private void Awake()
    {
        _myButton.onClick.AddListener(OnResponseButtonClickHandler);
    }
    public void SetResponseText(string resp)
    {
        _myTextComponent.text = resp;
    }
    public void SetResponseButtonClickCallback( System.Action onResponseButtonClick)
    {
        _onResponseButtonClick = onResponseButtonClick;
    }
    private void OnResponseButtonClickHandler()
    {
        _onResponseButtonClick?.Invoke();
    }
}
