using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredKnot : MonoBehaviour
{
    public SizeChanger sizeChanger;

    private float rWidth = 1;
    private float rHeight = 1;

    void Start()
    {
        sizeChanger.SetHeight(rHeight, 0);
        sizeChanger.SetWidth(rWidth, 0);
        sizeChanger.SetLineWidth(0.12f);
        sizeChanger.SetOffsets(0.1f, 0.1f);
        sizeChanger.ChangeSizesNet();
    }
}
