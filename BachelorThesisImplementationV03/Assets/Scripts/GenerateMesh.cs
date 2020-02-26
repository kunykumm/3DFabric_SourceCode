
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamteck.Splines
{
    public class GenerateMesh : GenerateBase
    {
        void Start()
        {
            SetupNet();
            SetupKnotUtility();

            ChangeColumns();
            ChangeRows();

            sizeChanger.SetHeightOffset(heightOffset);
            sizeChanger.ChangeSizesNet();
        }

        public void UpdateNet()
        {
            DeleteRows(prevRows - 1);
            knotClone = runtimeRows.transform.GetChild(0).gameObject;
            knotClone.transform.parent = null;

            SetupNet();

            prevColumns = 1;
            prevRows = 1;

            ChangeColumns();
            ChangeRows();

            sizeChanger.ChangeSizesNet();
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (prevColumns != (int)columns.value) ChangeColumns();
                if (prevRows != (int)rows.value) ChangeRows();
                sizeChanger.ChangeSizesNet();
            }
        }

        protected void ChangeColumns()
        {
            int diff = (int)columns.value - prevColumns;
            if (diff > 0)
            {
                int newPoints = diff * (basePointCount - 1);
                AddColumns(newPoints, splineComputer);
            }
            else
            {
                int newCount = currentPointCount + diff * (basePointCount - 1);
                DeleteColumns(newCount, splineComputer);
            }
            prevColumns = (int)columns.value;
        }

        protected void AddColumns(int newPoints, SplineComputer sc)
        {
            for (int i = 0; i < newPoints; ++i)
            {
                int index = currentPointCount - basePointCount + 1;
                var twinPoint = sc.GetPoint(index);
                sc.SetPointPosition(currentPointCount, new Vector3(twinPoint.position.x + width, twinPoint.position.y, twinPoint.position.z));
                sc.SetPointSize(currentPointCount, point_size);
                sc.SetPointColor(currentPointCount, Color.white);
                sc.SetPointNormal(currentPointCount, sc.GetPointNormal(index));
                currentPointCount++;
            }
        }

        protected void DeleteColumns(int newCount, SplineComputer sc)
        {
            if (newCount < basePoints.Length) return;
            SplinePoint[] short_segment = new SplinePoint[newCount];
            SplinePoint[] old_points = sc.GetPoints();
            Array.Copy(old_points, 0, short_segment, 0, newCount);
            sc.SetPoints(short_segment);
            currentPointCount = newCount;
        }

        protected void ChangeRows()
        {
            int diff = (int)rows.value - prevRows;
            if (diff > 0)
            {
                AddRows(diff);
            }
            else
            {
                DeleteRows(-diff);
            }
            prevRows = (int)rows.value;
        }

        protected void AddRows(int diff)
        {
            float curHeight = -(height - heightOffset);

            GameObject newKnot;

            knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

            for (int i = 0; i < diff; ++i)
            {
                newKnot = Instantiate(knotClone, knotClone.transform.position, Quaternion.identity);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.transform.parent = runtimeRows.transform;
                newKnot.transform.position += transform.up * (i + prevRows) * curHeight;

                if ((i + prevRows) % 2 == 1) newKnot.transform.position += transform.right * (width / 2);

                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
            knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        }

        protected void DeleteRows(int diff)
        {
            int firstChildDying = prevRows - diff;
            for (int i = 0; i < diff; ++i)
            {
                var child = runtimeRows.transform.GetChild(firstChildDying).gameObject;
                child.transform.parent = null;
                Destroy(child);
            }
        }
    }
}