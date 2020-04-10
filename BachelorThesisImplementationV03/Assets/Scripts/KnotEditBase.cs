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
        public SizeChanger sizeChanger;

        public string sceneName;

        protected float prevWidth;
        protected float prevDetail;

        private float rWidth;
        private float rHeight;
        private KnotUtility knotUti;

        private void Start()
        {
            PrepareValues();
            ChangeWidth();
        }

        protected void PrepareValues()
        {
            prevWidth = width.value;
            prevDetail = detail.value;

            knotUti = new KnotUtility();
            knotUti.FindMaxsMins(ref rHeight, ref rWidth, splineComputer.GetPoints());

            sizeChanger.SetHeight(rHeight);
            sizeChanger.SetWidth(rWidth);
            sizeChanger.SetLineWidth(prevWidth);
            sizeChanger.ChangeSizesNet();
        }

        public void OnEdit()
        {
            if (prevWidth != width.value) ChangeWidth();
            if (prevDetail != detail.value) ChangeDetail();
        }

        protected void ChangeWidth()
        {
            prevWidth = width.value;
            float realValue = prevWidth / sizeChanger.GetCurrentScale();
            for (int i = 0; i < splineComputer.pointCount; ++i)
            {
                splineComputer.SetPointSize(i, realValue);
            }
            sizeChanger.UpdateFromSlider(prevWidth);
        }

        protected void ChangeDetail()
        {
            prevDetail = detail.value;
            tubeGenerator.sides = (int)prevDetail;
        }
    }
}
