using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBase : MonoBehaviour
{
    public Slider columns;
    public Slider rows;
    public SizeChanger sizeChanger;
    public GameObject runtimeRows;
    public GameObject knotPrefab;
    public float heightOffset;

    protected float point_size;

    protected SplinePoint[] basePoints;
    protected KnotUtility knotUti;
    protected GameObject knotClone;
    protected SplineComputer splineComputer;
    protected float width;
    protected float height;
    protected int currentPointCount;
    protected int basePointCount;
    protected int prevColumns = 1;
    protected int prevRows = 1;
    protected bool updateValues;

    protected void SetupNet(Transform parent)
    {
        knotClone = GameObject.Find("KnotForNet");
        UpdateKnot();

        knotClone.transform.parent = parent;
        knotClone.tag = "knotrow";
        knotClone.layer = 9;

        splineComputer = knotClone.GetComponent<SplineComputer>();
        basePoints = splineComputer.GetPoints();
        basePointCount = basePoints.Length;
        currentPointCount = basePointCount;
        point_size = splineComputer.GetPointSize(0);
        updateValues = false;
    }

    protected void UpdateKnot()
    {
        var points = knotPrefab.GetComponent<SplineComputer>().GetPoints();
        knotClone.GetComponent<SplineComputer>().SetPoints(points);
        knotClone.GetComponent<TubeGenerator>().sides = knotPrefab.GetComponent<TubeGenerator>().sides;
        if (knotPrefab.GetComponent<SplineComputer>().isClosed) knotClone.GetComponent<SplineComputer>().Close();
    }

    protected void SetupKnotUtility()
    {
        knotUti = new KnotUtility();
        knotUti.FindMaxsMins(ref height, ref width, basePoints);
    }
}
