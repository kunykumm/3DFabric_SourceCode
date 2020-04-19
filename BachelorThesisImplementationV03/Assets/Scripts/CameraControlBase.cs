using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class CameraControlBase : CameraControlCovered
{
    public GameObject leftSliderPanel;
    public GameObject rightSliderPanel;

    protected void FromNetToKnot()
    {
        StateOfSliders(leftSliderPanel, true);
        StateOfButtons(leftAddMinusButtons, true, customSilver);
        StateOfSliders(rightSliderPanel, false);

        FromNetToKnotBase();
    }

    protected void FromKnotToNet()
    {
        StateOfSliders(leftSliderPanel, false);
        StateOfButtons(leftAddMinusButtons, false, customGrey);
        StateOfSliders(rightSliderPanel, true);

        FromKnotToNetBase();
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
