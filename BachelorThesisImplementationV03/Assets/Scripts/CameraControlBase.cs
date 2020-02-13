using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class CameraControlBase : MonoBehaviour
{
    public Camera cameraKnot;
    public Camera cameraNet;
    public GameObject leftSliderPanel;
    public GameObject rightSliderPanel;

    private CameraMovement camKnotMov;
    private CameraMovement camNetMov;

    protected bool editNet;
    public Text buttonText;

    protected void PrepareScene()
    {
        camKnotMov = cameraKnot.GetComponent<CameraMovement>();
        camNetMov = cameraNet.GetComponent<CameraMovement>();

        editNet = true;
    }

    protected void FromNetToKnot()
    {
        StateOfSliders(leftSliderPanel, true);
        StateOfSliders(rightSliderPanel, false);
        camKnotMov.enabled = true;
        camNetMov.enabled = false;
        editNet = false;
        buttonText.text = "Edit Net";
    }

    protected void FromKnotToNet()
    {
        StateOfSliders(leftSliderPanel, false);
        StateOfSliders(rightSliderPanel, true);
        camKnotMov.enabled = false;
        camNetMov.enabled = true;
        editNet = true;
        buttonText.text = "Edit Knot";
    }

    private void StateOfSliders(GameObject panel, bool enabled)
    {
        int childrenCount = panel.transform.childCount;
        for (int i = 0; i < childrenCount; ++i)
        {
            if (i % 2 == 1) panel.transform.GetChild(i).gameObject.GetComponent<Slider>().enabled = enabled;
        }
    }
}
