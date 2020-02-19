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

        public SizeChanger SizeChanger;

        public string sceneName;

        protected float prevWidth;
        protected float prevDetail;

        private float rWidth;
        private float rHeight;
        private KnotUtility knotUti;

        private void Start()
        {
            PlayerPrefs.SetString("scene", sceneName);
            PrepareValues();
            ChangeWidth();
        }

        protected void PrepareValues()
        {
            prevWidth = width.value;
            prevDetail = detail.value;

            knotUti = new KnotUtility();
            knotUti.FindMaxsMins(ref rHeight, ref rWidth, splineComputer.GetPoints());

            lineWidth.text = prevWidth.ToString("0.00");
            //realWidth.text = rWidth.ToString();
            //realHeight.text = rHeight.ToString();

            SizeChanger.setHeight(rHeight);
            SizeChanger.setWidth(rWidth);
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
