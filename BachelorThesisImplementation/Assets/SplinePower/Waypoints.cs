using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


[Serializable]
public class WaypointsModule
{
    public bool Enabled;
    public int WaypointsNumber = 3;
    public Vector3 Shift;
    public bool DrawPath;
    public bool Loop;

    private List<Waypoint> _waypoints = new List<Waypoint>();
    private float _pathLength;
    private bool _loop;
    private Waypoint _first;
    private Waypoint _last;

    public List<Waypoint> Waypoints { get { return _waypoints; } }

    public float PathLength { get { return _pathLength; } }

    public void RecalculateWaypoints(SplineFormer splineFormer)
    {
        if (!Enabled)
        {
            _waypoints.Clear();
            return;
        }

        WaypointsNumber = Mathf.Clamp(WaypointsNumber, 2, 100);
        var pointNumber = _loop ? WaypointsNumber + 1 : WaypointsNumber;
        var points = splineFormer.GetSplinePoints(pointNumber);
        _loop = splineFormer.Loop || Loop;
        if (_loop)
        {
            Array.Resize(ref points, points.Length - 1);
        }

        UpdateWaypoints(points);

        if (_waypoints.Count <= 0) return;

        _first = _waypoints.First();
        _last = _waypoints.Last();
        if (_loop)
        {
            _first.Previous = _last;
            _last.Next = _first;
        }
        else
        {
            _first.Previous = null;
            _last.Next = null;
        }

        Vector3[] shifts = new Vector3[_waypoints.Count];
        for (int i = 0; i < _waypoints.Count; i++)
        {
            var wp = _waypoints[i];
            if (wp.Next != null)
            {
                shifts[i] = Quaternion.LookRotation(wp.Next.Position - wp.Position) * Shift;
            }
            else if (wp.Previous != null)
            {
                shifts[i] = Quaternion.LookRotation(wp.Position - wp.Previous.Position) * Shift;
            }
        }

        for (int i = 0; i < _waypoints.Count; i++)
        {
            _waypoints[i].Position += shifts[i];
            _waypoints[i].Number = i;
        }

        _pathLength = 0;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            var waypoint = _waypoints[i];
            waypoint.DistancePosition = _pathLength;
            if (waypoint.Next == null) break;
            _pathLength += Vector3.Distance(waypoint.Position, waypoint.Next.Position);
        }
    }

    private void UpdateWaypoints(Vector3[] points)
    {
        int min = Mathf.Min(points.Length, _waypoints.Count);
        int toRemove = _waypoints.Count - min;
        int toAdd = points.Length - min;

        for (int i = 0; i < toAdd; i++)
        {
            _waypoints.Add(new Waypoint());
        }

        if (toRemove > 0)
        {
            _waypoints.RemoveRange(min, toRemove);
        }

        for (int i = 0; i < _waypoints.Count; i++)
        {
            _waypoints[i].Position = points[i];
            if (i < _waypoints.Count - 1) _waypoints[i].Next = _waypoints[i + 1];
            if (i > 0) _waypoints[i].Previous = _waypoints[i - 1];
        }
    }

    public void DrawGizmos()
    {
        if (!Enabled) return;
        if (!DrawPath) return;

        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (_waypoints[i].Next != null)
            {
                DrawArrow(
                    _waypoints[i].Position,
                    _waypoints[i].Next.Position - _waypoints[i].Position
                    );
            }
        }
    }

    private static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f,
        float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    public WaypointPair GetWaypointsAtPart(float part)
    {
        return GetWaypointsAtDistance(part * _pathLength);
    }

    public Vector3 GetPointAtDistance(float distancePosition)
    {
        if (_waypoints.Count <= 0) return Vector3.zero;
        distancePosition = NormalizeDistancePosition(distancePosition);

        var waypoints = GetWaypointsAtDistance(distancePosition);

        if (waypoints.Behind == null && waypoints.InFront == null) return Vector3.zero;
        if (waypoints.Behind == null && waypoints.InFront != null) return waypoints.InFront.Position;
        if (waypoints.Behind != null && waypoints.InFront == null) return waypoints.Behind.Position;

        var t = (distancePosition - waypoints.Behind.DistancePosition) / waypoints.DistanceBetween;
        return Vector3.Lerp(waypoints.Behind.Position, waypoints.InFront.Position, t);
    }

    public WaypointPair GetWaypointsAtDistance(float distancePosition)
    {
        if (_waypoints.Count <= 0) return new WaypointPair(null, null);
        distancePosition = NormalizeDistancePosition(distancePosition);

        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (_waypoints[i].DistancePosition > distancePosition)
            {
                return new WaypointPair(_waypoints[i].Previous, _waypoints[i]);
            }
        }

        return new WaypointPair(_last, _last.Next);
    }

    private float NormalizeDistancePosition(float distancePosition)
    {
        if (_loop)
        {
            distancePosition = distancePosition % _pathLength;
            while (distancePosition < 0) distancePosition = _pathLength + distancePosition;
        }
        else
        {
            distancePosition = Mathf.Clamp(distancePosition, 0, _pathLength);
        }

        return distancePosition;
    }

    public float GetDistanceAtPoint(Vector3 position)
    {
        return GetPathIntersectionData(position, null, new WaypointPair()).PathDistance;
    }

    public PathIntersectionData GetPathIntersectionData(
        Vector3 position,
        PathIntersectionData previousData)
    {
        return GetPathIntersectionData(position, previousData.ClosestWaypoint, previousData.Waypoints);
    }

    public PathIntersectionData GetPathIntersectionData(
        Vector3 position,
        Waypoint previousClosestWaypoint,
        WaypointPair previousWaypointPair)
    {
        if (_waypoints.Count <= 0)
        {
            return new PathIntersectionData();
        }

        Waypoint closestWaypoint = null;

        if (previousClosestWaypoint == null)
        {
            closestWaypoint = GetClosestWaypoint(position);
        }
        else
        {
            closestWaypoint = GetClosestWaypoint(
                position,
                previousClosestWaypoint.Number - 1,
                previousClosestWaypoint.Number + 1);
        }

        Vector3 closestVectorToPath = closestWaypoint.Position - position;
        float closestDistanceToPath = closestVectorToPath.magnitude;
        Waypoint closestBehind = closestWaypoint;
        Waypoint closestInFront = closestWaypoint.Next;

        Vector3 closestPoint = closestWaypoint.Position;
        Vector3 closestOnNormal = closestInFront != null
            ? closestInFront.Position - closestBehind.Position
            : Vector3.zero; //null REF
        Vector3 closestProjection = Vector3.zero;
        float closestPathDistance = closestBehind.DistancePosition;

        int minIndex = closestWaypoint.Number - 1;
        int maxIndex = closestWaypoint.Number + 1;

        if (!_loop)
        {
            minIndex = Mathf.Clamp(minIndex, 0, _waypoints.Count - 2);
            maxIndex = Mathf.Clamp(maxIndex, 0, _waypoints.Count - 2);
        }

        for (int index = minIndex; index <= maxIndex; index++)
        {
            int i = NormalizeWaypointIndex(index);

            var behind = _waypoints[i];
            var inFront = behind.Next;

            var vector = position - behind.Position;
            var onNormal = inFront.Position - behind.Position;

            var projection = Vector3.Project(vector, onNormal);
            if (projection.sqrMagnitude > onNormal.sqrMagnitude || Vector3.Dot(projection, onNormal) < 0)
            {
                continue; //out of normal
            }

            //vector + VectorToPath = Projection
            var vectorToPath = projection - vector;
            var distanceToPath = vectorToPath.magnitude;

            if (distanceToPath < closestDistanceToPath)
            {
                closestDistanceToPath = distanceToPath;
                closestBehind = behind;
                closestInFront = inFront;
                closestVectorToPath = vectorToPath;
                closestPoint = behind.Position + projection;
                closestPathDistance = closestBehind.DistancePosition + projection.magnitude;
                closestOnNormal = onNormal;
                closestProjection = projection;
            }
        }

        var waypoints = new WaypointPair(closestBehind, closestInFront);

        var loopIncrement = 0;
        if (previousWaypointPair.InFront == _waypoints[0] && waypoints.InFront == _waypoints[1])
        {
            loopIncrement = 1;
        }
        else if (previousWaypointPair.InFront == _waypoints[1] && waypoints.InFront == _waypoints[0])
        {
            loopIncrement = -1;
        }
        bool wrongWay = false;
        if (previousWaypointPair.Behind != null && previousWaypointPair.Behind == waypoints.InFront)
        {
            wrongWay = true;
        }

        return new PathIntersectionData(
            waypoints,
            closestWaypoint,
            closestPoint,
            closestPathDistance,
            closestDistanceToPath,
            closestProjection,
            closestOnNormal,
            closestVectorToPath,
            loopIncrement,
            wrongWay);
    }

    public int NormalizeWaypointIndex(int index)
    {
        if (_loop)
        {
            index = index % _waypoints.Count;
            if (index < 0) index = _waypoints.Count + index;
        }
        else
        {
            index = Mathf.Clamp(index, 0, _waypoints.Count - 1);
        }
        return index;
    }

    public Waypoint GetClosestWaypoint(Transform transform)
    {
        return GetClosestWaypoint(transform.position);
    }

    public Waypoint GetClosestWaypoint(Vector3 position)
    {
        return GetClosestWaypoint(position, 0, _waypoints.Count - 1);
    }

    public Waypoint GetClosestWaypoint(Vector3 position, int minIndex, int maxIndex)
    {
        if (_waypoints.Count <= 0) return null;

        Waypoint waypoint = null;
        float minDist = Single.MaxValue;
        if (!_loop)
        {
            minIndex = Mathf.Clamp(minIndex, 0, _waypoints.Count - 1);
            maxIndex = Mathf.Clamp(maxIndex, 0, _waypoints.Count - 1);
        }

        for (int index = minIndex; index <= maxIndex; index++)
        {
            int i = NormalizeWaypointIndex(index);

            float newDist = Vector3.SqrMagnitude(_waypoints[i].Position - position);
            if (newDist < minDist)
            {
                minDist = newDist;
                waypoint = _waypoints[i];
            }
        }
        return waypoint;
    }
}

public struct PathIntersectionData
{
    public readonly WaypointPair Waypoints;
    public readonly Waypoint ClosestWaypoint;
    public readonly Vector3 ClosestPoint;
    public readonly float PathDistance;
    public readonly float DistanceToPath;
    public readonly Vector3 Projection;
    public readonly Vector3 VectorToPath;
    public readonly int LoopIncrement;
    public readonly bool WrongWay;
    public readonly Vector3 PathDirection;

    public PathIntersectionData(WaypointPair waypoints, Waypoint closestWaypoint, Vector3 closestPoint, float pathDistance, float distanceToPath, Vector3 projection, Vector3 pathDirection, Vector3 vectorToPath, int loopIncrement, bool wrongWay)
    {
        Waypoints = waypoints;
        ClosestWaypoint = closestWaypoint;
        ClosestPoint = closestPoint;
        PathDistance = pathDistance;
        DistanceToPath = distanceToPath;
        Projection = projection;
        VectorToPath = vectorToPath;
        LoopIncrement = loopIncrement;
        WrongWay = wrongWay;
        PathDirection = pathDirection;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Waypoints:");
        sb.AppendLine(Waypoints.ToString());
        sb.Append("ClosestWaypoint:");
        sb.AppendLine(ClosestWaypoint == null ? "null" : ClosestWaypoint.ToString());
        sb.Append("PathDistance:");
        sb.AppendLine(PathDistance.ToString("F"));
        sb.Append("DistanceToPath:");
        sb.AppendLine(DistanceToPath.ToString("F"));
        sb.Append("Projection:");
        sb.AppendLine(Projection.ToString());
        sb.Append("VectorToPath:");
        sb.AppendLine(VectorToPath.ToString());
        sb.Append("PathDirection:");
        sb.Append(PathDirection.ToString());
        return sb.ToString();
    }
}

public struct WaypointPair
{
    public readonly Waypoint Behind;
    public readonly Waypoint InFront;

    public float DistanceBetween { get { return (InFront.Position - Behind.Position).magnitude; } }

    public WaypointPair(Waypoint behind, Waypoint inFront) : this()
    {
        Behind = behind;
        InFront = inFront;
    }

    public Waypoint GetClosest(Vector3 position)
    {
        if (Behind == null && InFront == null) return null;
        if (Behind != null && InFront == null) return Behind;
        if (Behind == null && InFront != null) return InFront;

        if ((position - Behind.Position).sqrMagnitude < (position - InFront.Position).sqrMagnitude)
        {
            return Behind;
        }
        return InFront;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        sb.Append(Behind == null ? "null" : Behind.Number.ToString());
        sb.Append(";");
        sb.Append(InFront == null ? "null" : InFront.Number.ToString());
        sb.Append("}");
        return sb.ToString();
    }
}

public class Waypoint
{
    public Vector3 Position;
    public Waypoint Next;
    public Waypoint Previous;
    public float DistancePosition;
    public int Number;

    public override string ToString()
    {
        return Number.ToString();
    }
}