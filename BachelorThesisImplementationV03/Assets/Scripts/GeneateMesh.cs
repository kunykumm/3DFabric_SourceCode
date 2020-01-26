using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamteck.Splines
{
    public class GeneateMesh : MonoBehaviour
    {
        public Slider columns;
        public Slider rows;
        public GameObject runtimeRows;

        private SplineComputer splineComputer;
        private GameObject knotPrefab;
        private GameObject knotClone;

        private int basePointCount;
        private SplinePoint[] basePoints;
        private float width;
        private float height;
        private float point_size;
        private int currentPointCount;

        private int prevColumns;
        private int prevRows;

        // Start is called before the first frame update
        void Start()
        {
            knotPrefab = (GameObject)Resources.Load("KnotForNet");
            Instantiate(knotPrefab);
            knotClone = GameObject.Find("KnotForNet(Clone)");
            splineComputer = knotClone.GetComponent<SplineComputer>();
            runtimeRows.transform.position = knotClone.transform.position;

            basePoints = splineComputer.GetPoints();
            basePointCount = basePoints.Length;
            currentPointCount = basePointCount;
            point_size = splineComputer.GetPointSize(0);
            FindMaxsMins();

            prevColumns = 1;
            prevRows = 1;
            ChangeColumns();
            ChangeRows();
        }

        private void FindMaxsMins()
        {
            float minx = basePoints[0].position.x;
            float maxx = basePoints[0].position.x;
            height = basePoints[0].position.y;
            for(int i = 1; i < basePointCount; ++i)
            {
                if (basePoints[i].position.x < minx) minx = basePoints[i].position.x;
                if (basePoints[i].position.x > maxx) maxx = basePoints[i].position.x;
                if (basePoints[i].position.y > height) height = basePoints[i].position.y;
            }
            width = maxx - minx;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (prevColumns != (int)columns.value)
                {
                    ChangeColumns();
                    DeleteRows(prevRows - 1);
                    prevRows = 1;
                    AddRows((int)rows.value - 1);
                }
                if (prevRows != (int)rows.value) ChangeRows();
            }
        }

        private void ChangeColumns()
        {
            int diff = (int)columns.value - prevColumns;
            if (diff > 0)
            {
                AddColumns(diff);

            } else
            {
                DeleteColumns(diff);
            }
            prevColumns = (int)columns.value;
            PrefabUtility.SaveAsPrefabAsset(knotClone, "Assets/Resources/KnotForNet.prefab");
        }

        private void AddColumns(int diff)
        {
            int newPoints = diff * (basePointCount - 1);
            for (int i = 0; i < newPoints; ++i)
            {
                int index = currentPointCount - basePointCount + 1;
                var twinPoint = splineComputer.GetPoint(index);
                splineComputer.SetPointPosition(currentPointCount, new Vector3(twinPoint.position.x + width, twinPoint.position.y, twinPoint.position.z));
                splineComputer.SetPointSize(currentPointCount, point_size);
                splineComputer.SetPointColor(currentPointCount, Color.white);
                splineComputer.SetPointNormal(currentPointCount, splineComputer.GetPointNormal(index));
                currentPointCount++;
            }
        }

        private void DeleteColumns(int diff)
        {
            int new_count = currentPointCount + diff * (basePointCount - 1);
            Debug.Log(new_count);
            if (new_count < basePoints.Length) return;
            SplinePoint[] short_segment = new SplinePoint[new_count];
            SplinePoint[] old_points = splineComputer.GetPoints();
            Array.Copy(old_points, 0, short_segment, 0, new_count);
            splineComputer.SetPoints(short_segment);
            currentPointCount = new_count;
        }

        private void ChangeRows()
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

        private void AddRows(int diff)
        {
            float curHeight = -(height - 0.9f);

            knotPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;
            for (int i = 0; i < diff; ++i)
            {
                GameObject newKnot = Instantiate(knotPrefab, knotPrefab.transform.position, Quaternion.identity);
                newKnot.transform.parent = runtimeRows.transform;
                newKnot.transform.position += transform.up * (i + prevRows) * curHeight;
                if ((i + prevRows) % 2 == 1) newKnot.transform.position += transform.right * (width / 2);
                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
            knotPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        }

        private void DeleteRows(int diff)
        {
            int firstChildDying = prevRows - diff - 1;
            for (int i = 0; i < diff; ++i)
            {
                Destroy(runtimeRows.transform.GetChild(firstChildDying + i).gameObject);
            }
        }
    }
}

