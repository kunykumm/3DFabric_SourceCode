using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSimplyMesh : GenerateBase
{
    public float verticalOffset;

    void Start()
    {
        SetupNet();
        SetupKnotUtility();

        ChangeColumnsSimply();
        ChangeRowsSimply();

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
        //DeleteRows(prevRows - 1);
        //knotClone = runtimeRows.transform.GetChild(0).gameObject;
        //knotClone.transform.parent = null;

        //SetupNet();

        //prevColumns = 1;
        //prevRows = 1;

        //ChangeColumns();
        //ChangeRows();

        //sizeChanger.ChangeSizesNet();
    }

    private void ChangeColumnsSimply()
    {
        int diff = (int)columns.value - prevColumns;

        if (diff > 0) AddColumnsSimply(diff);
        else DeleteColumnsSimply(diff);

        prevColumns = (int)columns.value;
    }

    private void AddColumnsSimply(int diff)
    {
        int childCount = runtimeRows.transform.childCount;
        var child = runtimeRows.transform.GetChild(0);
        int grandChildCount = child.transform.childCount;
        int newDiff = diff - grandChildCount - 1;

        GameObject newKnot;
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

        for (int i = 0; i < childCount; ++i)
        {
            var currentChild = runtimeRows.transform.GetChild(i);

            for (int j = 0; j < newDiff; ++j)
            {
                newKnot = Instantiate(knotClone, knotClone.transform.position, Quaternion.identity);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.transform.parent = currentChild.transform;
                newKnot.transform.position += transform.right * (child.transform.childCount + j) * 2 * verticalOffset;
                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
        }
    }

    private void DeleteColumnsSimply(int diff)
    {

    }

    private void ChangeRowsSimply()
    {
        //int diff = (int)rows.value - prevRows;
        //if (diff > 0)
        //{
        //    AddRows(diff);
        //}
        //else
        //{
        //    DeleteRows(-diff);
        //}
        //prevRows = (int)rows.value;
    }
}
