using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extends the base class for editing the single uncovered elements. 
/// Provides functionality to changle the angle of a loop. This implementation is limited to stitches similar to Basic Stitch.
/// </summary>
public class KnotEditWithAngle : KnotEditBase
{
    /// <value> Slider changing the value of the angle. </value>
    public Slider angle;
    /// <value> Index of the point, which position will change. </value>
    public int indexOne;
    /// <value> Index of the second point, which position will change. </value>
    public int indexTwo;
    /// <value> Offset in the x-axis </value>
    public float xOffset;
    /// <value> Offset in the z-axis </value>
    public float zOffset;

    protected float prevAngle;
    /// <value> difference in x-axis added to the point position when slider changes </value>
    protected float xDiff;
    /// <value> difference in z-axis added to the point position when slider changes </value>
    protected float zDiff;
    /// <value> the start position of the first point </value>
    protected Vector3 firstPointStart;
    /// <value> the start position of the second point </value>
    protected Vector3 secondPointStart;

    /// <summary>
    /// Sets up the single element when scene starts.
    /// </summary>
    private void Start()
    {
        PrepareValues();
        prevAngle = angle.value;
        CalculateBaseValues();
        ChangeWidth();
    }

    /// <summary>
    /// Calculates the differencies in x and z axis.
    /// </summary>
    protected void CalculateBaseValues()
    {
        firstPointStart = splineComputer.GetPointPosition(indexOne);
        secondPointStart = splineComputer.GetPointPosition(indexTwo);
        xDiff = (firstPointStart.x - xOffset) / angle.maxValue;
        zDiff = (firstPointStart.z - zOffset) / angle.maxValue;
    }

    /// <summary>
    /// Called when values of sliders change.
    /// Besides calling OnEdit() from base class, it changes the angle of the loop.
    /// </summary>
    public void OnEditWithAngle()
    {
        if (angle != null && prevAngle != angle.value) ChangeAngle();
        OnEdit();
    }

    /// <summary>
    /// Changes the angle of the loop.
    /// The position of chosen two points is calculated using differencies in the axis and value of the slider.
    /// </summary>
    protected void ChangeAngle()
    {
        prevAngle = angle.value;
        splineComputer.SetPointPosition(indexOne, new Vector3(firstPointStart.x - prevAngle * xDiff, firstPointStart.y, firstPointStart.z - prevAngle * zDiff));
        splineComputer.SetPointPosition(indexTwo, new Vector3(secondPointStart.x + prevAngle * xDiff, secondPointStart.y, secondPointStart.z + prevAngle * zDiff));
    }

}
