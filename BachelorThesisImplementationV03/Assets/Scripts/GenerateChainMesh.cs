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
        int diff = (int)columns.value - 1 - prevColumns;

        if (diff > 0) AddInterColumns(diff);
        else DeleteInterColumns(-diff);

        prevColumns = (int)columns.value;
    }

    private void AddInterColumns(int diff, int beginning = 0)
    {

    }

    private void DeleteInterColumns(int diff)
    {

    }

    private void ChangeInterRows()
    {
        int diff = (int)rows.value - 1 - prevRows;

        if (diff > 0) AddInterRows(diff);
        else DeleteInterRows(-diff);

        prevRows = (int)rows.value;
    }

    private void AddInterRows(int diff)
    {

    }

    private void DeleteInterRows(int diff)
    {

    }
}
