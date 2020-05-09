using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class KnotEditWithAngle : KnotEditBase
{
    public Slider angle;
    public int indexOne;
    public int indexTwo;
    public float xOffset;
    public float zOffset;

    protected float prevAngle;
    protected float xDiff;
    protected float zDiff;
    protected Vector3 firstPointStart;
    protected Vector3 secondPointStart;

    private void Start()
    {
        PrepareValues();
        prevAngle = angle.value;
        CalculateBaseValues();
        ChangeWidth();
    }

    protected void CalculateBaseValues()
    {
        firstPointStart = splineComputer.GetPointPosition(indexOne);
        secondPointStart = splineComputer.GetPointPosition(indexTwo);
        xDiff = (firstPointStart.x - xOffset) / angle.maxValue;
        zDiff = (firstPointStart.z - zOffset) / angle.maxValue;
    }

    public void OnEditWithAngle()
    {
        if (angle != null && prevAngle != angle.value) ChangeAngle();
        OnEdit();
    }

    protected void ChangeAngle()
    {
        prevAngle = angle.value;
        splineComputer.SetPointPosition(indexOne, new Vector3(firstPointStart.x - prevAngle * xDiff, firstPointStart.y, firstPointStart.z - prevAngle * zDiff));
        splineComputer.SetPointPosition(indexTwo, new Vector3(secondPointStart.x + prevAngle * xDiff, secondPointStart.y, secondPointStart.z + prevAngle * zDiff));
    }

}
