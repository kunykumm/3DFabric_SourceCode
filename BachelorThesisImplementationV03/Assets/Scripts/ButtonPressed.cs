using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SizeChanger sizeChanger;
    public float change;
    public Button otherButton;

    private bool ispressed;
    private const float minimumHeldDuration = 0.25f;
    private float buttonPressedTime = 0;


    void Start()
    {
        ispressed = false;
    }

    void Update()
    {
        if (ispressed && Time.timeSinceLevelLoad - buttonPressedTime > minimumHeldDuration) Change();
    }

    private void Change()
    {
        float result = sizeChanger.AllowChangeValues();
        if (result == 0) sizeChanger.ChangeValues(change);
        else
        {
            if (change < 0 && result == sizeChanger.GetCoveredDefaultLineWidth()) GetComponent<Button>().interactable = false;
            if (change < 0 && result == 0.2f) GetComponent<Button>().interactable = false;
            if (change > 0 && result == 1f) GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        bool result = MakeButtonInteractable();
        if (GetComponent<Button>().interactable == false) return;
        ispressed = true;
        if (result) sizeChanger.ChangeValues(change);
        buttonPressedTime = Time.timeSinceLevelLoad;
    }

    public bool MakeButtonInteractable()
    {
        float result = sizeChanger.AllowChangeValues();
        if (result != 0)
        {
            if (change > 0 && result == sizeChanger.GetCoveredDefaultLineWidth())
            {
                otherButton.GetComponent<Button>().interactable = true;
                result = 0;
            }
            if (change > 0 && result == 0.2f)
            {
                otherButton.GetComponent<Button>().interactable = true;
                result = 0;
            }
            if (change < 0 && result == 1f)
            {
                otherButton.GetComponent<Button>().interactable = true;
                result = 0;
            }
        }
        return (result == 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
        buttonPressedTime = 0;
    }
}
