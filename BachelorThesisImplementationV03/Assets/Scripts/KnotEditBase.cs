using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Dreamteck.Splines
{
    public class KnotEditBase : MonoBehaviour
    {
        public TubeGenerator tubeGenerator;
        public SplineComputer splineComputer;

        public Slider width;
        public Slider detail;

        public Text lineWidth;
        public Text realWidth;
        public Text realHeight;

        public int rotationWhenGenerated;
        public string sceneName;

        protected float prevWidth;
        protected float prevDetail;

        private void Start()
        {
            PrepareValues();
            ChangeWidth();
        }

        protected void PrepareValues()
        {
            PlayerPrefs.SetString("scene", sceneName);

            prevWidth = width.value;
            prevDetail = detail.value;

            lineWidth.text = prevWidth.ToString("0.00");
            realWidth.text = "0,00";
            realHeight.text = "0,00";
        }

        public void OnEdit()
        {
            if (prevWidth != width.value) ChangeWidth();
            if (prevDetail != detail.value) ChangeDetail();
        }

        protected void ChangeWidth()
        {
            prevWidth = width.value;
            for (int i = 0; i < splineComputer.pointCount; ++i)
            {
                splineComputer.SetPointSize(i, prevWidth);
            }
            lineWidth.text = prevWidth.ToString("0.00");
        }

        protected void ChangeDetail()
        {
            prevDetail = detail.value;
            tubeGenerator.sides = (int)prevDetail;
        }
    }
}
