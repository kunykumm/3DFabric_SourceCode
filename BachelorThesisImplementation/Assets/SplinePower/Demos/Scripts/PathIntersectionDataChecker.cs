using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SplinePower.Demos.Scripts
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    class PathIntersectionDataChecker : MonoBehaviour
    {
        [SerializeField] private SplineFormer _splineFormer;
        private PathIntersectionData _data;
        [SerializeField] private Material _lineMaterial;

        public Vector3 ClosestPoint
        {
            get { return _data.ClosestPoint; }
        }

        private LineRenderer _lineRenderer;
        public LineRenderer LineRenderer
        {
            get
            {
                if (_lineRenderer != null) return _lineRenderer;
                if (_lineRenderer == null) _lineRenderer = gameObject.GetComponent<LineRenderer>();
                if (_lineRenderer == null) _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.useWorldSpace = false;
                _lineRenderer.SetColors(Color.red, Color.red);
                _lineRenderer.SetWidth(0.1f, 0.1f);
                _lineRenderer.material = Instantiate(_lineMaterial);
                return _lineRenderer;
            }
        }

        public void Update()
        {
            var position = transform.position;
            _data = _splineFormer.WaypointsModule.GetPathIntersectionData(position, _data);
            Debug.DrawRay(position, _data.VectorToPath, Color.red);
            Debug.DrawRay(_data.Waypoints.Behind.Position, _data.Projection, Color.cyan);

            LineRenderer.SetPositions(new[] {Vector3.zero, _data.VectorToPath});

            if(!Application.isPlaying) return;

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance = 0;
            if (plane.Raycast(ray, out rayDistance))
            {
                transform.position = ray.GetPoint(rayDistance);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_data.ClosestPoint, 0.1f);
            Gizmos.color = Color.cyan;
            if (_data.Waypoints.Behind != null) Gizmos.DrawSphere(_data.Waypoints.Behind.Position, 0.1f);
            if (_data.Waypoints.InFront != null) Gizmos.DrawSphere(_data.Waypoints.InFront.Position, 0.1f);
        }

        public void OnGUI()
        {
            var position = Camera.main.WorldToScreenPoint(transform.position);
            position.y = Screen.height - position.y;
            var text = _data.ToString();
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), "label");
            rect.position = position;
            GUI.Box(rect, "");
            GUI.Label(rect, text);
        }
    }
}