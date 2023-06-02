using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _myText;
    [SerializeField] private Button _myButton;
    [SerializeField] private Image _myPicture;
    private WeaponType _type;
    private int _prevMagazineBullets;
    private int _prevStorageBullets;
    public int PrevMagazineBullets => _prevMagazineBullets;
    public int PrevStorageBullets => _prevStorageBullets;
    public WeaponType Type => _type;

    public void SetWeapon(WeaponType type, System.Action action)
    {
        var info = WeaponManager.Instance.GetWeaponData(type);
        _myPicture.sprite = info.UiSlotPicture;
        _type = info.Type;
        _myButton.onClick.RemoveAllListeners();
        _myButton.onClick.AddListener(()=>action());
    }
    public void SetBulletsNum(int magazine, int storage)
    {
        _myText.text = $"<b><color=white><size=36>{magazine}</size></color></b>/{storage}";
        _prevMagazineBullets = magazine;
        _prevStorageBullets = storage;
    }
}
