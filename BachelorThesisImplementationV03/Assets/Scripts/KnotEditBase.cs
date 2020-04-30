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

        private void Start()
        {
            PrepareValues();
            ChangeWidth();
        }

        protected void PrepareValues()
        {
            prevWidth = width.value;
            prevDetail = detail.value;

            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            rHeight = bounds.size.y;
            rWidth = bounds.size.x;

            sizeChanger.SetHeight(rHeight);
            sizeChanger.SetWidth(rWidth);
            sizeChanger.SetLineWidth(prevWidth);
            sizeChanger.UpdateTriangleCount(tubeGenerator.GetTriangleCount());
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
            sizeChanger.UpdateTriangleCount(tubeGenerator.GetTriangleCount());
        }
    }
}
