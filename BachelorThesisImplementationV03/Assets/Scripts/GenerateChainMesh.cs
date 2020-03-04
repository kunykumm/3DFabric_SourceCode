using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChainMesh : GenerateSimplyMesh
{
    public GameObject runtimeInterRows;

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
        int diff = (int)columns.value - 1 - prevInterColumns;

        if (diff > 0) AddInterColumns(diff);
        else DeleteInterColumns(-diff);

        prevInterColumns = (int)columns.value - 1;
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
                var newPosition = new Vector3(2, -2, 0);
                newPosition -= transform.up * i * height;
                newPosition += transform.right * j * width;

                //if (i % 2 == 0) 
                Debug.Log("subnet" + j % 2);
                newKnot = Instantiate(GameObject.Find("subnet" + j % 2), newPosition, Quaternion.identity);
                //else newKnot = Instantiate(GameObject.Find("subnet" + j % 2), newPosition, Quaternion.identity);

                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.layer = 9;
                newKnot.transform.parent = currentChild.transform;
                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
        }

        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
    }

    private void DeleteInterColumns(int diff)
    {
        int childCount = runtimeInterRows.transform.childCount;
        HelperDeleteColumns(runtimeInterRows, diff - 1, childCount);
    }

    private void ChangeInterRows()
    {
        int diff = (int)rows.value - 1 - prevInterRows;

        if (diff > 0) AddInterRows(diff);
        else DeleteInterRows(-diff);

        prevInterRows = (int)rows.value - 1;
    }

    private void AddInterRows(int diff)
    {
        HelperAddRows(runtimeInterRows, diff - 1);
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
