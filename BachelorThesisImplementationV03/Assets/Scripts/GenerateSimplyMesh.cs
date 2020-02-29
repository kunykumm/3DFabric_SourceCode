using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSimplyMesh : GenerateBase
{
    public float verticalOffset;

    void Start()
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

        sizeChanger.SetHeightOffset(heightOffset);
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
        SetupNet(runtimeRows.transform.GetChild(0).transform);
        knotClone.transform.parent = null;
        knotClone.layer = 10;
        knotClone.tag = "hidden";

        DeleteRowsSimply(prevRows);
        //DeleteColumnsSimply(prevColumns - 1);

        prevColumns = 0;
        prevRows = 0;

        ChangeRowsSimply();
        ChangeColumnsSimply();

        sizeChanger.ChangeSizesNet();
    }

    private void ChangeColumnsSimply()
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
        
        for(int i = 0; i < childCount; ++i)
        {
            var child = runtimeRows.transform.GetChild(i);
            int firstChildDying = prevColumns - diff;

            for (int j = 0; j < diff; ++j)
            {
                var grandChild = child.transform.GetChild(firstChildDying).gameObject;
                grandChild.transform.parent = null;
                Destroy(grandChild);
            }
        }
    }

    private void ChangeRowsSimply()
    {
        int diff = (int)rows.value - prevRows;

        if (diff > 0) AddRowsSimply(diff);
        else DeleteRowsSimply(-diff);

        prevRows = (int)rows.value;
    }

    private void AddRowsSimply(int diff)
    {
        for (int i = 0; i < diff; ++i)
        {
            var newChild = (GameObject)Instantiate(Resources.Load("BaseMeshes/Row"));
            newChild.transform.parent = runtimeRows.transform;
        }
        int tmp = prevColumns;
        prevColumns = 0;
        AddColumnsSimply(tmp, prevRows);
        prevColumns = tmp;
    }

    private void DeleteRowsSimply(int diff)
    {
        int firstChildDying = prevRows - diff;

        for (int i = 0; i < diff; ++i)
        {
            var dyingChild = runtimeRows.transform.GetChild(firstChildDying).gameObject;
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
