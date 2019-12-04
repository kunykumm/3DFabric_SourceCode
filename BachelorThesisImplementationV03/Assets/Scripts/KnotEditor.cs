using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Dreamteck.Splines
{
    public class KnotEditor : MonoBehaviour
    {
        public TubeGenerator tubeGenerator;
        public SplineComputer splineComputer;

        public Slider angle;
        public Slider width;
        public Slider detail;

        private float prevAngle;
        private float prevWidth;
        private float prevDetail;

        private float xDiff;
        private float zDiff;
        private Vector3 firstPointStart;
        private Vector3 secondPointStart;

        /**
         * ANGLES
         * Start position:   3.5  3.0  0.8
         *                  -0.5  3.0 -0.8
         * End position:     1.5  3.0  1.8
         *                   1.5  3.0 -1.8
         */

        // Start is called before the first frame update
        void Start()
        {
            prevAngle = angle.value;
            prevWidth = width.value;
            prevDetail = detail.value;
            CalculateBaseValues();
            ChangeWidth();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void CalculateBaseValues()
        {
            firstPointStart = new Vector3(3.5f, 3.0f, 0.8f);
            secondPointStart = new Vector3(-0.5f, 3.0f, -0.8f);
            xDiff = (firstPointStart.x - 1.5f) / angle.maxValue;
            zDiff = (firstPointStart.z - 1.8f) / angle.maxValue;
        }

        public void OnEdit()
        {
            if (prevAngle != angle.value) ChangeAngle();
            if (prevWidth != width.value) ChangeWidth();
            if (prevDetail != detail.value) ChangeDetail();
        }

        private void ChangeAngle()
        {
            prevAngle = angle.value;
            splineComputer.SetPointPosition(1, new Vector3(firstPointStart.x - prevAngle * xDiff, firstPointStart.y, firstPointStart.z - prevAngle * zDiff));
            splineComputer.SetPointPosition(3, new Vector3(secondPointStart.x + prevAngle * xDiff, secondPointStart.y, secondPointStart.z + prevAngle * zDiff));
        }

        private void ChangeWidth()
        {
            prevWidth = width.value;
            for (int i = 0; i < 5; ++i)
            {
                splineComputer.SetPointSize(i, prevWidth);
            }
        }

        private void ChangeDetail()
        {
            prevDetail = detail.value;
            tubeGenerator.sides = (int)prevDetail;
        }
    }
}
