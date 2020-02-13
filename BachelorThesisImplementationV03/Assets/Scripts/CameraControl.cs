using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Camera cameraKnot;
    public Camera cameraNet;
    public GenerateMesh generateMesh;
    public GameObject leftSliderPanel;
    public GameObject rightSliderPanel;

    private CameraMovement camKnotMov;
    private CameraMovement camNetMov;

    private bool editNet;

    // Start is called before the first frame update
    void Start()
    {
        camKnotMov = cameraKnot.GetComponent<CameraMovement>();
        camNetMov = cameraNet.GetComponent<CameraMovement>();
        
        editNet = false;
        ChangeCameras();
        ChangeCameras();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCameras()
    {
        if (editNet)
        {
            StateOfSliders(leftSliderPanel, true);
            StateOfSliders(rightSliderPanel, false);
            camKnotMov.enabled = true;
            camNetMov.enabled = false;
            editNet = false;
        }
        else
        {
            generateMesh.UpdateNet();
            StateOfSliders(leftSliderPanel, false);
            StateOfSliders(rightSliderPanel, true);
            camKnotMov.enabled = false;
            camNetMov.enabled = true;
            editNet = true;
        }
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
