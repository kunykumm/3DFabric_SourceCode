using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains functions for setting all essential attributes for generating nets using DreamteckSplines.
/// </summary>
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

    /// <summary>
    /// Sets up all protected parameters.
    /// </summary>
    /// <param name="parent"> Object in the scene (called RuntimeRows) that is a parent to all generated rows of elements. </param>
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

    /// <summary>
    /// Prefab for the net elements (knotClone) is updated with changed values of the base element (knotPrefab).
    /// </summary>
    protected void UpdateKnot()
    {
        var points = knotPrefab.GetComponent<SplineComputer>().GetPoints();
        knotClone.GetComponent<SplineComputer>().SetPoints(points);
        knotClone.GetComponent<TubeGenerator>().sides = knotPrefab.GetComponent<TubeGenerator>().sides;
        if (knotPrefab.GetComponent<SplineComputer>().isClosed) knotClone.GetComponent<SplineComputer>().Close();
    }

    /// <summary>
    /// Initialises the KnotUtility.
    /// Calculates the height and width based on the coordinates of points in the spline, not the bounds of the mesh
    /// (the fibre diameter is not calculated in this size)..
    /// </summary>
    protected void SetupKnotUtility()
    {
        knotUti = new KnotUtility();
        knotUti.FindMaxsMins(ref height, ref width, basePoints);
    }
}
