using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshTwoKnots : GenerateMesh
{
    public GameObject knotPrefabRotated;

    private GameObject knotCloneRotated;

    private SplineComputer splineComputerRotated;

    //private SplinePoint[] basePointsRotated;


    void Start()
    {
        SetupNet();
        SetupComplicatedNet();
        FindMaxsMins();

        prevColumns = 1;
        prevRows = 1;

        //ChangeColumns();
        //ChangeRows();
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

        knotCloneRotated.transform.parent = runtimeRows.transform;
        knotCloneRotated.tag = "knotrow";
        knotCloneRotated.layer = 10;

        splineComputerRotated = knotCloneRotated.GetComponent<SplineComputer>();
        //basePointsRotated = splineComputerRotated.GetPoints();
    }

    public void UpdateComplicatedNet()
    {
        DeleteRows(prevRows - 2);
        knotClone = runtimeRows.transform.GetChild(0).gameObject;
        knotCloneRotated = runtimeRows.transform.GetChild(1).gameObject;
        knotClone.transform.parent = null;
        knotCloneRotated.transform.parent = null;

        SetupNet();
        SetupComplicatedNet();

        prevColumns = 1;
        prevRows = 1;

        //ChangeColumns();
        //ChangeRows();
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
            DeleteRowsComplicated(-diff);
        }
        prevRows = (int)rows.value;
    }

    private void AddRowsComplicated(int diff)
    {

    }

    private void DeleteRowsComplicated(int diff)
    {
        int firstChildDying = prevRows - diff;
        for (int i = 0; i < diff; ++i)
        {
            if (runtimeRows.transform.childCount == 2)
            {
                var hiddenchild = runtimeRows.transform.GetChild(firstChildDying).gameObject;
                hiddenchild.layer = 10;
                break;
            }

            var child = runtimeRows.transform.GetChild(firstChildDying).gameObject;
            child.transform.parent = null;
            Destroy(child);
        }
    }
}
