using Dreamteck.Splines;
using UnityEngine;

/// <summary>
/// Extends functionality of GenerateSimplyMesh class.
/// Customised for type: Chain mail imitations.
/// Further comments: Suitable for complicated chain mail structures..
/// Scenes: Circle Knot.
/// </summary>
public class GenerateChainMesh : GenerateSimplyMesh
{
    public GameObject runtimeInterRows;
    public GameObject starter;

    private int prevInterColumns;
    private int prevInterRows;


    /// <summary>
    /// Sets up the default net at the start of the scene.
    /// </summary>
    void Start()
    {
        BaseStart();

        prevInterColumns = 0;
        prevInterRows = 1;

        ChangeInterRows();
        ChangeInterColumns();
    }
    /// <summary>
    /// In case the value of column and row sliders change, the functions to change the net are called. 
    /// There are two other functions to generate an additional grid of elements needed to connect the elements in the main grid.
    /// 
    /// o o o o o o
    ///  o o o o o
    /// o o o o o o     -> This is the main grid.
    /// 
    /// \/\/\/\/\/
    /// /\/\/\/\/\      -> This is the additional grid connecting the elements of the main grid together.
    /// 
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
            if (prevColumns != (int)columns.value)
            {
                ChangeColumnsSimply();
                ChangeInterColumns();
            }
            if (prevRows != (int)rows.value)
            {
                ChangeRowsSimply();
                ChangeInterRows();
            }
            updateValues = true;
        }
    }
    /// <summary>
    /// Decides how to change columns of the additional net (increase or decrease the count).
    /// </summary>
    private void ChangeInterColumns()
    {
        int diff = 2 * (int)columns.value - 1 - prevInterColumns;

        if (diff > 0) AddInterColumns(diff);
        else DeleteInterColumns(-diff);

        prevInterColumns = 2 * (int)columns.value - 1;
    }

    /// <summary>
    /// Adds new columns.
    /// </summary>
    /// <param name="diff"> Number of columns that will be added to the net. </param>
    /// <param name="beginning">
    /// 0 = in case only columns are changed. 
    /// n = index of a row, where generation of all columns starts (in case new rows were added).
    /// </param>
    private void AddInterColumns(int diff, int beginning = 0)
    {
        int childCount = runtimeInterRows.transform.childCount;

        GameObject newKnot;
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

        for (int i = beginning; i < childCount; ++i)
        {
            var currentChild = runtimeInterRows.transform.GetChild(i);

            for (int j = 0; j < diff; ++j)
            {
                var newPosition = new Vector3(3.4f, -1.4f, 0);
                int xRotation = 45;

                InterColumnsPosAngleChanger(i, j, ref newPosition, ref xRotation);

                newKnot = Instantiate(knotClone, newPosition, Quaternion.Euler(new Vector3(xRotation, 90, 0)));

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
    /// Special function to change the rotation and position of elements in the additional grid. 
    /// </summary>
    /// <param name="childIndex"> Index of a row. </param>
    /// <param name="diffIndex"> Index of a new column. </param>
    /// <param name="position"> Default position of the element. </param>
    /// <param name="rotation"> Rotation in the direction of x axis. </param>
    private void InterColumnsPosAngleChanger(int childIndex, int diffIndex, ref Vector3 position, ref int rotation)
    {
        if ((diffIndex + prevInterColumns) % 2 == 0)
        {

            if (childIndex % 2 == 0) rotation *= -1;
            if (childIndex % 2 == 1) position = new Vector3(0.6f, -1.4f, 0);

        }
        else
        {
            if (childIndex % 2 == 1) rotation *= -1;
            if (childIndex % 2 == 0) position = new Vector3(0.6f, -1.4f, 0);

        }

        position += transform.right * (prevInterColumns + diffIndex) * width;
        position -= transform.up * childIndex * height;
    }

    /// <summary>
    /// Deletes columns from the additional net.
    /// </summary>
    /// <param name="diff"></param>
    private void DeleteInterColumns(int diff)
    {
        int childCount = runtimeInterRows.transform.childCount;
        HelperDeleteColumns(runtimeInterRows, diff, childCount, prevInterColumns);
    }

    /// <summary>
    /// Decides how to change rows of the additional net (increase or decrease the count).
    /// </summary>
    private void ChangeInterRows()
    {
        int diff = (int)rows.value - 1 - prevInterRows;

        if (diff > 0) AddInterRows(diff);
        else DeleteInterRows(-diff + 1);

        prevInterRows = (int)rows.value - 1;
    }

    /// <summary>
    /// Adds new rows to the additional net.
    /// </summary>
    /// <param name="diff"> Number of new rows to add. </param>
    private void AddInterRows(int diff)
    {
        HelperAddRows(runtimeInterRows, diff);
        int tmp = prevInterColumns;
        prevInterColumns = 0;
        AddInterColumns(tmp, prevInterRows);
        prevInterColumns = tmp;
    }

    /// <summary>
    /// Deletes rows from the additional net.
    /// </summary>
    /// <param name="diff"></param>
    private void DeleteInterRows(int diff)
    {
        int firstChildDying = prevInterRows - diff + 1;
        HelperDeleteRows(runtimeInterRows, diff - 1, firstChildDying);
    }
}
