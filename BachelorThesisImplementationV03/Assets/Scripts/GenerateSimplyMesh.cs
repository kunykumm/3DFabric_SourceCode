using Dreamteck.Splines;
using UnityEngine;

/// <summary>
/// Extends functionality of GenerateMesh class.
/// Customised for type: Chain mail imitations.
/// Further comments: Suitable for chain mail structures where every two elements in the rows are missing.
/// Scenes: Clover Knot, Square Knot.
/// </summary>
public class GenerateSimplyMesh : GenerateMesh
{
    public float verticalOffset;

    void Start()
    {
        BaseStart();
    }

    /// <summary>
    /// Sets up the default net at the start of the scene.
    /// </summary>
    protected void BaseStart()
    {
        SetupNet(runtimeRows.transform.GetChild(0).transform);
        SetupKnotUtility();

        knotClone.transform.parent = null;
        knotClone.layer = 10;
        knotClone.tag = "hidden";

        prevColumns = 0;
        prevRows = 1;

        ChangeRowsSimply();
        ChangeColumnsSimply();

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
            if (prevColumns != (int)columns.value) ChangeColumnsSimply();
            if (prevRows != (int)rows.value) ChangeRowsSimply();
            updateValues = true;
        }
    }

    /// <summary>
    /// When EditNet button is clicked, this function is called to update all attributes according to knotPrefab.
    /// </summary>
    public override void UpdateNet()
    {
        UpdateKnot();
        sizeChanger.ChangeSizesNet();
    }

    /// <summary>
    /// Decides how to change columns of the net (increase or decrease the count).
    /// </summary>
    protected void ChangeColumnsSimply()
    {
        int diff = (int)columns.value - prevColumns;

        if (diff > 0) AddColumnsSimply(diff);
        else DeleteColumnsSimply(-diff);

        prevColumns = (int)columns.value;
    }

    /// <summary>
    /// Adds new columns.
    /// </summary>
    /// <param name="diff"> Number of columns that will be added to the net. </param>
    /// <param name="beginning"> 
    /// 0 = in case only columns are changed. 
    /// n = index of a row, where generation of all columns starts (in case new rows were added). 
    /// </param>
    private void AddColumnsSimply(int diff, int beginning = 0)
    {
        int childCount = runtimeRows.transform.childCount;

        GameObject newKnot;
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

        for (int i = beginning; i < childCount; ++i)
        {
            var currentChild = runtimeRows.transform.GetChild(i);

            for (int j = 0; j < diff; ++j)
            {
                var newPosition = new Vector3(0, 0, 0);
                newPosition += transform.right * (prevColumns + j) * 2 * verticalOffset;
                newPosition -= transform.up * i * (height - heightOffset);
                if (i % 2 == 1) newPosition += transform.right * verticalOffset;

                newKnot = Instantiate(knotClone, newPosition, Quaternion.identity);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.layer = 9;
                newKnot.transform.parent = currentChild.transform;
                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
        }

        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
    }

    /// <summary>
    /// Deletes columns from the net.
    /// </summary>
    /// <param name="diff"> Number of columns to be deleted in every row. </param>
    protected void DeleteColumnsSimply(int diff)
    {
        int childCount = runtimeRows.transform.childCount;
        HelperDeleteColumns(runtimeRows, diff, childCount, prevColumns);
    }

    /// <summary>
    /// Helper function for deleting columns to avoid repetition.
    /// </summary>
    /// <param name="rows"> RuntimeRows object. </param>
    /// <param name="diff"> Number of columns to be deleted. </param>
    /// <param name="childCount"> Number of rows under RuntimeRows object. </param>
    /// <param name="columnsNum"> Number of columns prior deletion. </param>
    protected void HelperDeleteColumns(GameObject rows, int diff, int childCount, int columnsNum)
    {
        for (int i = 0; i < childCount; ++i)
        {
            var child = rows.transform.GetChild(i);
            int firstChildDying = columnsNum - diff;

            for (int j = 0; j < diff; ++j)
            {
                var grandChild = child.transform.GetChild(firstChildDying).gameObject;
                grandChild.transform.parent = null;
                Destroy(grandChild);
            }
        }
    }

    /// <summary>
    /// Decides how to change rows of the net (increase or decrease the count).
    /// </summary>
    protected void ChangeRowsSimply()
    {
        int diff = (int)rows.value - prevRows;

        if (diff > 0) AddRowsSimply(diff);
        else DeleteRowsSimply(-diff);

        prevRows = (int)rows.value;
    }

    /// <summary>
    /// Adds new rows to the net.
    /// </summary>
    /// <param name="diff"> Number of rows to be added. </param>
    private void AddRowsSimply(int diff)
    {
        HelperAddRows(runtimeRows, diff);
        int tmp = prevColumns;
        prevColumns = 0;
        AddColumnsSimply(tmp, prevRows);
        prevColumns = tmp;
    }

    /// <summary>
    /// Helper function for adding rows to avoid repetition.
    /// </summary>
    /// <param name="rows"> RuntimeRows object. </param>
    /// <param name="diff"> Number of rows to be added. </param>
    protected void HelperAddRows(GameObject rows, int diff)
    {
        for (int i = 0; i < diff; ++i)
        {
            var newChild = (GameObject)Instantiate(Resources.Load("BaseMeshes/Row"));
            newChild.transform.parent = rows.transform;
        }
    }

    /// <summary>
    /// Deletes rows from the net.
    /// </summary>
    /// <param name="diff"> Number of rows to be deleted. </param>
    protected void DeleteRowsSimply(int diff)
    {
        int firstChildDying = prevRows - diff;
        HelperDeleteRows(runtimeRows, diff, firstChildDying);
    }

    /// <summary>
    /// Helper function for deleting rows to avoid repetition.
    /// </summary>
    /// <param name="rows"> RuntimeRows object. </param>
    /// <param name="diff"> Number of rows to be deleted from the net. </param>
    /// <param name="firstChildDying"> Index of a row from which the deletion starts. </param>
    protected void HelperDeleteRows(GameObject rows, int diff, int firstChildDying)
    {
        for (int i = 0; i < diff; ++i)
        {
            var dyingChild = rows.transform.GetChild(firstChildDying).gameObject;
            int count = dyingChild.transform.childCount;
            for (int j = 0; j < count; ++j)
            {
                var grandChild = dyingChild.transform.GetChild(0).gameObject;
                grandChild.transform.parent = null;
                Destroy(grandChild);
            }
            dyingChild.transform.parent = null;
            Destroy(dyingChild);
        }
    }
}
