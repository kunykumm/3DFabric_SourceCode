using UnityEngine;
using UnityEngine.UI;

public class SizeChanger : MonoBehaviour
{
    public Text heightText;
    public Text widthText;
    public Button addButton;
    public Button subButton;

    private float originalHeight;
    private float originalWidth;
    private float previousHeight;
    private float previousWidth;


    public void setHeight(float height)
    {
        previousHeight = height;
        originalHeight = height;
        heightText.text = originalHeight.ToString();
    }

    public void setWidth(float width)
    {
        previousWidth = width;
        originalWidth = width;
        widthText.text = originalWidth.ToString();
    }

    public void ChangeValues(float change)
    {
        float newHeight = previousHeight + change;
        float newWidth = previousWidth + change * originalWidth / originalHeight;

        heightText.text = newHeight.ToString("0.00") + " cm";
        widthText.text = newWidth.ToString("0.00") + " cm";

        previousHeight = newHeight;
        previousWidth = newWidth;
    }

    private void ChangeLineWidth()
    {

    }
}
