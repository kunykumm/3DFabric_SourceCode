using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SplinePower
{
    public class BasicKnot : MonoBehaviour
    {

        private SplineFormer splineFormer;
        private int prevAngle;
        private float prevWidth;
        private float prevDetail;
        private double yRatio;
        private double zRatio;
        
        public GameObject baseObject;
        public Slider angleSlider;
        public Slider widthSlider;
        public Slider detailSlider;

        void Awake()
        {
            splineFormer = baseObject.GetComponent<SplineFormer>();
            prevAngle = (int)angleSlider.value;
            prevWidth = widthSlider.value;
            prevDetail = detailSlider.value;
            yRatio = (1.25 - 0.7) / angleSlider.maxValue;    // (max final position - min start position) / slider size
            zRatio = 1.75 / angleSlider.maxValue;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEdit()
        {
            if ((int)angleSlider.value != prevAngle) ChangeAngle();
        }

        private void ChangeAngle()
        {
            int difference = (int)angleSlider.value - prevAngle;
            //splineFormer.Nodes[1].Position + new Vector3(0, difference * (float)yRatio, difference * (float)zRatio);
            Debug.Log(splineFormer.Nodes[1].LocalPosition);
            Debug.Log(splineFormer.Nodes[1].LocalPosition + new Vector3(0, difference * (float)yRatio, difference * (float)zRatio));
            splineFormer.Nodes[1].RecalculateLocalPosition(splineFormer.Nodes[1].LocalPosition + new Vector3(0, difference * (float)yRatio, difference * (float)zRatio));
            //splineFormer.Nodes[1].RecalculateLocalPosition();
            //splineFormer.RecalculateValues();
            //splineFormer.InvalidateMesh();
            splineFormer.RebuildAllGroups();


            prevAngle = (int)angleSlider.value;
        }
    }
}
