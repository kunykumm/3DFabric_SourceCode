using Dreamteck.Splines;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides all functionality for calculating the size changes of single elements and whole nets.
/// Because of the size changes, it also contains functions that update camera position (to fit the whole net in the viewport).
/// This class calculates the crucial 'current scale' needed for exporting the final mesh in the right size.
/// </summary>
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

    //RigthSide (Net)
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
    private CameraMovement cameraMovement;

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


    /// <summary>
    /// Sets up private attributes for camera changes.
    /// </summary>
    private void Start()
    {
        zChange = cameraNet.transform.position.z;
        cameraMovement = cameraNet.GetComponent<CameraMovement>();
    }

    /// <summary>
    /// Setter used by KnotEditBase and CoveredKnot classes during the start phase of the scene to save the height of the base element.
    /// </summary>
    /// <param name="height"> Base height of the single element. </param>
    public void SetHeight(float height)
    {
        previousHeight = height;
        heightText.text = previousHeight.ToString("0.00") + " cm";
    }

    /// <summary>
    /// Setter used by KnotEditBase and CoveredKnot classes during the start phase of the scene to save the width of the base element.
    /// </summary>
    /// <param name="width"> Base width of the single element. </param>
    public void SetWidth(float width)
    {
        previousWidth = width;
        widthText.text = previousWidth.ToString("0.00") + " cm";
    }

    /// <summary>
    /// Setter used by KnotEditBase and CoveredKnot classes during the start phase of the scene to save the lineWidth of the base element.
    /// </summary>
    /// <param name="lineWidth"> Base line width (fibre diameter) of the single element. </param>
    public void SetLineWidth(float lineWidth)
    {
        previousLineWidth = lineWidth;
        lineWidthText.text = previousLineWidth.ToString("0.00") + " cm";
    }

    /// <summary>
    /// Additional setter used by CoveredKnot class during the start phase of the scene to save the lineWidth of the base element.
    /// </summary>
    /// <param name="coveredeLineWidth"> Base line width (fibre diameter) of the single covered element. </param>
    public void SetCoveredDefaultLineWidth(float coveredeLineWidth)
    {
        coveredDefaultLineWidth = coveredeLineWidth;
    }

    /// <summary>
    /// Getter for the default line width of a covered element.
    /// </summary>
    /// <returns> Default line width of a covered element. </returns>
    public float GetCoveredDefaultLineWidth()
    {
        return coveredDefaultLineWidth;
    }

    /// <summary>
    /// Function called in OnPointerDown function in ButtonPressed class when + or - buttons are being pressed and change is allowed.
    /// </summary>
    /// <param name="newChange"> Scalar added to the changer, increasing or decreasing the current scale. </param>
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

    /// <summary>
    /// Checks the restrictions for fibre diameter in the elements. 
    /// </summary>
    /// <returns>   0 if the fibre diameter is within the accepted interval,
    ///             a float number rounded to 2 decimal places when the value is out of bounds 
    ///             (these values are needed for setting the interactibility of buttons in ButtonPressed class)
    /// </returns>
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

    /// <summary>
    /// Changes the text informing about the height of a single element.
    /// </summary>
    private void ChangeHeight()
    {
        float newHeight = previousHeight * currentScale;
        heightText.text = newHeight.ToString("0.00") + " cm";
    }

    /// <summary>
    /// Changes the text informing about the width of a single element.
    /// </summary>
    private void ChangeWidth()
    {
        float newWidth = previousWidth * currentScale;
        widthText.text = newWidth.ToString("0.00") + " cm";
    }

    /// <summary>
    /// Changes the text informing about the line width / fibre diameter of a single element.
    /// AllowUpdate is set to false to prevent function UpdateFromSlider from being executed 
    /// (the text is not rewritten to wrong value).
    /// </summary>
    private void ChangeLineWidth()
    {
        allowUpdate = false;
        newLineWidth = previousLineWidth * currentScale * currentScale; 
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
        lineWidthSlider.value = newLineWidth;
    }

    /// <summary>
    /// Custom function for changing the text informing about the line width / fibre diameter of a single covered element.
    /// In this case is no slider updated.
    /// </summary>
    private void ChangeLineWidthCovered()
    {
        newLineWidth = previousLineWidth * currentScale * currentScale;
        lineWidthText.text = newLineWidth.ToString("0.00") + " cm";
    }

    /// <summary>
    /// In case the fibre diameter is changed with slider, the text informing about the line width changes in this function.
    /// </summary>
    /// <param name="newValue"></param>
    public void UpdateFromSlider(float newValue)
    {
        if (allowUpdate)
        { 
            lineWidthText.text = newValue.ToString("0.00") + " cm";
        }
    }

    /// <summary>
    /// When the head of the slider is clicked, the allowUpdate function is set to true.
    /// This means, the above function UpdateFromSlider is called just in case the fibre diameter is changed by slider.
    /// </summary>
    public void OnClickLineWidthSlider()
    {
        allowUpdate = true;
    }

    /// <summary>
    /// Executed when the dragging of the line width slider is finished.
    /// CurrentScale changes sizes of all provided attributes. Current scale is then set to 1.
    /// </summary>
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

    /// <summary>
    /// Getter for current scale. Used when exporting the mesh.
    /// </summary>
    /// <returns> Current scale multiplied by previous scales, resulting in overall scale. </returns>
    public float GetCurrentScale()
    {
        return currentScale * previousScale;
    }

    /// <summary>
    /// Changes the texts informing about the actual net size and triangle count of the whole mesh.
    /// </summary>
    public void ChangeSizesNet()
    {
        Bounds bounds = GetBoundsSize();

        if (bounds.size.y != editorNetHeight || bounds.size.x != editorNetWidth) ChangeNetCameraFocus(bounds.size.y, bounds.size.x);

        netHeight.text = (bounds.size.y * GetCurrentScale()).ToString("0.00") + " cm";
        netWidth.text = (bounds.size.x * GetCurrentScale()).ToString("0.00") + " cm";

        CalculateTrianglesCount();
    }

    /// <summary>
    /// Finds the bounds of the mesh. Needed for calculating width and height of the mesh.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Calculates the overall triangle count of the generated mesh in the scene.
    /// </summary>
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

    /// <summary>
    /// Changes the position of the camera (to zoom out / zoom in on the mesh).
    /// </summary>
    /// <param name="newHeight"> New height of the net calculates from bounds of the mesh. </param>
    /// <param name="newWidth"> New width of the net calculates from bounds of the mesh. </param>
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

    /// <summary>
    /// Calculates the z coordinate of the camera from new height and width of the net.
    /// </summary>
    /// <param name="newHeight"> New height of the net calculates from bounds of the mesh. </param>
    /// <param name="newWidth"> New width of the net calculates from bounds of the mesh. </param>
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

    /// <summary>
    /// Calculates the centre of the mesh to precisely set the x and y coordinates of the camera.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Changes the text informing about overall triangle count.
    /// </summary>
    /// <param name="triangleCount"> Calculated triangle count of the generated net. </param>
    public void UpdateTriangleCount(int triangleCount)
    {
        this.triangleCount = triangleCount;
        triangleCountText.text = triangleCount.ToString();
    }
}
