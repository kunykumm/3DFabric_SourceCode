using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SplinePower.Demos.Scripts
{
    [RequireComponent(typeof (LineRenderer))]
    class WaypointMap : MonoBehaviour
    {
        [SerializeField] private Material _lineMaterial;
        [SerializeField] private Material _dotsMaterial;
        [SerializeField] private SplineFormer _splineFormer;
        private LineRenderer _lineRenderer;

        [SerializeField] private List<KartPlace> _places;
        [SerializeField] private TextMesh _text;
        [SerializeField] private Transform _player;

        private void Start()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.material = Instantiate(_lineMaterial);
            var points =
                _splineFormer.WaypointsModule.Waypoints
                    .Select(p => ProjectPoint(p.Position))
                    .ToArray();
            _lineRenderer.SetVertexCount(points.Length + 1);
            _lineRenderer.SetPositions(points);
            _lineRenderer.SetPosition(points.Length, points[0]);

            foreach (KartPlace kartPlace in _places)
            {
                kartPlace.Dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                kartPlace.Dot.name = kartPlace.KartTransform.name + "LR";
                kartPlace.Dot.transform.SetParent(transform, false);

                kartPlace.Material = Instantiate(_dotsMaterial);
                kartPlace.Material.color = kartPlace.Color;
                kartPlace.Dot.GetComponent<MeshRenderer>().material = kartPlace.Material;
            }
        }

        private void Update()
        {
            var sortedPlaces = _places.OrderBy(p => p.Laps).ThenBy(p => p.Data.PathDistance).Reverse().ToList();
            var leader = sortedPlaces.First();

            foreach (KartPlace kartPlace in _places)
            {
                kartPlace.Data = _splineFormer.WaypointsModule.GetPathIntersectionData(
                    kartPlace.KartTransform.position,
                    kartPlace.Data);
                kartPlace.Laps += kartPlace.Data.LoopIncrement;

                float dotSize = 3f;
                if (kartPlace == leader)
                {
                    dotSize = 4f;
                }

                kartPlace.Dot.transform.localScale = Vector3.one * dotSize;
                kartPlace.Dot.transform.localPosition = ProjectPoint(kartPlace.Data.ClosestPoint);

                if (kartPlace.KartTransform == _player)
                {
                    var placesText = String.Join("\n",
                        sortedPlaces
                        .Select((p, i) => String.Format("{0}:{1}", i + 1, p.KartTransform.name))
                        .ToArray());

                    _text.text = String.Format("Lap: {0}\n" + placesText, kartPlace.Laps);
                }
            }
        }

        private Vector3 ProjectPoint(Vector3 point)
        {
            return new Vector3(point.x, point.z, 0);
        }
    }

    [Serializable]
    class KartPlace
    {
        public Transform KartTransform;
        public Color Color;
        public PathIntersectionData Data;
        public int Laps;
        public GameObject Dot { get; set; }
        public Material Material { get; set; }
    }
}