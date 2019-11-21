using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.SplinePower.Demos.Scripts
{
    class ProceduralSpline : MonoBehaviour
    {
        [Range(2, 30)]
        [SerializeField] private int _nodesNumber = 4;

        [Range(0.1f,0.5f)]
        [SerializeField] private float _step = 0.25f;

        [Range(1,10)]
        [SerializeField] private float _scale = 5f;

        private SplineFormer _sf;

        public void Start()
        {
            UpdateNodes();
        }

        private void OnValidate()
        {
            if(!Application.isPlaying) return;
            UpdateNodes();            
        }

        private void UpdateNodes()
        {
            if (_sf == null) _sf = GetComponent<SplineFormer>();
            //if _sf.Nodes.Count>_nodesNumber remove redundant nodes
            for (int i = _nodesNumber; i < _sf.Nodes.Count; i++)
            {
                var node = _sf.Nodes[i];
                _sf.RemoveNodeImmediately(node);
                Destroy(node.gameObject);
            }

            //if _nodesNumber>_sf.Nodes.Count add new nodes
            for (int i = _sf.Nodes.Count; i < _nodesNumber; i++)
            {
                _sf.AddNodeImmediately();
            }

            //setting the position of each node
            for (int i = 0; i < _sf.Nodes.Count; i++)
            {
                float t = i * Mathf.PI * _step;
                _sf.Nodes[i].transform.position = _scale * new Vector3(Mathf.Sin(t), 0, t);
            }

            _sf.InvalidateMesh();
        }
    }
}