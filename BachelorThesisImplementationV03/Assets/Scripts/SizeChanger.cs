using Dreamteck.Splines;
using System;
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
    public bool continuousLine;

    //CameraChange
    public GameObject cameraNetFocus;
    public Camera cameraNet;
    public int horizontalOffset;

    //Left Side Info
    private float previousHeight;
    private float previousWidth;
    private float previousLineWidth;
    private float triangleCount;
    private float coveredDefaultLineWidth;

    private float changer = 0.0f;
    private float currentScale = 1.0f;
    private float previousScale = 1.0f;
    private float newLineWidth = 0.5f;
    private bool allowUpdate = false;

    //Right Side Info
    private float editorNetHeight = 0;
    private float editorNetWidth = 0;
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
        heightText.text = previousHeight.ToString("0.00") + " cm";
    }

    public void SetWidth(float width)
    {
        previousWidth = width;
        widthText.text = previousWidth.ToString("0.00") + " cm";
    }

    public void SetLineWidth(float lineWidth)
    {
        previousLineWidth = lineWidth;
        lineWidthText.text = previousLineWidth.ToString("0.00") + " cm";
    }

    public void SetCoveredDefaultLineWidth(float coveredeLineWidth)
    {
        coveredDefaultLineWidth = coveredeLineWidth;
    }

    public float GetCoveredDefaultLineWidth()
    {
        return coveredDefaultLineWidth;
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
        if (isCovered) result = newLineWidth >= coveredDefaultLineWidth && newLineWidth <= 1;
        else result = newLineWidth >= 0.2 && newLineWidth <= 1;

        if (!result)
        {
            if (isCovered)
            {
                if (newLineWidth >= 1) return 1f;
                return coveredDefaultLineWidth;
            }
            return (float)Math.Round(newLineWidth, 2);
        }
        return 0;
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
        SetWidth(previousWidth);
        SetHeight(previousHeight);
    }

    public float GetCurrentScale()
    {
        return currentScale * previousScale;
    }

    public void ChangeSizesNet()
    {
        Bounds bounds = GetBoundsSize();

        if (bounds.size.y != editorNetHeight || bounds.size.x != editorNetWidth) ChangeNetCameraFocus(bounds.size.y, bounds.size.x);

        netHeight.text = (bounds.size.y * GetCurrentScale()).ToString("0.00") + " cm";
        netWidth.text = (bounds.size.x * GetCurrentScale()).ToString("0.00") + " cm";

        CalculateTrianglesCount();
    }

    private Bounds GetBoundsSize()
    {
        Bounds bounds = theMesh.GetComponent<Renderer>().bounds;
        foreach (Transform child in theMesh.transform)
        {
            if (child.childCount > 0)
            {
                foreach (Transform grandChild in child.transform)
                {
                    bounds.Encapsulate(grandChild.GetComponent<Renderer>().bounds);
                }
            }
            bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
        }
        return bounds;
    }

    private void CalculateTrianglesCount()
    {
        if (isCovered)
        {
            netTrianglesCount.text = (rowsSlider.value * columnsSlider.value * triangleCount).ToString();
            return;
        }
        if (continuousLine) netTrianglesCount.text = (rowsSlider.value * tubeGeneratorNet.GetTriangleCount()).ToString();
        else netTrianglesCount.text = (rowsSlider.value * columnsSlider.value * triangleCount).ToString();
    }

    private void ChangeNetCameraFocus(float newHeight, float newWidth)
    {
        CalculateZChange(newHeight, newWidth);
        Vector3 centre = GetBoundsCenter();

        cameraNetFocus.transform.position = new Vector3(centre.x, centre.y, cameraNetFocus.transform.position.z);

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

    private void CalculateZChange(float newHeight, float newWidth)
    {
        if (newHeight > newWidth)
        {
            zChange = -newHeight * 1.7f;
            if (editorNetHeight > newHeight) zChange *= (-1);
            if (newWidth > newHeight) zChange *= 0;

        }
        else
        {
            zChange = -newWidth * 1.7f;
            if (editorNetWidth > newWidth) zChange *= (-1);
            if (newHeight > newWidth) zChange *= 0;
        }
    }

    private Vector3 GetBoundsCenter()
    {
        Vector3 centre = new Vector3();
        int counter = 0;

        foreach (Transform child in theMesh.transform)
        {
            if (child.childCount > 0)
            {
                foreach (Transform grandChild in child.transform)
                {
                    centre += grandChild.GetComponent<Renderer>().bounds.center;
                    counter++;
                }
            } else
            {
                centre += child.GetComponent<Renderer>().bounds.center;
                counter++;
            }
        }
        return (centre / counter);
    }

    public void UpdateTriangleCount(int triangleCount)
    {
        this.triangleCount = triangleCount;
        triangleCountText.text = triangleCount.ToString();
    }
}
