using UnityEngine;
using UnityEngine.UI;

public class SizeChanger : MonoBehaviour
{
    public Text heightText;
    public Text widthText;
    public Text lineWidthText;
    public Button addButton;
    public Button subButton;
    public Slider lineWidthSlider;

    private float originalHeight;
    private float originalWidth;
    private float originalLineWidth;

    private float previousHeight;
    private float previousWidth;
    private float previousLineWidth;


    public void setHeight(float height)
    {
        previousHeight = height;
        originalHeight = height;
        heightText.text = originalHeight.ToString("0.00") + " cm";
    }

    public void setWidth(float width)
    {
        previousWidth = width;
        originalWidth = width;
        widthText.text = originalWidth.ToString("0.00") + " cm";
    }

    public void setLineWidth(float lineWidth)
    {
        originalLineWidth = lineWidth;
        previousLineWidth = lineWidth;
        lineWidthText.text = originalLineWidth.ToString("0.00") + " cm";
    }

    public void ChangeValues(float change)
    {
        ChangeHeight(change);
        ChangeWidth(change);
        ChangeLineWidth(change);
    }

    private void ChangeHeight(float change)
    {
        float newHeight = previousHeight + change;
        heightText.text = newHeight.ToString("0.00") + " cm";
        previousHeight = newHeight;
    }
    
    private void ChangeWidth(float change)
    {
        float newWidth = previousWidth + change * originalWidth / originalHeight;
        widthText.text = newWidth.ToString("0.00") + " cm";
        previousWidth = newWidth;
    }

    private void ChangeLineWidth(float change)
    {
        float newLineWidth = previousLineWidth + change * originalLineWidth / originalHeight;
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
        previousLineWidth = newLineWidth;
        lineWidthSlider.value = newLineWidth;
    }

    public void UpdateFromSlider(float newValue)
    {
        setLineWidth(newValue);
        lineWidthText.text = newValue.ToString("0.00") + " cm";
    }
}
