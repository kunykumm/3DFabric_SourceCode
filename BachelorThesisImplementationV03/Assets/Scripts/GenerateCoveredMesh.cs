using UnityEngine;

/// <summary>
/// Extends functionality of GenerateSimplyMesh class.
/// Customised for type: Covered textiles.
/// Further comments:.
/// Scenes: Covered Triangle, Covered Square, Covered Hexagon.
/// </summary>
public class GenerateCoveredMesh : GenerateSimplyMesh
{
    public bool hexagonal;
    public bool triangle;

    /// <summary>
    /// Sets up the default net at the start of the scene.
    /// </summary>
    void Start()
    {
        knotClone = GameObject.Find("KnotForNet");
        height = 1f;
        width = 1f;

        knotClone.transform.parent = null;
        knotClone.layer = 10;
        knotClone.tag = "hidden";

        prevColumns = 0;
        prevRows = 1;

        ChangeRowsCovered();
        ChangeColumnsCovered();

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
            if (prevColumns != (int)columns.value) ChangeColumnsCovered();
            if (prevRows != (int)rows.value) ChangeRowsCovered();
            updateValues = true;
        }
    }

    /// <summary>
    /// When EditNet button is clicked, this function is called the information about size of the net.
    /// In this case, there is no Dreamteck Splines knotPrefab, so no knotClone needs to be updated.
    /// </summary>
    public override void UpdateNet()
    { 
        sizeChanger.ChangeSizesNet();
    }

    /// <summary>
    /// Decides how to change columns of the net (increase or decrease the count).
    /// </summary>
    protected void ChangeColumnsCovered()
    {
        int diff = (int)columns.value - prevColumns;

        if (diff > 0) AddColumnsCovered(diff);
        else DeleteColumnsSimply(-diff);

        prevColumns = (int)columns.value;
    }

    /// <summary>
    /// Adds new columns to the net.
    /// Contains different options for triangle and hexagon cases (square is default).
    /// </summary>
    /// <param name="diff"> Number of columns that will be added to the net. </param>
    /// <param name="beginning"> 
    /// 0 = in case only columns are changed. 
    /// n = index of a row, where generation of all columns starts (in case new rows were added). 
    /// </param>
    private void AddColumnsCovered(int diff, int beginning = 0)
    {
        bool rotateElement = false;
        int childCount = runtimeRows.transform.childCount;
        Quaternion quat = Quaternion.identity;
        Vector3 rotation = new Vector3(0, 0, 60);

        for (int i = beginning; i < childCount; ++i)
        {
            if ((i % 2) == (prevColumns % 2)) rotateElement = true;
            else rotateElement = false;

            var currentChild = runtimeRows.transform.GetChild(i);

            for (int j = 0; j < diff; ++j)
            {
                quat = Quaternion.identity;
                var newPosition = new Vector3(0, 0, 0);
                newPosition -= transform.up * i * (height - heightOffset);
                newPosition += transform.right * (prevColumns + j) * (width - verticalOffset);
                if (hexagonal)
                {
                    newPosition -= transform.right * (prevColumns + j) * (-verticalOffset);
                    if ((prevColumns + j) % 2 == 1) newPosition += transform.up * (-height / 2 + heightOffset / 2);
                }
                if (triangle)
                {
                    newPosition -= transform.up * i * (-heightOffset) * 4 / 3.1f;
                    newPosition -= transform.right * (prevColumns + j) * (-verticalOffset) * 2;
                    if (rotateElement)
                    {
                        newPosition -= transform.up * ((height - heightOffset) / 3 + 0.065f);
                        quat = Quaternion.Euler(rotation);
                    }
                    rotateElement = !rotateElement;
                }
                GameObject newKnot = Instantiate(knotClone, newPosition, quat);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.layer = 9;
                newKnot.transform.parent = currentChild.transform;
            }
        }
    }

    /// <summary>
    /// Decides how to change rows of the net (increase or decrease the count).
    /// </summary>
    protected void ChangeRowsCovered()
    {
        int diff = (int)rows.value - prevRows;

        if (diff > 0) AddRowsCovered(diff);
        else DeleteRowsSimply(-diff);

        prevRows = (int)rows.value;
    }

    /// <summary>
    /// Adds new rows to the net.
    /// </summary>
    /// <param name="diff"> Number of rows to be added. </param>
    private void AddRowsCovered(int diff)
    {
        HelperAddRows(runtimeRows, diff);
        int tmp = prevColumns;
        prevColumns = 0;
        AddColumnsCovered(tmp, prevRows);
        prevColumns = tmp;
    }

}
