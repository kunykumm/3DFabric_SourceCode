using Dreamteck.Splines;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SizeChanger : MonoBehaviour
{
    public GameObject theMesh;

    //LeftSide (Knot)
    public Text heightText;
    public Text widthText;
    public Text lineWidthText;
    public Text triangleCountText;
    public Button addButton;
    public Button subButton;
    public Slider lineWidthSlider;
    public TubeGenerator tubeGeneratorNet;
    public bool isCovered;

    //RigthSide (Knot)
    public Text netHeight;
    public Text netWidth;
    public Text netTrianglesCount;
    public Slider columnsSlider;
    public Slider rowsSlider;
    public int halfKnotAtEnd = 0; // pripad basic knot
    public int continuousLine = 0; // pripad vsetkych, ktore su napojene v jednej linii
    public int alternation = 1; // clover knot, vynechavanie uzlov, napr. kazdy druhy v riadku

    //CameraChange
    public GameObject cameraNetFocus;
    public Camera cameraNet;
    public int horizontalOffset;

    //Left Side Info
    private float previousHeight;
    private float previousWidth;
    private float previousLineWidth;
    private float triangleCount;

    private float changer = 0.0f;
    private float currentScale = 1.0f;
    private float previousScale = 1.0f;
    private float newLineWidth = 0.5f;
    private bool allowUpdate = false;

    //Right Side Info
    private float editorNetHeight = 0;
    private float editorNetWidth = 0;
    private float heightOffset = 0; //posunutie vo vertikalnej osi
    public float widthOffset = 0; // posunutie v horizontálnej osi
    private float zChange;

    private CameraMovement cameraMovement;


    private void Start()
    {
        zChange = cameraNet.transform.position.z;
        cameraMovement = cameraNet.GetComponent<CameraMovement>();
    }

    public void SetHeight(float height, float lineWidth = 0.5f)
    {
        previousHeight = height;
        heightText.text = (previousHeight + lineWidth).ToString("0.00") + " cm";
    }

    public void SetWidth(float width, float lineWidth = 0.5f)
    {
        previousWidth = width;
        widthText.text = (previousWidth + lineWidth).ToString("0.00") + " cm";
    }

    public void SetLineWidth(float lineWidth)
    {
        previousLineWidth = lineWidth;
        lineWidthText.text = previousLineWidth.ToString("0.00") + " cm";
    }

    public void SetOffsets(float heightOff, float widthOff = 0)
    {
        heightOffset = heightOff;
        widthOffset = widthOff;
    }

    public float GetScale()
    {
        return previousScale;
    }

    public void ChangeValues(float newChange)
    {
        changer += newChange;
        currentScale = 1 + changer;
        if (isCovered) ChangeLineWidthCovered();
        else ChangeLineWidth();
        ChangeHeight();
        ChangeWidth();
        ChangeSizesNet();
    }

    public float AllowChangeValues()
    {
        bool result;
        if (isCovered) result = newLineWidth >= 0.12;
        else result = newLineWidth >= 0.2 && newLineWidth <= 1;

        if (!result) return (float)Math.Round(newLineWidth, 2);
        return 0;
    }

    private void ChangeHeight()
    {
        float newHeight = previousHeight * currentScale;
        heightText.text = (newHeight + (isCovered ? 0 : newLineWidth)).ToString("0.00") + " cm";
    }

    private void ChangeWidth()
    {
        float newWidth = previousWidth * currentScale;
        widthText.text = (newWidth + (isCovered ? 0 : newLineWidth)).ToString("0.00") + " cm";
    }

    private void ChangeLineWidth()
    {
        allowUpdate = false;
        newLineWidth = previousLineWidth * currentScale * currentScale; 
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
        lineWidthSlider.value = newLineWidth;
    }

    private void ChangeLineWidthCovered()
    {
        newLineWidth = previousLineWidth * currentScale * currentScale;
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
    }

    public void UpdateFromSlider(float newValue)
    {
        if (allowUpdate)
        { 
            lineWidthText.text = newValue.ToString("0.00") + " cm";
        }
    }

    public void OnClickLineWidthSlider()
    {
        allowUpdate = true;
    }

    public void UpdateAllValues()
    {
        previousLineWidth = lineWidthSlider.value;
        previousWidth *= currentScale;
        previousHeight *= currentScale;
        previousScale *= currentScale;
        currentScale = 1.0f;
        changer = 0.0f;
        SetWidth(previousWidth, previousLineWidth);
        SetHeight(previousHeight, previousLineWidth);
    }

    public float GetCurrentScale()
    {
        return currentScale * previousScale;
    }

    public void ChangeSizesNet()
    {
        float realHeight = previousHeight;
        float realWidth = previousWidth;
        float savedLineWidth = previousLineWidth;
        if (isCovered) previousLineWidth = 0;

        CalculateDimensions(ref realHeight, rowsSlider.value, 1, previousHeight, false);
        CalculateDimensions(ref realWidth, columnsSlider.value, alternation, previousWidth, true);

        Bounds bounds = theMesh.GetComponent<Renderer>().bounds;
        foreach (Transform child in theMesh.transform)
        {
            if(child.childCount > 0)
            {
                foreach(Transform grandChild in child.transform)
                {
                    bounds.Encapsulate(grandChild.GetComponent<Renderer>().bounds);
                }
            }
            bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
        }

        if (bounds.size.y != editorNetHeight || bounds.size.x != editorNetWidth) ChangeNetCameraFocus(bounds.size.y, bounds.size.x);

        netHeight.text = "Editor: " + (bounds.size.y * currentScale).ToString("0.00") + " cm | Real: " + (realHeight * currentScale).ToString("0.00") + " cm";
        netWidth.text = "Editor: " + (bounds.size.x * currentScale).ToString("0.00") + " cm | Real: " + (realWidth * currentScale).ToString("0.00") + " cm";

        previousLineWidth = savedLineWidth;

        CalculateTrianglesCount();
    }

    private void CalculateDimensions(ref float dimension, float sliderValue, int alter, float prevDimension, bool isWidth)
    {
        dimension = 0;
        float lineWidth = previousLineWidth;
        float newPrevDim = prevDimension;

        if (isWidth)
        {
            if (halfKnotAtEnd == 1) dimension += ((prevDimension + lineWidth) / 2);
            if (continuousLine == 1)
            {
                newPrevDim = prevDimension + lineWidth;
                lineWidth = 0;
            }
            if (alternation > 1) dimension += (2 * (prevDimension - 3 * lineWidth));
        }

        if (sliderValue == 2) dimension += (alter * sliderValue * (newPrevDim - lineWidth));
        if (sliderValue > 2) dimension += (alter * (sliderValue - 2) * (newPrevDim - 3 * lineWidth) +
                2 * (newPrevDim - lineWidth));
        if (isCovered) dimension += ((sliderValue - 1) * widthOffset);
    }

    private void CalculateTrianglesCount()
    {
        if (isCovered)
        {
            netTrianglesCount.text = (rowsSlider.value * columnsSlider.value * triangleCount).ToString();
            return;
        }
        if (continuousLine == 1) netTrianglesCount.text = (rowsSlider.value * tubeGeneratorNet.GetTriangleCount()).ToString();
        else netTrianglesCount.text = (rowsSlider.value * columnsSlider.value * triangleCount).ToString();
    }

    private void ChangeNetCameraFocus(float newHeight, float newWidth)
    {
        float customWidth = newWidth / 2;
        if (horizontalOffset == 1) customWidth += ((previousWidth - widthOffset) / 4);
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

        cameraNetFocus.transform.position = new Vector3(customWidth, -newHeight / 2 + previousHeight, cameraNetFocus.transform.position.z);

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

    public void UpdateTriangleCount(int triangleCount)
    {
        this.triangleCount = triangleCount;
        triangleCountText.text = triangleCount.ToString();
    }
}
