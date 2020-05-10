using UnityEngine;
using UnityEngine.UI;

namespace Dreamteck.Splines
{
    /// <summary>
    /// Contains all necessary functions to edit single uncovered element.
    /// </summary>
    public class KnotEditBase : MonoBehaviour
    {
        /// <value> TubeGenerator in Dreamteack Splines </value>
        public TubeGenerator tubeGenerator;
        /// <value> SplineComputer in Dreamteack Splines </value>
        public SplineComputer splineComputer;

        public Slider width;
        public Slider detail;
        public SizeChanger sizeChanger;

        protected float prevWidth;
        protected float prevDetail;

        /// <summary>
        /// Sets up the single element when scene starts.
        /// </summary>
        private void Start()
        {
            PrepareValues();
            ChangeWidth();
        }

        /// <summary>
        /// Sets the current values of width and detail.
        /// Sets attributes in sizeChanger for size calculations.
        /// </summary>
        protected void PrepareValues()
        {
            prevWidth = width.value;
            prevDetail = detail.value;

            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            sizeChanger.SetHeight(bounds.size.y);
            sizeChanger.SetWidth(bounds.size.x);

            sizeChanger.SetLineWidth(prevWidth);
            sizeChanger.UpdateTriangleCount(tubeGenerator.GetTriangleCount());
            sizeChanger.ChangeSizesNet();
        }

        /// <summary>
        /// Called when values of sliders change.
        /// The width and detail are updated according to this change.
        /// </summary>
        public void OnEdit()
        {
            if (prevWidth != width.value) ChangeWidth();
            if (prevDetail != detail.value) ChangeDetail();
        }

        /// <summary>
        /// Changes the fibre diameter to the current width value.
        /// Size of all points needs to be rewritten.
        /// 
        /// Important:
        /// <code>
        ///    float realValue = prevWidth / sizeChanger.GetCurrentScale() * (5f / 4.523f);
        /// </code>
        /// This line of code scales the fibre diameter to the right size. 
        /// The approximate constant (5f / 4.523f) is necessary to get the closest real value. 
        /// For example, the width of Dreamteck Splines shows 0.5cm, but in reality it is just around 0.45cm.
        /// </summary>
        protected void ChangeWidth()
        {
            prevWidth = width.value;
            float realValue = prevWidth / sizeChanger.GetCurrentScale() * (5f / 4.523f);
            for (int i = 0; i < splineComputer.pointCount; ++i)
            {
                splineComputer.SetPointSize(i, realValue);
            }
            sizeChanger.UpdateFromSlider(prevWidth);
        }

        /// <summary>
        /// Changes the number of segments of the cross-section to get smooth/rough surface.
        /// </summary>
        protected void ChangeDetail()
        {
            prevDetail = detail.value;
            tubeGenerator.sides = (int)prevDetail;
            sizeChanger.UpdateTriangleCount(tubeGenerator.GetTriangleCount());
        }
    }
}
