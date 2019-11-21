using UnityEngine;

namespace SplinePower.Demos.Scripts
{
    [ExecuteInEditMode]
    class Follower:MonoBehaviour
    {
        [SerializeField] private SplineFormer _splineFormer;
        [SerializeField] private PathIntersectionDataChecker _target;
        [SerializeField] private float _shift;

        private void Update()
        {
            if(_splineFormer == null || _target == null) return;
            var distance = _splineFormer.WaypointsModule.GetDistanceAtPoint(_target.ClosestPoint);
            distance += _shift;
            transform.position = _splineFormer.WaypointsModule.GetPointAtDistance(distance);
        }
    }
}
