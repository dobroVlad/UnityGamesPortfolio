using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeSlotButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _myText;
    [SerializeField] private Button _myButton;
    [SerializeField] private Image _myPicture;
    private GrenadeType _type;
    private int _prevNumber;
    public int PrevNumber => _prevNumber;
    public GrenadeType Type => _type;

    public void SetGrenade(GrenadeType type, System.Action action)
    {
        var info = WeaponManager.Instance.GetGrenadeData(type);
        _myPicture.sprite = info.UiSlotPicture;
        _type = info.Type;
        _myButton.onClick.RemoveAllListeners();
        _myButton.onClick.AddListener(() => action());
    }
    public void SetNumber(int number)
    {
        _myText.text = $"<b><color=white><size=36>{number}</size></color></b>";
        _prevNumber = number;
    }
}
