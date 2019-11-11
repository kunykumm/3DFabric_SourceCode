﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SplineMesher
{
    public class GenerateMesh : MonoBehaviour
    {
        //private bool bezierCurve;
        private Material matForMesh;
        private List<Vector3> knotList;
        private List<Vector3> vectorList;

        private GameObject baseElement;
        private float baseWidth;
        private float baseHeight;
        private int baseKnotNum;
        private int baseVectorNum;
        private int prevVectorNum;
        private LineManager lineMgrComp;
        private SplineMesher spMeshComp;

        //public GameObject meshNet;
        //private LineManager meshLineMgr;
        //private SplineMesher meshSpMesh;

        public Slider columns;
        private int prevColValue;
        public Slider rows;
        private int prevRowValue;

        public void Awake()
        {
            baseElement = GameObject.Find("EditedKnot");

            lineMgrComp = baseElement.GetComponent<LineManager>();
            spMeshComp = baseElement.GetComponent<SplineMesher>();

            vectorList = lineMgrComp.GetControllerList();

            lineMgrComp.bezierCurve = false;
            knotList = lineMgrComp.GetLineKnots();
            lineMgrComp.bezierCurve = true;

            baseKnotNum = knotList.Count;
            baseVectorNum = vectorList.Count;
            prevVectorNum = baseVectorNum;

            columns.value = 10;
            rows.value = 5;
            prevColValue = 10;
            prevRowValue = 5;

            GetDimensions();
            InitialiseColumns();
            InitialiseRows();
        }

        private void GetDimensions()
        {
            int listLength = knotList.Count;

            baseWidth = knotList[listLength - 1].x - knotList[0].x;

            float min = knotList[0].y;
            float max = knotList[0].y;

            for (int i = 1; i < listLength; ++i)
            {
                float current = knotList[i].y;
                if (current > max) max = current;
                if (current < min) min = current;
            }

            baseHeight = max - min;
        }

        // Start is called before the first frame update
        public void Start()
        {

        }

        private void InitialiseColumns()
        {
            for (int i = 1; i < columns.value; ++i)
            {
                AddNewElement();
            }
        }

        private void AddNewElement()
        {
            lineMgrComp.bezierCurve = false;
            AddKnot();
            AddVector();
            lineMgrComp.bezierCurve = true;
            lineMgrComp.ManualUpdate();
        }

        private void AddKnot()
        {
            List<Vector3> newValues = new List<Vector3>();

            int indexNum = knotList.Count;

            for (int j = baseKnotNum - 1; j > 0; --j)
            {
                newValues.Add(new Vector3(knotList[indexNum - j].x + baseWidth, knotList[indexNum - j].y, knotList[indexNum - j].z));
            }

            for (int k = 0; k < baseKnotNum - 1; ++k)
            {
                lineMgrComp.knotNum++;
                lineMgrComp.ManualUpdate();
                knotList = lineMgrComp.GetLineKnots();
                knotList[indexNum + k] = newValues[k];
                lineMgrComp.ManualUpdate();
            }
        }
        
        private void AddVector()
        {
            vectorList = lineMgrComp.GetControllerList();
            List<Vector3> oldVectors = new List<Vector3>(vectorList);
            vectorList.Clear();

            for (int i = 0; i < prevVectorNum; ++i)
            {
                vectorList.Add(oldVectors[i]);
            }
            for(int j = 0; j < baseVectorNum; ++j)
            {
                int index = vectorList.Count - baseVectorNum;
                var vector = new Vector3(vectorList[index].x + baseWidth, vectorList[index].y, vectorList[index].z);
                vectorList.Add(vector);
            }
            prevVectorNum = vectorList.Count;
        }

        private void InitialiseRows()
        {
            for (int i = 0; i < rows.value; ++i)
            {
                Instantiate(baseElement, new Vector3(0, i * baseHeight, 0), Quaternion.identity);
            }
        }

        // Update is called once per frame
        public void Update()
        {
            Debug.Log("on edit detected");
            if (columns.value != prevColValue)
            {
                Debug.Log("value changed");
                ChangeColumns();
                prevColValue = (int)columns.value;
            }
            if (rows.value != prevRowValue)
            {

            }
        }

        public void OnEdit()
        {
            Debug.Log("on edit detected");
            if (columns.value != prevColValue)
            {
                Debug.Log("value changed");
                ChangeColumns();
                prevColValue = (int)columns.value;
            }
            if (rows.value != prevRowValue)
            {

            }
        }

        private void ChangeColumns()
        {
            int diff = (int)columns.value - prevColValue;
            Debug.Log(columns.value);

            if (diff > 0)
            {
                for (int i = 0; i < diff; ++i)
                {
                    AddNewElement();
                }
                return;
            }
            diff = prevColValue - (int)columns.value;
            lineMgrComp.knotNum -= diff * baseKnotNum;
            lineMgrComp.ManualUpdate();
        }
    }
}
