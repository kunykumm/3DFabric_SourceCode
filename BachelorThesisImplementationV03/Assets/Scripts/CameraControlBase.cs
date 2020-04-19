using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControlBase : MonoBehaviour
{
    public GenerateMesh generateMesh;

    public Button editNet;
    public Button editKnot;

    public Camera cameraKnot;
    public Camera cameraNet;
    public GameObject leftAddMinusButtons;
    public Button rightExportButton;

    protected CameraMovement camKnotMov;
    protected CameraMovement camNetMov;
    protected Color customGrey;
    protected Color customSilver;
    protected Color customOrange;

    protected bool isEditNet;

    void Start()
    {
        PrepareScene();
        ChangeCameras();
    }

    protected void PrepareScene()
    {
        camKnotMov = cameraKnot.GetComponent<CameraMovement>();
        camNetMov = cameraNet.GetComponent<CameraMovement>();

        isEditNet = true;

        customGrey = new Color(0.35f, 0.35f, 0.35f, 1);
        customOrange = new Color(1, 0.6578746f, 0, 1);
        customSilver = new Color(195, 195, 195, 1);
    }

    protected void FromNetToKnotBase()
    {
        rightExportButton.interactable = false;
        rightExportButton.GetComponentInChildren<Text>().color = customGrey;
        editNet.interactable = true;
        editNet.GetComponentInChildren<Text>().color = customOrange;
        editKnot.interactable = false;
        editKnot.GetComponentInChildren<Text>().color = customGrey;

        camKnotMov.enabled = true;
        camNetMov.enabled = false;
        isEditNet = false;
    }

    protected void FromKnotToNetBase()
    {
        rightExportButton.interactable = true;
        rightExportButton.GetComponentInChildren<Text>().color = customOrange;
        editNet.interactable = false;
        editNet.GetComponentInChildren<Text>().color = customGrey;
        editKnot.interactable = true;
        editKnot.GetComponentInChildren<Text>().color = customOrange;

        camKnotMov.enabled = false;
        camNetMov.enabled = true;
        isEditNet = true;
    }

    public virtual void ChangeCameras()
    {
        if (isEditNet)
        {
            FromNetToKnotBase();
        }
        else
        {
            generateMesh.UpdateNet();
            FromKnotToNetBase();
        }
    }
}
