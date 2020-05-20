using Dreamteck.Splines;
using UnityEngine;

/// <summary>
/// Extends functionality of GenerateBase class.
/// Customised for type: Weft-knitting imitations.
/// Further comments: Suitable for complex continuous lines, where rotation or position change is needed.
/// Scenes: Hook Knot, Edged Knot.
/// </summary>
public class GenerateMeshTwoKnots : GenerateMesh
{
    public GameObject knotPrefabRotated;
    public bool allowForwardOffset;
    public float forwardOffset;

    private GameObject knotCloneRotated;
    private SplineComputer splineComputerRotated;

    /// <summary>
    /// Sets up the default net at the start of the scene.
    /// </summary>
    void Start()
    {
        SetupNet(runtimeRows.transform);
        SetupComplicatedNet();
        SetupKnotUtility();

        ChangeColumnsComplicated();
        ChangeRowsComplicated();

        sizeChanger.ChangeSizesNet();
    }

    /// <summary>
    /// In case the value of column and row sliders change, the functions to change the net are called.
    /// </summary>
    void Update()
    {
        if (updateValues)
        {
            sizeChanger.ChangeSizesNet();
            updateValues = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (prevColumns != (int)columns.value) ChangeColumnsComplicated();
            if (prevRows != (int)rows.value) ChangeRowsComplicated();
            updateValues = true;
        }
    }

    /// <summary>
    /// Updates the knotCloneRotated according to the changes in knotPrefab.
    /// </summary>
    protected void SetupComplicatedNet()
    {
        knotCloneRotated = GameObject.Find("KnotForNetRotated");
        var points = knotPrefabRotated.GetComponent<SplineComputer>().GetPoints();
        knotCloneRotated.GetComponent<SplineComputer>().SetPoints(points);
        knotCloneRotated.GetComponent<TubeGenerator>().sides = knotPrefabRotated.GetComponent<TubeGenerator>().sides;
        knotCloneRotated.layer = 10;
        splineComputerRotated = knotCloneRotated.GetComponent<SplineComputer>();
    }

    /// <summary>
    /// When EditNet button is clicked, this function is called to update all attributes according to knotPrefab.
    /// In this case, knotClone and knotCloneRotated are both updated. 
    /// </summary>
    public override void UpdateNet()
    {
        DeleteRows(prevRows - 1);
        knotClone = runtimeRows.transform.GetChild(0).gameObject;
        knotClone.transform.parent = null;

        SetupNet(runtimeRows.transform);
        SetupComplicatedNet();

        prevColumns = 1;
        prevRows = 1;

        ChangeColumnsComplicated();
        ChangeRowsComplicated();

        sizeChanger.ChangeSizesNet();
    }

    /// <summary>
    /// Decides how to change columns of the net (increase or decrease the count).
    /// </summary>
    private void ChangeColumnsComplicated()
    {
        int diff = (int)columns.value - prevColumns;
        if (diff > 0)
        {
            int newPoints = diff * (basePointCount - 1);
            AddColumns(newPoints, splineComputer);
            currentPointCount -= newPoints;
            AddColumns(newPoints, splineComputerRotated);
        }
        else
        {
            int newCount = currentPointCount + diff * (basePointCount - 1);
            DeleteColumns(newCount, splineComputer);
            DeleteColumns(newCount, splineComputerRotated);
        }
        prevColumns = (int)columns.value;
    }

    /// <summary>
    /// Decides how to change rows of the net (increase or decrease the count).
    /// </summary>
    private void ChangeRowsComplicated()
    {
        int diff = (int)rows.value - prevRows;
        if (diff > 0)
        {
            AddRowsComplicated(diff);
        }
        else
        {
            DeleteRows(-diff);
        }
        prevRows = (int)rows.value;
    }

    /// <summary>
    /// Adds new rows.
    /// The selection of knotClone / knotCloneRotated depends on the index of the row.
    /// If the index is even, the knotClone is chosen. Otherwise, the knotCloneRotated is chosen.
    /// </summary>
    /// <param name="diff"> Number of new rows to add. </param>
    private void AddRowsComplicated(int diff)
    {
        float curHeight = -(height - heightOffset);

        GameObject newKnot;

        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;
        knotCloneRotated.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

        for (int i = 0; i < diff; ++i)
        {
            if ((i + prevRows) % 2 == 1)
            {
                newKnot = Instantiate(knotCloneRotated, knotCloneRotated.transform.position, Quaternion.identity);
                if (allowForwardOffset) newKnot.transform.position += transform.forward * forwardOffset;
                newKnot.layer = 9;
            } 
            else
            {
                newKnot = Instantiate(knotClone, knotClone.transform.position, Quaternion.identity);
            }
            newKnot.name = "KnotForNet";
            newKnot.tag = "knotrow";
            newKnot.transform.parent = runtimeRows.transform;
            newKnot.transform.position += transform.up * (i + prevRows) * curHeight;

            newKnot.GetComponent<SplineComputer>().RebuildImmediate();
        }
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        knotCloneRotated.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
    }
}
