using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera cameraKnot;
    public Camera cameraNet;

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
            camKnotMov.enabled = true;
            camNetMov.enabled = false;
            editNet = false;
        }
        else
        {
            camKnotMov.enabled = false;
            camNetMov.enabled = true;
            editNet = true;
        }
    }
}
