using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider _thisSlider;
    [SerializeField] TextMeshProUGUI _healthNumText;
    public int Max => (int) _thisSlider.maxValue;
    public int Value => (int)_thisSlider.value;
    public void SetMaxValue(int num)
    {
        _thisSlider.maxValue = num;
        UpdateText();
    }
    public void UpdateHealthPoints(int num)
    {
        _thisSlider.value = num;
        UpdateText();
    }
    private void UpdateText()
    {
        _healthNumText.text = _thisSlider.value > 0 ? _thisSlider.value.ToString() : "K.O.";
    }
}
