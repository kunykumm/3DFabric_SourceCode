using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : CameraControlBase
{
    public GenerateMesh generateMesh;

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
            generateMesh.UpdateNet();
            FromKnotToNet();
        }
    }
}
