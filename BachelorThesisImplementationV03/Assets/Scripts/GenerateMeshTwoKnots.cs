using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshTwoKnots : GenerateMesh
{
    public GameObject knotPrefabRotated;

    private GameObject knotCloneRotated;

    private SplineComputer splineComputerRotated;


    void Start()
    {
        SetupNet();
        SetupComplicatedNet();
        FindMaxsMins();

        prevColumns = 1;
        prevRows = 1;

        ChangeColumnsComplicated();
        ChangeRowsComplicated();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (prevColumns != (int)columns.value) ChangeColumnsComplicated();
            if (prevRows != (int)rows.value) ChangeRowsComplicated();
        }
    }

    protected void SetupComplicatedNet()
    {
        knotCloneRotated = GameObject.Find("KnotForNetRotated");
        var points = knotPrefabRotated.GetComponent<SplineComputer>().GetPoints();
        knotCloneRotated.GetComponent<SplineComputer>().SetPoints(points);
        knotCloneRotated.GetComponent<TubeGenerator>().sides = knotPrefabRotated.GetComponent<TubeGenerator>().sides;
        knotCloneRotated.layer = 10;
        splineComputerRotated = knotCloneRotated.GetComponent<SplineComputer>();
    }

    public void UpdateComplicatedNet()
    {
        DeleteRows(prevRows - 1);
        knotClone = runtimeRows.transform.GetChild(0).gameObject;
        knotClone.transform.parent = null;

        SetupNet();
        SetupComplicatedNet();

        prevColumns = 1;
        prevRows = 1;

        ChangeColumnsComplicated();
        ChangeRowsComplicated();
    }

    private void ChangeColumnsComplicated()
    {
        int diff = (int)columns.value - prevColumns;
        if (diff > 0)
        {
            int newPoints = diff * (basePointCount - 1);
            AddColumns(newPoints, splineComputer);
            currentPointCount -= newPoints;
            AddColumns(newPoints, splineComputerRotated);
        }
        else
        {
            int newCount = currentPointCount + diff * (basePointCount - 1);
            DeleteColumns(newCount, splineComputer);
            DeleteColumns(newCount, splineComputerRotated);
        }
        prevColumns = (int)columns.value;
    }

    private void ChangeRowsComplicated()
    {
        int diff = (int)rows.value - prevRows;
        if (diff > 0)
        {
            AddRowsComplicated(diff);
        }
        else
        {
            DeleteRows(-diff);
        }
        prevRows = (int)rows.value;
    }

    private void AddRowsComplicated(int diff)
    {
        float curHeight = -(height - heightOffset);

        GameObject newKnot;

        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;
        knotCloneRotated.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

        for (int i = 0; i < diff; ++i)
        {
            if ((i + prevRows) % 2 == 1)
            {
                newKnot = Instantiate(knotCloneRotated, knotCloneRotated.transform.position, Quaternion.identity);
                newKnot.layer = 9;
            } 
            else
            {
                newKnot = Instantiate(knotClone, knotClone.transform.position, Quaternion.identity);
            }
            newKnot.name = "KnotForNet";
            newKnot.tag = "knotrow";
            newKnot.transform.parent = runtimeRows.transform;
            newKnot.transform.position += transform.up * (i + prevRows) * curHeight;

            //if ((i + prevRows) % 2 == 1) newKnot.transform.position += transform.right * (width / 2);

            newKnot.GetComponent<SplineComputer>().RebuildImmediate();
        }
        knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        knotCloneRotated.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
    }
}
