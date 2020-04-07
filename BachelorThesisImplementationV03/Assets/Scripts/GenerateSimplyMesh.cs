using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSimplyMesh : GenerateBase
{
    public float verticalOffset;

    void Start()
    {
        BaseStart();
    }

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

        sizeChanger.SetOffsets(width - verticalOffset);
        sizeChanger.ChangeSizesNet();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (prevColumns != (int)columns.value) ChangeColumnsSimply();
            if (prevRows != (int)rows.value) ChangeRowsSimply();
            sizeChanger.ChangeSizesNet();
        }
    }

    public void UpdateNet()
    {
        UpdateKnot();
        sizeChanger.ChangeSizesNet();
    }

    protected void ChangeColumnsSimply()
    {
        int diff = (int)columns.value - prevColumns;

        if (diff > 0) AddColumnsSimply(diff);
        else DeleteColumnsSimply(-diff);

        prevColumns = (int)columns.value;
    }

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

    private void DeleteColumnsSimply(int diff)
    {
        int childCount = runtimeRows.transform.childCount;
        HelperDeleteColumns(runtimeRows, diff, childCount, prevColumns);
    }

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

    protected void ChangeRowsSimply()
    {
        int diff = (int)rows.value - prevRows;

        if (diff > 0) AddRowsSimply(diff);
        else DeleteRowsSimply(-diff);

        prevRows = (int)rows.value;
    }

    private void AddRowsSimply(int diff)
    {
        HelperAddRows(runtimeRows, diff);
        int tmp = prevColumns;
        prevColumns = 0;
        AddColumnsSimply(tmp, prevRows);
        prevColumns = tmp;
    }

    protected void HelperAddRows(GameObject rows, int diff)
    {
        for (int i = 0; i < diff; ++i)
        {
            var newChild = (GameObject)Instantiate(Resources.Load("BaseMeshes/Row"));
            newChild.transform.parent = rows.transform;
        }
    }

    private void DeleteRowsSimply(int diff)
    {
        int firstChildDying = prevRows - diff;
        HelperDeleteRows(runtimeRows, diff, firstChildDying);
    }

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
