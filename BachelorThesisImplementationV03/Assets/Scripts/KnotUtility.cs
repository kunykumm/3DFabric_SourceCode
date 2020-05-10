using Dreamteck.Splines;

/// <summary>
/// Helper class providing general functionality for Dreamteck Splines needed by many other classes
/// </summary>
public class KnotUtility
{
    /// <summary>
    /// Finds the height and width of a single element created by Dreamteck Splines.
    /// These values are necessary for calculating the positions of elements during net generation.
    /// </summary>
    /// <param name="height"> height of the single element </param>
    /// <param name="width"> width of the single element </param>
    /// <param name="basePoints"> all points of the spline the single element is made of </param>
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
