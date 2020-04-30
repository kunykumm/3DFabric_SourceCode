using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;
    public int triangleCount;

    public float lineWidth;
    public float offset;

    private float rWidth;
    private float rHeight;

    void Start()
    {
        Bounds bounds = GetComponent<MeshRenderer>().bounds;
        rHeight = bounds.size.y;
        rWidth = bounds.size.x;
        sizeChanger.SetHeight(rHeight, 0);
        sizeChanger.SetWidth(rWidth, 0);
        sizeChanger.SetLineWidth(lineWidth);
        sizeChanger.SetOffsets(offset, offset);
        sizeChanger.ChangeSizesNet();
        sizeChanger.UpdateTriangleCount(triangleCount);
    }

}
