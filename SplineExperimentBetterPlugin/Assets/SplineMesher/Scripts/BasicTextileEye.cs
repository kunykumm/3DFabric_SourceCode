using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SplineMesher
{
    public class BasicTextileEye : MonoBehaviour
    {
        private LineManager lineMgrComp;
        private SplineMesher spMeshComp;
        private GameObject basicEye;
        private float prevAngle;

        public bool bezierCurve;
        public Material matForMesh;
        public Slider angleChanger;
        public Slider widthChanger;
        public Slider detailChanger;
        public List<Vector3> knotList;
        public List<Vector3> vectorList;

        // Camera coordinates> 1.7850, 2.160001, -8.849875

        private void GameObjectInit()
        {
            prevAngle = 0f;

            basicEye = new GameObject();
            basicEye.name = "basic eye";

            lineMgrComp = basicEye.AddComponent<LineManager>();
            spMeshComp = basicEye.AddComponent<SplineMesher>();
        }

        private void BasicEyeInit()
        {
            for (int i = 0; i < 3; i++)
            {
                lineMgrComp.knotNum++;
            }
            lineMgrComp.ManualUpdate();

            knotList = lineMgrComp.GetLineKnots();

            knotList[0] = new Vector3(0, 0, 0);
            knotList[1] = new Vector3(2.8f, 3, 0);
            knotList[2] = new Vector3(1.6f, 4.5f, 0);
            knotList[3] = new Vector3(0.4f, 3, 0);
            knotList[4] = new Vector3(3.2f, 0, 0);

            vectorList = lineMgrComp.GetControllerList();
            vectorList.Clear();
            vectorList.Add(new Vector3(1, 0, 0));
            vectorList.Add(new Vector3(2.8f, 2.288531f, 0));
            vectorList.Add(new Vector3(2.8f, 3.711469f, 0));
            vectorList.Add(new Vector3(2.5f, 4.483798f, 0));
            vectorList.Add(new Vector3(0.5f, 4.483798f, 0));
            vectorList.Add(new Vector3(0.4f, 3.711469f, 0));
            vectorList.Add(new Vector3(0.4f, 2.288531f, 0));
            vectorList.Add(new Vector3(2, 0, 0));

            lineMgrComp.bezierCurve = true;

            spMeshComp.mat = matForMesh;

            spMeshComp.isRectangle = false;
            spMeshComp.tubeRadius = 0.2f;
            spMeshComp.sides = 24;

            detailChanger.value = 24;
            widthChanger.value = 0.2f;

            lineMgrComp.ManualUpdate();

            ChangeAngle();
        }

        void Awake()
        {
            GameObjectInit();
            BasicEyeInit();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void ChangeAngle()
        {
            float sliderMax = 2.5f;

            //up node
            vectorList[3] = new Vector3(2.5f - (float)(angleChanger.value / sliderMax * 0.9), vectorList[3].y, (float)(angleChanger.value / sliderMax * 1.25));
            vectorList[4] = new Vector3(0.5f + (float)(angleChanger.value / sliderMax * 1.1), vectorList[4].y, 0 - (float)(angleChanger.value / sliderMax * 1.25));

            //right side node
            knotList[1] = new Vector3(2.8f - (float)(angleChanger.value / sliderMax * 1.2), knotList[1].y, (float)(angleChanger.value / sliderMax * 1.2));

            vectorList[0] = new Vector3(1f + (float)(angleChanger.value / sliderMax * 0.5), vectorList[0].y, vectorList[0].z);
            vectorList[1] = new Vector3(2.8f - (float)(angleChanger.value / sliderMax * 1.21), vectorList[1].y, (float)(angleChanger.value / sliderMax * 1.21));
            vectorList[2] = new Vector3(2.8f - (float)(angleChanger.value / sliderMax * 1.19), 3.711469f - (float)(angleChanger.value / sliderMax * 0.421469), (float)(angleChanger.value / sliderMax * 1.21));

            //left side node
            knotList[3] = new Vector3(0.4f + (float)(angleChanger.value / sliderMax * 1.2), knotList[3].y, 0 - (float)(angleChanger.value / sliderMax * 1.2));

            vectorList[5] = new Vector3(0.4f + (float)(angleChanger.value / sliderMax * 1.19), 3.711469f - (float)(angleChanger.value / sliderMax * 0.421469), 0 - (float)(angleChanger.value / sliderMax * 1.21));
            vectorList[6] = new Vector3(0.4f + (float)(angleChanger.value / sliderMax * 1.21), vectorList[6].y, 0 - (float)(angleChanger.value / sliderMax * 1.21));
            vectorList[7] = new Vector3(2f - (float)(angleChanger.value / sliderMax * 0.5), vectorList[7].y, vectorList[7].z);

            lineMgrComp.ManualUpdate();
        }

        public void OnEdit()
        {
            if (prevAngle != angleChanger.value)
            {
                ChangeAngle();
                prevAngle = angleChanger.value;
            }

            if (spMeshComp.tubeRadius != widthChanger.value)
            {
                spMeshComp.tubeRadius = widthChanger.value;
                lineMgrComp.ManualUpdate();
            }

            if (spMeshComp.sides != (int)detailChanger.value)
            {
                spMeshComp.sides = (int)detailChanger.value;
                lineMgrComp.ManualUpdate();
            }
        }
    }
}
