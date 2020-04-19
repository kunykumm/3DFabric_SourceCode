using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;
    public int triangleCount;

    public float rWidth;
    public float rHeight;

    void Start()
    {
        sizeChanger.SetHeight(rHeight, 0);
        sizeChanger.SetWidth(rWidth, 0);
        sizeChanger.SetLineWidth(0.14f);
        sizeChanger.SetOffsets(0.14f, 0.14f);
        sizeChanger.ChangeSizesNet();
        sizeChanger.UpdateTriangleCount(triangleCount);
    }

}
