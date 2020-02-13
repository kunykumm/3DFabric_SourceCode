using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class KnotEditWithAngle : KnotEditBase
{
    public Slider angle;

    protected float prevAngle;

    protected float xDiff;
    protected float zDiff;
    protected Vector3 firstPointStart;
    protected Vector3 secondPointStart;

    /**
    * ANGLES (BasicKnot)
    * Start position:   3.0  3.0  0.8
    *                   0.0  3.0 -0.8
    * End position:     1.5  3.0  1.6
    *                   1.5  3.0 -1.6
    *                   
    * ALL NORMALS: At average center - best result when angle changes
    */

    private void Start()
    {
        PlayerPrefs.SetString("scene", sceneName);
        PrepareValues();
        prevAngle = angle.value;
        CalculateBaseValues();
        ChangeWidth();
    }

    protected void CalculateBaseValues()
    {
        firstPointStart = new Vector3(2.6f, 3.0f, 0.8f);
        secondPointStart = new Vector3(0.4f, 3.0f, -0.8f);
        xDiff = (firstPointStart.x - 1.5f) / angle.maxValue;
        zDiff = (firstPointStart.z - 1.6f) / angle.maxValue;
    }

    public void OnEditWithAngle()
    {
        if (angle != null && prevAngle != angle.value) ChangeAngle();
        OnEdit();
    }

    protected void ChangeAngle()
    {
        prevAngle = angle.value;
        splineComputer.SetPointPosition(1, new Vector3(firstPointStart.x - prevAngle * xDiff, firstPointStart.y, firstPointStart.z - prevAngle * zDiff));
        splineComputer.SetPointPosition(3, new Vector3(secondPointStart.x + prevAngle * xDiff, secondPointStart.y, secondPointStart.z + prevAngle * zDiff));
    }

}
