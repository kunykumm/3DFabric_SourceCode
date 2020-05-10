using UnityEngine;

/// <summary>
/// Holds information about covered element.
/// </summary>
public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;
    public int triangleCount;

    public float rWidth;
    public float rHeight;
    public float lineWidth;

    /// <summary>
    /// Sets up the sizeChanger attributes for this element.
    /// </summary>
    void Start()
    {
        sizeChanger.SetHeight(rHeight);
        sizeChanger.SetWidth(rWidth);
        sizeChanger.SetLineWidth(lineWidth);
        sizeChanger.SetCoveredDefaultLineWidth(lineWidth);
        sizeChanger.ChangeSizesNet();
        sizeChanger.UpdateTriangleCount(triangleCount);
    }

}
