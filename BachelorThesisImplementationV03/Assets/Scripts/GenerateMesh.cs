
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamteck.Splines
{
    public class GenerateMesh : MonoBehaviour
    {
        public Slider columns;
        public Slider rows;
        public GameObject runtimeRows;
        public GameObject knotPrefab;
        public float heightOffset;

        private float point_size;

        protected SplinePoint[] basePoints;
        protected KnotUtility knotUti;
        protected GameObject knotClone;
        protected SplineComputer splineComputer;
        protected float width;
        protected float height;
        protected int currentPointCount;
        protected int basePointCount;
        protected int prevColumns;
        protected int prevRows;


        void Start()
        {
            SetupNet();
            knotUti = new KnotUtility();
            knotUti.FindMaxsMins(ref height, ref width, basePoints);

            prevColumns = 1;
            prevRows = 1;
            ChangeColumns();
            ChangeRows();
        }

        protected void SetupNet()
        {
            knotClone = GameObject.Find("KnotForNet");

            var points = knotPrefab.GetComponent<SplineComputer>().GetPoints();
            knotClone.GetComponent<SplineComputer>().SetPoints(points);
            knotClone.GetComponent<TubeGenerator>().sides = knotPrefab.GetComponent<TubeGenerator>().sides;

            knotClone.transform.parent = runtimeRows.transform;
            knotClone.tag = "knotrow";
            knotClone.layer = 9;

            splineComputer = knotClone.GetComponent<SplineComputer>();
            basePoints = splineComputer.GetPoints();
            basePointCount = basePoints.Length;
            currentPointCount = basePointCount;
            point_size = splineComputer.GetPointSize(0);
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
        }

        //protected void FindMaxsMins()
        //{
        //    float minx = basePoints[0].position.x;
        //    float maxx = basePoints[0].position.x;
        //    height = basePoints[0].position.y;
        //    for (int i = 1; i < basePointCount; ++i)
        //    {
        //        if (basePoints[i].position.x < minx) minx = basePoints[i].position.x;
        //        if (basePoints[i].position.x > maxx) maxx = basePoints[i].position.x;
        //        if (basePoints[i].position.y > height) height = basePoints[i].position.y;
        //    }
        //    width = maxx - minx;
        //}


        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (prevColumns != (int)columns.value) ChangeColumns();
                if (prevRows != (int)rows.value) ChangeRows();
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