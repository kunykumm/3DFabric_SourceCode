using Dreamteck.Splines;
using UnityEngine;

public class GenerateChainMesh : GenerateSimplyMesh
{
    public GameObject runtimeInterRows;
    public GameObject starter;

    private int prevInterColumns;
    private int prevInterRows;


    void Start()
    {
        BaseStart();

        prevInterColumns = 0;
        prevInterRows = 1;

        ChangeInterRows();
        ChangeInterColumns();
    }

    void Update()
    {
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
            sizeChanger.ChangeSizesNet();
        }
    }

    private void ChangeInterColumns()
    {
        int diff = 2 * (int)columns.value - 1 - prevInterColumns;

        if (diff > 0) AddInterColumns(diff);
        else DeleteInterColumns(-diff);

        prevInterColumns = 2 * (int)columns.value - 1;
    }

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

    private void DeleteInterColumns(int diff)
    {
        int childCount = runtimeInterRows.transform.childCount;
        HelperDeleteColumns(runtimeInterRows, diff, childCount, prevInterColumns);
    }

    private void ChangeInterRows()
    {
        int diff = (int)rows.value - 1 - prevInterRows;

        if (diff > 0) AddInterRows(diff);
        else DeleteInterRows(-diff + 1);

        prevInterRows = (int)rows.value - 1;
    }

    private void AddInterRows(int diff)
    {
        HelperAddRows(runtimeInterRows, diff);
        int tmp = prevInterColumns;
        prevInterColumns = 0;
        AddInterColumns(tmp, prevInterRows);
        prevInterColumns = tmp;
    }

    private void DeleteInterRows(int diff)
    {
        int firstChildDying = prevInterRows - diff + 1;
        HelperDeleteRows(runtimeInterRows, diff - 1, firstChildDying);
    }
}
