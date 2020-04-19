using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlCovered : CameraControlBase
{
    public GenerateCoveredMesh generateCoveredMesh;

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
            generateCoveredMesh.UpdateCoveredNet();
            FromKnotToNet();
        }
    }
}
