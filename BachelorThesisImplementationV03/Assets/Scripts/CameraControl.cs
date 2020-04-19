using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : CameraControlBase
{
    public GameObject leftSliderPanel;

    protected void FromNetToKnot()
    {
        StateOfSliders(leftSliderPanel, true);
        FromNetToKnotBase();
    }

    protected void FromKnotToNet()
    {
        StateOfSliders(rightSliderPanel, true);
        FromKnotToNetBase();
    }

    public override void ChangeCameras()
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
