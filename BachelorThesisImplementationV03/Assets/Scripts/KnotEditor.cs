using UnityEditor;
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

        public Text lineWidth;
        public Text realWidth;
        public Text realHeight;

        public int rotationWhenGenerated;

        private float prevAngle;
        private float prevWidth;
        private float prevDetail;

        private float xDiff;
        private float zDiff;
        private Vector3 firstPointStart;
        private Vector3 secondPointStart;

        /**
         * ANGLES
         * Start position:   3.0  3.0  0.8
         *                   0.0  3.0 -0.8
         * End position:     1.5  3.0  1.6
         *                   1.5  3.0 -1.6
         *                   
         * ALL NORMALS: At average center - best result when angle changes
         */

        // Start is called before the first frame update

        private void Start()
        {
            //LoadPreviousKnot();

            prevWidth = width.value;
            prevDetail = detail.value;

            lineWidth.text = prevWidth.ToString("0.00");
            realWidth.text = "0,00";
            realHeight.text = "0,00";

            if (angle != null) 
            {
                prevAngle = angle.value;
                CalculateBaseValues(); 
            }
            ChangeWidth();
        }

        private void LoadPreviousKnot()
        {
            var before = (GameObject)Resources.Load("Knot");
            if (before.GetComponent<SplineComputer>().pointCount > 0)
            {
                var points = before.GetComponent<SplineComputer>().GetPoints();
                splineComputer.SetPoints(points);
                tubeGenerator.sides = before.GetComponent<TubeGenerator>().sides;
                FillSlidersAndTextsWithData();
                //krivka nie je ako má byť, ale po zmene uhlu je všetko v pohode
                before.GetComponent<SplineComputer>().SetPoints(new SplinePoint[] { });
            }
        }

        private void FillSlidersAndTextsWithData()
        {
            if (angle != null) angle.value = PlayerPrefs.GetFloat("angle");
            width.value = PlayerPrefs.GetFloat("width");
            detail.value = PlayerPrefs.GetFloat("detail");

            lineWidth.text = PlayerPrefs.GetString("lWidth");
            realWidth.text = PlayerPrefs.GetString("rWidth");
            realHeight.text = PlayerPrefs.GetString("rHeight");

            rotationWhenGenerated = PlayerPrefs.GetInt("rotation");
        }

        private void CalculateBaseValues()
        {
            firstPointStart = new Vector3(2.6f, 3.0f, 0.8f);
            secondPointStart = new Vector3(0.4f, 3.0f, -0.8f);
            xDiff = (firstPointStart.x - 1.5f) / angle.maxValue;
            zDiff = (firstPointStart.z - 1.6f) / angle.maxValue;
        }

        public void OnEdit()
        {
            if (angle != null && prevAngle != angle.value) ChangeAngle();
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
            for (int i = 0; i < splineComputer.pointCount; ++i)
            {
                splineComputer.SetPointSize(i, prevWidth);
            }
            lineWidth.text = prevWidth.ToString("0.00");
        }

        private void ChangeDetail()
        {
            prevDetail = detail.value;
            tubeGenerator.sides = (int)prevDetail;
        }
    }
}
