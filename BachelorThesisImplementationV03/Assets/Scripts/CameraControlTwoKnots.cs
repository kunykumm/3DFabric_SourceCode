using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlTwoKnots : CameraControlBase
{
    public GenerateMeshTwoKnots generateMeshTwoKnots;

    void Start()
    {
        PrepareScene();
        ChangeCameras();
    }

    public void ChangeCameras()
    {
        if (isEditNet)
        {
            FromNetToKnot();
        }
        else
        {
            generateMeshTwoKnots.UpdateComplicatedNet();
            FromKnotToNet();
        }
    }
}
