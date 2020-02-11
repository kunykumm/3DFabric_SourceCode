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

        private int rotationWhenGenerated;
        private GameObject rotatedPrefab;

        // Start is called before the first frame update
        void Start()
        {
            knotPrefab = (GameObject)Resources.Load("KnotForNet");
            Instantiate(knotPrefab);
            knotClone = GameObject.Find("KnotForNet(Clone)");
            knotClone.tag = "knotrow";
            splineComputer = knotClone.GetComponent<SplineComputer>();
            knotClone.transform.position = new Vector3(0, 0, 0);

            basePoints = splineComputer.GetPoints();
            basePointCount = basePoints.Length;
            currentPointCount = basePointCount;
            point_size = splineComputer.GetPointSize(0);
            FindMaxsMins();

            rotationWhenGenerated = PlayerPrefs.GetInt("rotation");
            if (rotationWhenGenerated == 1) PrepareRotatedPrefab();

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

        private void PrepareRotatedPrefab()
        {
            GameObject prefabRef = (GameObject)Resources.Load("BaseMeshes/" + PlayerPrefs.GetString("scene") + "MeshRotated");
            GameObject prefabRotated = (GameObject)PrefabUtility.InstantiatePrefab(prefabRef);

            int pointCount = prefabRotated.GetComponent<SplineComputer>().pointCount;
            float pointSize = knotPrefab.GetComponent<SplineComputer>().GetPointSize(0);

            for (int i = 0; i < pointCount; ++i)
            {
                prefabRotated.GetComponent<SplineComputer>().SetPointSize(i, pointSize);
            }

            prefabRotated.GetComponent<TubeGenerator>().sides = knotPrefab.GetComponent<TubeGenerator>().sides;
            prefabRotated.GetComponent<SplineComputer>().RebuildImmediate();

            PrefabUtility.SaveAsPrefabAsset(prefabRotated, "Assets/Resources/KnotRotated.prefab");
            PrefabUtility.SaveAsPrefabAsset(prefabRotated, "Assets/Resources/KnotRotatedForNet.prefab");

            Destroy(prefabRotated);

            rotatedPrefab = (GameObject)Resources.Load("KnotRotatedForNet");
            rotatedPrefab.GetComponent<KnotEditor>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (prevColumns != (int)columns.value)
                {
                    ChangeColumns();
                    UpdateRowsAfterColumnsChanged();
                }
                if (prevRows != (int)rows.value) ChangeRows();
            }
        }

        private void UpdateRowsAfterColumnsChanged()
        {
            DeleteRows(prevRows - 1);
            prevRows = 1;
            AddRows((int)rows.value - 1);
            prevRows = (int)rows.value;
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
            GeneralAddColumns(newPoints, splineComputer);
            if (rotationWhenGenerated == 1)
            {
                currentPointCount -= newPoints;
                GameObject rotated = Instantiate(rotatedPrefab);
                GeneralAddColumns(newPoints, rotated.GetComponent<SplineComputer>());
                PrefabUtility.SaveAsPrefabAsset(rotated, "Assets/Resources/KnotRotatedForNet.prefab");
                Destroy(rotated);
            }
        }

        private void GeneralAddColumns(int newPoints, SplineComputer sc)
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

        private void DeleteColumns(int diff)
        {
            int new_count = currentPointCount + diff * (basePointCount - 1);
            GeneralDeleteColumns(new_count, splineComputer);
            if (rotationWhenGenerated == 1)
            {
                GameObject rotated = Instantiate(rotatedPrefab);
                GeneralDeleteColumns(new_count, rotated.GetComponent<SplineComputer>());
                PrefabUtility.SaveAsPrefabAsset(rotated, "Assets/Resources/KnotRotatedForNet.prefab");
                Destroy(rotated);
            }
            currentPointCount = new_count;
        }

        private void GeneralDeleteColumns(int new_count, SplineComputer sc)
        { 
            if (new_count < basePoints.Length) return;
            SplinePoint[] short_segment = new SplinePoint[new_count];
            SplinePoint[] old_points = sc.GetPoints();
            Array.Copy(old_points, 0, short_segment, 0, new_count);
            sc.SetPoints(short_segment);
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

            GameObject newKnot;

            knotPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;
            if (rotationWhenGenerated == 1) rotatedPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

            for (int i = 0; i < diff; ++i)
            {
                if ((i + prevRows) % 2 == 1 && rotationWhenGenerated == 1)
                {
                    newKnot = Instantiate(rotatedPrefab, rotatedPrefab.transform.position, Quaternion.identity);
                }
                else
                {
                    newKnot = Instantiate(knotPrefab, knotPrefab.transform.position, Quaternion.identity);
                }

                newKnot.tag = "knotrow";
                newKnot.transform.parent = runtimeRows.transform;
                newKnot.transform.position += transform.up * (i + prevRows) * curHeight;

                if ((i + prevRows) % 2 == 1 && rotationWhenGenerated != 1) newKnot.transform.position += transform.right * (width / 2);

                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
            knotPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
            if (rotationWhenGenerated == 1) rotatedPrefab.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        }

        private void DeleteRows(int diff)
        {
            int firstChildDying = prevRows - diff - 1;
            for (int i = 0; i < diff; ++i)
            {
                var child = runtimeRows.transform.GetChild(firstChildDying).gameObject;
                child.transform.parent = null;
                Destroy(child);
            }
        }
    }
}

