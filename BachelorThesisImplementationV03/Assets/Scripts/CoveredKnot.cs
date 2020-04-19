using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;
    public int triangleCount;

    private float rWidth = 1f;
    private float rHeight = 1f;

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
