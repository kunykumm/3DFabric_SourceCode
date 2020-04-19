using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class CameraControlBase : MonoBehaviour
{
    public Button editNet;
    public Button editKnot;

    public Camera cameraKnot;
    public Camera cameraNet;
    public GameObject leftSliderPanel;
    public GameObject rightSliderPanel;
    public GameObject leftAddMinusButtons;
    public Button rightExportButton;

    private CameraMovement camKnotMov;
    private CameraMovement camNetMov;
    private Color customGrey;
    private Color customSilver;
    private Color customOrange;

    protected bool isEditNet;

    protected void PrepareScene()
    {
        camKnotMov = cameraKnot.GetComponent<CameraMovement>();
        camNetMov = cameraNet.GetComponent<CameraMovement>();

        isEditNet = true;

        customGrey = new Color(0.35f, 0.35f, 0.35f, 1);
        customOrange = new Color(1, 0.6578746f, 0, 1);
        customSilver = new Color(195, 195, 195, 1);
    }

    protected void FromNetToKnot()
    {
        StateOfSliders(leftSliderPanel, true);
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

    protected void FromKnotToNet()
    {
        StateOfSliders(leftSliderPanel, false);
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

    private void StateOfSliders(GameObject panel, bool enabled)
    {
        int childrenCount = panel.transform.childCount;
        for (int i = 0; i < childrenCount; ++i)
        {
            if (i % 2 == 1) panel.transform.GetChild(i).gameObject.GetComponent<Slider>().enabled = enabled;
        }
    }

    private void StateOfButtons(GameObject panel, bool enabled, Color color)
    {
        int childrenCount = panel.transform.childCount;
        for (int i = 0; i < childrenCount; ++i)
        {
            var button = panel.transform.GetChild(i).gameObject.GetComponent<Button>();
            button.interactable = enabled;
            button.GetComponentInChildren<Text>().color = color;
        }
    }
}
