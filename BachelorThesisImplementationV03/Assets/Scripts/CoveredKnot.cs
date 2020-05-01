﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;
    public int triangleCount;

    public float rWidth;
    public float rHeight;
    public float lineWidth;
    public float offset;

    void Start()
    {
        sizeChanger.SetHeight(rHeight);
        sizeChanger.SetWidth(rWidth);
        sizeChanger.SetLineWidth(lineWidth);
        sizeChanger.ChangeSizesNet();
        sizeChanger.UpdateTriangleCount(triangleCount);
    }

}
