using UnityEngine;
using System.Collections;

public class WaypointRider : MonoBehaviour
{
    [SerializeField] private SplineFormer _splineFormer;

    public SplineFormer SplineFormer
    {
        get { return _splineFormer; }
        set
        {
            if (_splineFormer != value)
            {
                _splineFormer = value;
                Initialize();
            }
        }
    }

    [SerializeField] private float _speed;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    private Waypoint _currentWaypoint;
    private float _partToNextNode = 0;
    private float _lastSpeed;

    public Waypoint NextWaypoint
    {
        get
        {
            if (_currentWaypoint == null) return null;
            if (_speed > 0)
            {
                return _currentWaypoint.Next;
            }
            else
            {
                return _currentWaypoint.Previous;
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_splineFormer == null || !_splineFormer.WaypointsModule.Enabled || _splineFormer.WaypointsModule.WaypointsNumber < 2)
        {
            _currentWaypoint = null;
            return;
        }

        _currentWaypoint = _splineFormer.WaypointsModule.GetClosestWaypoint(transform);

        if(_currentWaypoint == null) return;
        if (NextWaypoint == null) return;

        var path = NextWaypoint.Position - _currentWaypoint.Position;
        var dir = transform.position - _currentWaypoint.Position;
        var proj = Vector3.Project(dir, path);
        if (Vector3.Dot(path, proj) > 0)
        {
            _partToNextNode = proj.magnitude / path.magnitude;
        }
        else if (_currentWaypoint.Previous != null)
        {

        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_splineFormer == null || !_splineFormer.WaypointsModule.Enabled || _splineFormer.WaypointsModule.WaypointsNumber < 2 || _currentWaypoint == null)
        {
            return;
        }

        if ((_speed > 0) != (_lastSpeed > 0) && _currentWaypoint != null)
        {
            if (_speed < 0 && _currentWaypoint.Next != null)
            {
                _currentWaypoint = _currentWaypoint.Next;
            }
            else if (_currentWaypoint.Previous != null)
            {
                _currentWaypoint = _currentWaypoint.Previous;
            }
            _partToNextNode = 1 - _partToNextNode;
        }
        _lastSpeed = _speed;

        if (NextWaypoint == null)
        {
            return;
        }

        var path = NextWaypoint.Position - _currentWaypoint.Position;
        _partToNextNode += Mathf.Abs(_speed) * Time.deltaTime / path.magnitude;

        while (_partToNextNode > 1)
        {
            _currentWaypoint = NextWaypoint;
            
            if (NextWaypoint == null)
            {
                _partToNextNode = 1;
                return;
            }
            else
            {
                path = NextWaypoint.Position - _currentWaypoint.Position;
                _partToNextNode -= 1;
            }
        }

        transform.position = _currentWaypoint.Position + _partToNextNode * path;
    }
}