using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetSliderEditor : MonoBehaviour
{
    public Slider slider;
    public string baseText; 
    public Text text;

    void OnEnable()
    {
        slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(slider.value);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    void ChangeValue(float value)
    {
        text.text = baseText + " " + value.ToString();
    }
}
