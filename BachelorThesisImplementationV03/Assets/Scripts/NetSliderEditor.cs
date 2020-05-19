using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains functions that change text corresponding to a particular slider.
/// </summary>
public class NetSliderEditor : MonoBehaviour
{
    public Slider slider;
    public string baseText; 
    public Text text;

    /// <summary>
    /// When the head of a slider is dragged, it changes the corresponding text.
    /// </summary>
    void OnEnable()
    {
        slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(slider.value);
    }

    /// <summary>
    /// Removes set listeners from the slider when dragging stops.
    /// </summary>
    void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Function added as a listener to slider for onValueChanged SliderEvent.
    /// Changes the text of the given UI Text.
    /// </summary>
    /// <param name="value"> Number of columns / rows based on the slider value. </param>
    void ChangeValue(float value)
    {
        text.text = baseText + " " + value.ToString();
    }
}
