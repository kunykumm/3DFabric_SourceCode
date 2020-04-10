﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCoveredMesh : GenerateSimplyMesh
{
    public bool hexagonal;

    void Start()
    {
        knotClone = GameObject.Find("KnotForNet");
        height = 1;
        width = 1;

        knotClone.transform.parent = null;
        knotClone.layer = 10;
        knotClone.tag = "hidden";

        prevColumns = 0;
        prevRows = 1;

        ChangeRowsCovered();
        ChangeColumnsCovered();

        sizeChanger.SetOffsets(0.1f, 0.1f);
        sizeChanger.ChangeSizesNet();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (prevColumns != (int)columns.value) ChangeColumnsCovered();
            if (prevRows != (int)rows.value) ChangeRowsCovered();
            sizeChanger.ChangeSizesNet();
        }
    }

    public void UpdateCoveredNet()
    { 
        sizeChanger.ChangeSizesNet();
    }

    protected void ChangeColumnsCovered()
    {
        int diff = (int)columns.value - prevColumns;

        if (diff > 0) AddColumnsCovered(diff);
        else DeleteColumnsSimply(-diff);

        prevColumns = (int)columns.value;
    }

    private void AddColumnsCovered(int diff, int beginning = 0)
    {
        int childCount = runtimeRows.transform.childCount;

        for (int i = beginning; i < childCount; ++i)
        {
            var currentChild = runtimeRows.transform.GetChild(i);

            for (int j = 0; j < diff; ++j)
            {
                var newPosition = new Vector3(0, 0, 0);
                newPosition += transform.right * (prevColumns + j) * 2 * verticalOffset;
                newPosition -= transform.up * i * (height - heightOffset);
                if (hexagonal) newPosition += transform.right * verticalOffset;

                GameObject newKnot = Instantiate(knotClone, newPosition, Quaternion.identity);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.layer = 9;
                newKnot.transform.parent = currentChild.transform;
            }
        }
    }

    protected void ChangeRowsCovered()
    {
        int diff = (int)rows.value - prevRows;

        if (diff > 0) AddRowsCovered(diff);
        else DeleteRowsSimply(-diff);

        prevRows = (int)rows.value;
    }

    private void AddRowsCovered(int diff)
    {
        HelperAddRows(runtimeRows, diff);
        int tmp = prevColumns;
        prevColumns = 0;
        AddColumnsCovered(tmp, prevRows);
        prevColumns = tmp;
    }

}
