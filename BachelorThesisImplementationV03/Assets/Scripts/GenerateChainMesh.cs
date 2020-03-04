using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChainMesh : GenerateSimplyMesh
{
    public GameObject runtimeInterRows;
    public GameObject starter;

    private GameObject subnet0;
    private GameObject subnet1;
    private int prevInterColumns;
    private int prevInterRows;


    void Start()
    {
        BaseStart();

        subnet0 = GameObject.Find("subnet0");
        subnet1 = GameObject.Find("subnet1");

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
                Debug.Log("Cols:" + prevColumns + ", Inters:" + prevInterColumns);
            }
            if (prevRows != (int)rows.value)
            {
                ChangeRowsSimply();
                ChangeInterRows();
                Debug.Log("Rows:" + prevRows + ", Inters:" + prevInterRows);
            }
            sizeChanger.ChangeSizesNet();
        }
    }

    private void ChangeInterColumns()
    {
        int offset = 1;
        if (prevInterColumns % 2 == 1) prevInterColumns += 1;
        int diff = 2 * ((int)columns.value - 1) - prevInterColumns - 1;

        if (diff > 0) AddInterColumns(diff);
        else DeleteInterColumns(-diff, offset);

        prevInterColumns = (int)columns.value;
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

                if((j + prevInterColumns) % 2 == 0)
                {

                    if (i % 2 == 0) xRotation *= -1;
                    if (i % 2 == 1) newPosition = new Vector3(0.6f, -1.4f, 0);

                } else
                {
                    if (i % 2 == 1) xRotation *= -1;
                    if (i % 2 == 0) newPosition = new Vector3(0.6f, -1.4f, 0);

                }

                newPosition += transform.right * (prevInterColumns + j) * width;
                newPosition -= transform.up * i * height;

                newKnot = Instantiate(knotClone, newPosition, Quaternion.Euler(new Vector3(xRotation, 90, 0)));

                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.layer = 9;
                newKnot.transform.parent = currentChild.transform;
                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
        }

        Debug.Log(runtimeInterRows.transform.GetChild(0).transform.childCount);
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
    }

    private void DeleteInterColumns(int diff, int offset)
    {
        int childCount = runtimeInterRows.transform.childCount;
        HelperDeleteColumns(runtimeInterRows, diff + offset, childCount, prevInterColumns);
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
