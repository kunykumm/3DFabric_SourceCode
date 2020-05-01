using Dreamteck.Splines;
using System;
using UnityEngine;

public class KnotUtility
{
    public void FindMaxsMins(ref float height, ref float width, SplinePoint[] basePoints)
    {
        int basePointCount = basePoints.Length;
        float minx = basePoints[0].position.x;
        float maxx = basePoints[0].position.x;
        height = basePoints[0].position.y;
        for (int i = 1; i < basePointCount; ++i)
        {
            if (basePoints[i].position.x<minx) minx = basePoints[i].position.x;
            if (basePoints[i].position.x > maxx) maxx = basePoints[i].position.x;
            if (basePoints[i].position.y > height) height = basePoints[i].position.y;
        }
        width = maxx - minx;
    }

}
