using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSimply : CameraControlBase
{
    public GenerateSimplyMesh generateSimplyMesh;

    void Start()
    {
        PrepareScene();
        ChangeCameras();
    }

    public void ChangeCameras()
    {
        if (editNet)
        {
            FromNetToKnot();
        }
        else
        {
            generateSimplyMesh.UpdateNet();
            FromKnotToNet();
        }
    }
}
