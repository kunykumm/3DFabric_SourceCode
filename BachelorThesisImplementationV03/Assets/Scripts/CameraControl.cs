using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extension for CameraControlBase when sliders are present in the left viewport
/// </summary>
public class CameraControl : CameraControlBase
{
    public GameObject leftSliderPanel;

    /// <summary>
    /// Makes net editing viewport inactive and knot editing viewport active. The GUI changes accordingly.
    /// </summary>
    protected void FromNetToKnot()
    {
        StateOfSliders(leftSliderPanel, true);
        FromNetToKnotBase();
    }

    /// <summary>
    /// Makes knot editing viewport inactive and net editing viewport active. The GUI changes accordingly.
    /// </summary>
    protected void FromKnotToNet()
    {
        StateOfSliders(leftSliderPanel, false);
        FromKnotToNetBase();
    }

    /// <summary>
    /// Overrides the function in base class.
    /// Switches the active viewport of the scene.
    /// </summary>
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
