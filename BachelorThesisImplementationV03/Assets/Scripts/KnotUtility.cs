using Dreamteck.Splines;
using System;
using UnityEngine;

public class KnotUtility
{
    float[] lineWidthRatios = new float[]{ 1.164f, 1.193f, 1.23f, 1.27f, 1.312f, 1.36f, 1.41f, 1.452f, 1.495f };


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
        //Debug.Log("Current value: " + lineWidth);
        int minEdge = (int)Math.Floor(lineWidth * 10 + 0.05);
        int maxEdge = (int)Math.Ceiling(lineWidth * 10 - 0.05);
        //Debug.Log("Floor: " + minEdge + ", Ceiling: " + maxEdge);
        float decimalValues = lineWidth - ((float)minEdge / 10);
        //Debug.Log("Decimal values: " + decimalValues);
        float firstValue = lineWidthRatios[minEdge - 2];
        float secondValue = lineWidthRatios[maxEdge - 2];
        //Debug.Log("First: " + firstValue + ", Second: " + secondValue);
        float difference = (secondValue - firstValue) * Math.Abs(decimalValues);
        //Debug.Log("Difference: " + difference);
        //Debug.Log("Final Value: " + (firstValue + difference));
        return (firstValue + difference);

    }
}
