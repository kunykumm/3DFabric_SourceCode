using System.Collections;
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

    //CameraChange
    public GameObject cameraNetFocus;
    public Camera cameraNet;
    public int horizontalOffset;

    private float originalHeight;
    private float originalWidth;
    private float originalLineWidth;

    private float previousHeight;
    private float previousWidth;
    private float previousLineWidth;

    private float editorNetHeight = 0;
    private float editorNetWidth = 0;
    private float heightOffset;
    private float widthOffset;
    private float alternation;

    private float changer = 0.0f;
    private float currentScale = 1.0f;
    private float baseLineWidthRatio = 1.0f;
    private bool allowUpdate = false;
    private float zChange;
    private CameraMovement cameraMovement;


    private void Start()
    {
        zChange = cameraNet.transform.position.z;
        cameraMovement = cameraNet.GetComponent<CameraMovement>();
    }

    public void SetHeight(float height)
    {
        previousHeight = height;
        originalHeight = height;
        heightText.text = originalHeight.ToString("0.00") + " cm";
    }

    public void SetWidth(float width)
    {
        previousWidth = width;
        originalWidth = width;
        widthText.text = originalWidth.ToString("0.00") + " cm";
    }

    public void SetLineWidth(float lineWidth)
    {
        originalLineWidth = lineWidth;
        previousLineWidth = lineWidth;
        lineWidthText.text = originalLineWidth.ToString("0.00") + " cm";
    }

    public void SetOffsets(float heightOff, float widthOff = 0, int alt = 1)
    {
        heightOffset = heightOff;
        widthOffset = widthOff;
        alternation = alt;
    }

    public void ChangeValues(float newChange)
    {
        changer += newChange;
        currentScale = 1 + changer;
        ChangeHeight();
        ChangeWidth();
        ChangeLineWidth();    //-change
        ChangeSizesNet();
    }

    private void ChangeHeight()
    {
        float newHeight = previousHeight * currentScale;
        heightText.text = newHeight.ToString("0.00") + " cm";
    }

    private void ChangeWidth()
    {
        float newWidth = previousWidth * currentScale;
        widthText.text = newWidth.ToString("0.00") + " cm";
    }

    private void ChangeLineWidth()
    {
        Debug.Log("LineWidthRatio: " + baseLineWidthRatio);
        Debug.Log("CurrentScale: " + currentScale);
        allowUpdate = false;
        float newLineWidth = previousLineWidth * currentScale * baseLineWidthRatio;
        Debug.Log("NewLineWidth: " + newLineWidth);
        Debug.Log("TextValue: " + (previousLineWidth * baseLineWidthRatio * 4 / 5 * (currentScale * currentScale)));
        lineWidthText.text = (previousLineWidth * baseLineWidthRatio * 4/5 * (currentScale * currentScale)).ToString("0.00") + " cm";
        lineWidthSlider.value = newLineWidth;
    }

    public void UpdateFromSlider(float newValue)
    {
        if (allowUpdate)
        {
            baseLineWidthRatio = newValue / previousLineWidth;
            lineWidthText.text = (newValue * 4/5 * (currentScale * currentScale)).ToString("0.00") + " cm";
        }
    }

    public void OnClickLineWidthSlider()
    {
        allowUpdate = true;
    }

    public float LineWidthRatio()
    {
        return baseLineWidthRatio;
    }

    public void ChangeSizesNet()
    {
        var newHeight = rowsSlider.value * originalHeight - (rowsSlider.value - 1) * heightOffset;
        var newWidth = columnsSlider.value * alternation * (originalWidth - widthOffset);

        if (newHeight != editorNetHeight || newWidth != editorNetWidth) ChangeNetCameraFocus(newHeight, newWidth);

        netHeight.text = rowsSlider.value.ToString() + " rows | " + (rowsSlider.value * previousHeight - (rowsSlider.value - 1) * heightOffset).ToString("0.00") + " cm";
        netWidth.text = columnsSlider.value.ToString() + " columns | " + (columnsSlider.value * previousWidth - (columnsSlider.value - 1) * widthOffset).ToString("0.00") + " cm";
    }

    private void ChangeNetCameraFocus(float newHeight, float newWidth)
    {
        float customWidth = newWidth / 2;
        if (horizontalOffset == 1) customWidth += ((originalWidth - widthOffset) / 4);
        if (newHeight > newWidth)
        {
            zChange = - newHeight * 1.7f;
            if (editorNetHeight > newHeight) zChange *= (-1);
            if (editorNetWidth > newHeight) zChange *= 0;

        } else
        {
            zChange = - newWidth * 1.7f;
            if (editorNetWidth > newWidth) zChange *= (-1);
            if (editorNetHeight > newWidth) zChange *= 0;
        }

        cameraNetFocus.transform.position = new Vector3(customWidth, -newHeight / 2 + originalHeight, cameraNetFocus.transform.position.z);

        if (zChange != 0)
        {
            if (cameraMovement == null) cameraMovement = cameraNet.GetComponent<CameraMovement>();    //in case generateMesh started sooner
            var wantedVector = new Vector3(cameraNet.transform.position.x, cameraNet.transform.position.y, zChange);
            var newDistance = Vector3.Distance(wantedVector, cameraNetFocus.transform.position);
            cameraMovement.SetNewDistance(newDistance);
        }

        editorNetHeight = newHeight;
        editorNetWidth = newWidth;
    }
}
