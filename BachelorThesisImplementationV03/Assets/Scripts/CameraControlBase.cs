using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains functions that change active viewports in the scene.
/// Moreover, it enables/disables GUI elements according to the active viewport.
/// </summary>
public class CameraControlBase : MonoBehaviour
{
    public GenerateMesh generateMesh;
    public GameObject rightSliderPanel;

    public Button editNet;
    public Button editKnot;

    public Camera cameraKnot;
    public Camera cameraNet;
    public GameObject leftAddMinusButtons;
    public Button rightExportButton;

    protected CameraMovement camKnotMov;
    protected CameraMovement camNetMov;

    /// <value> Custom colours for buttons. </value>
    protected Color customGrey;
    protected Color customSilver;
    protected Color customOrange;

    /// <value> 'true' when the viewport of net editing is active, 'false' otherwise. </value>
    protected bool isEditNet;

    /// <summary>
    /// Calls functions to set the default viewport workspace at the start of the scene.
    /// </summary>
    void Start()
    {
        PrepareScene();
        ChangeCameras();
    }

    /// <summary>
    /// Sets up all uninitialised (protected) attributes.
    /// </summary>
    protected void PrepareScene()
    {
        camKnotMov = cameraKnot.GetComponent<CameraMovement>();
        camNetMov = cameraNet.GetComponent<CameraMovement>();

        isEditNet = true;

        customGrey = new Color(0.35f, 0.35f, 0.35f, 1);
        customOrange = new Color(1, 0.6578746f, 0, 1);
        customSilver = new Color(195, 195, 195, 1);
    }

    /// <summary>
    /// Makes net editing viewport inactive and knot editing viewport active. The GUI changes accordingly.
    /// </summary>
    protected void FromNetToKnotBase()
    {
        StateOfButtons(leftAddMinusButtons, true, customSilver);
        StateOfSliders(rightSliderPanel, false);

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

    /// <summary>
    /// Makes knot editing viewport inactive and net editing viewport active. The GUI changes accordingly.
    /// </summary>
    protected void FromKnotToNetBase()
    {
        StateOfButtons(leftAddMinusButtons, false, customGrey);
        StateOfSliders(rightSliderPanel, true);

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

    /// <summary>
    /// Helper function to make sliders enabled/disabled.
    /// </summary>
    /// <param name="panel"> UnityEngine.UI panel that has sliders as its children. </param>
    /// <param name="enabled"> 'true' when sliders should become enabled, 'false' otherwise. </param>
    protected void StateOfSliders(GameObject panel, bool enabled)
    {
        int childrenCount = panel.transform.childCount;
        for (int i = 0; i < childrenCount; ++i)
        {
            if (i % 2 == 1) panel.transform.GetChild(i).gameObject.GetComponent<Slider>().enabled = enabled;
        }
    }

    /// <summary>
    /// Helper function to make buttons enabled/disabled.
    /// </summary>
    /// <param name="panel"> UnityEngine.UI panel that has buttons as its children. </param>
    /// <param name="enabled"> 'true' when buttons should become enabled, 'false' otherwise. </param>
    /// <param name="color"> Colour that button should be changed to. </param>
    protected void StateOfButtons(GameObject panel, bool enabled, Color color)
    {
        int childrenCount = panel.transform.childCount;
        for (int i = 0; i < childrenCount; ++i)
        {
            var button = panel.transform.GetChild(i).gameObject.GetComponent<Button>();
            button.interactable = enabled;
            button.GetComponentInChildren<Text>().color = color;
        }
    }

    /// <summary>
    /// Switches the active viewport of the scene.
    /// </summary>
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
