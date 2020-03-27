using Dreamteck.Splines;
using System;
using UnityEngine;

public class KnotUtility
{
    float[] lineWidthRatios = new float[]{ 1.156f, 1.185f, 1.215f, 1.243f, 1.268f, 1.298f, 1.309f, 1.339f, 1.366f };


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

    public float getLineWidthRatio(float lineWidth)
    {
        Debug.Log("Current value: " + lineWidth);
        int minEdge = (int)Math.Floor(lineWidth * 10 + 0.05);
        int maxEdge = (int)Math.Ceiling(lineWidth * 10 - 0.05);
        Debug.Log("Floor: " + minEdge + ", Ceiling: " + maxEdge);
        float decimalValues = lineWidth - ((float)minEdge / 10);
        Debug.Log("Decimal values: " + decimalValues);
        float firstValue = lineWidthRatios[minEdge - 2];
        float secondValue = lineWidthRatios[maxEdge - 2];
        Debug.Log("First: " + firstValue + ", Second: " + secondValue);
        float difference = (secondValue - firstValue) * Math.Abs(decimalValues);
        Debug.Log("Difference: " + difference);
        Debug.Log("Final Value: " + (firstValue + difference));
        return (firstValue + difference);

    }
}
