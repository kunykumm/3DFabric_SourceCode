using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamteck.Splines
{
    public class GeneateMesh : MonoBehaviour
    {
        public Slider columns;
        public Slider rows;

        private SplineComputer splineComputer;

        private int basePointCount;
        private SplinePoint[] basePoints;
        private float width;
        private float height;
        private int currentPointCount;

        private int prevColumns;
        private int prevRows;

        // Start is called before the first frame update
        void Start()
        {
            splineComputer = GameObject.Find("Knot").GetComponent<SplineComputer>();
            basePoints = splineComputer.GetPoints();
            basePointCount = basePoints.Length;
            currentPointCount = basePointCount;
            FindMaxsMins();
            Debug.Log(width);
            Debug.Log(height);

            prevColumns = 0;
            prevRows = 0;
            ChangeColumns();
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
            Debug.Log(maxx);
            Debug.Log(minx);
            width = maxx - minx;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEdit()
        {
            if (prevColumns != (int)columns.value) ChangeColumns();
            if (prevRows != (int)rows.value) ChangeRows();
        }

        private void ChangeColumns()
        {
            int diff = (int)columns.value - prevColumns;
            if (diff > 0)
            {
                int newPoints = diff * (basePointCount - 1);
                for (int i = 0; i < newPoints; ++i)
                {
                    var twinPoint = splineComputer.GetPoint(currentPointCount - basePointCount + 1);
                    var newVector = new Vector3(twinPoint.position.x + width, twinPoint.position.y, twinPoint.position.z);
                    Debug.Log(newVector.x + " " + newVector.y + " " + newVector.z);
                    splineComputer.SetPointPosition(currentPointCount, new Vector3(twinPoint.position.x + width, twinPoint.position.y, twinPoint.position.z));
                    currentPointCount++;
                }
            }
            prevColumns = (int)columns.value;
        }

        private void ChangeRows()
        {
            int diff = (int)rows.value - prevRows;

        }
    }
}

