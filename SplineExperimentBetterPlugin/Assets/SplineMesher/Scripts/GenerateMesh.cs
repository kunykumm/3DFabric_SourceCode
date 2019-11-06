using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SplineMesher
{
    public class GenerateMesh : MonoBehaviour
    {
        private LineManager lineMgrComp;
        private SplineMesher spMeshComp;
        private bool bezierCurve;
        private Material matForMesh;
        private List<Vector3> knotList;
        private List<Vector3> vectorList;

        public GameObject baseElement;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
