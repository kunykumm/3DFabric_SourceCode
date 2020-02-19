using UnityEngine;
using UnityEngine.UI;

public class SizeChanger : MonoBehaviour
{
    //LeftSide (Knot)
    public Text heightText;
    public Text widthText;
    public Text lineWidthText;
    public Button addButton;
    public Button subButton;
    public Slider lineWidthSlider;

    //RigthSide (Knot)
    public Text netHeight;
    public Text netWidth;
    public Slider columnsSlider;
    public Slider rowsSlider;

    private float originalHeight;
    private float originalWidth;
    private float originalLineWidth;

    private float previousHeight;
    private float previousWidth;
    private float previousLineWidth;

    private bool allowUpdate = false;


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
        ChangeLineWidth(-change);
        ChangeSizesNet();
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
        allowUpdate = false;
        float newLineWidth = previousLineWidth + change * originalLineWidth / originalHeight;
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
        previousLineWidth = newLineWidth;
        lineWidthSlider.value = newLineWidth;
    }

    public void UpdateFromSlider(float newValue)
    {
        if (allowUpdate) setLineWidth(newValue);
        lineWidthText.text = newValue.ToString("0.00") + " cm";
    }

    public void OnClickLineWidthSlider()
    {
        allowUpdate = true;
    }

    public void ChangeSizesNet()
    {
        netHeight.text = rowsSlider.value.ToString() + " rows | " + (rowsSlider.value * previousHeight).ToString("0.00") + " cm";
        netWidth.text = columnsSlider.value.ToString() + " columns | " + (columnsSlider.value * previousWidth).ToString("0.00") + " cm";

        //netHeight.text = string.Format("{0} rows | {1,15} cm", rowsSlider.value, (rowsSlider.value * previousHeight).ToString("0.00"));
        //netWidth.text = string.Format("{0} columns | {1,15} cm", columnsSlider.value, (columnsSlider.value * previousWidth).ToString("0.00"));
    }
}
