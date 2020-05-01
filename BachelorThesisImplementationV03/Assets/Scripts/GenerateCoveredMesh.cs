using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCoveredMesh : GenerateSimplyMesh
{
    public bool hexagonal;
    public bool triangle;

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

    public override void UpdateNet()
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
        Quaternion quat = Quaternion.identity;
        Vector3 rotation = new Vector3(0, 0, 60);

        for (int i = beginning; i < childCount; ++i)
        {
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
                    if (((prevColumns + j) % 2 == 1 && (prevRows + i) % 2 == 0) || ((prevColumns + j) % 2 == 0 && (prevRows + i) % 2 == 1))
                    {
                        newPosition -= transform.up * ((height - heightOffset) / 3);
                        quat = Quaternion.Euler(rotation);
                    }
                }
                GameObject newKnot = Instantiate(knotClone, newPosition, quat);
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
