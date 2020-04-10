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
            if (change < 0 && result == 0.12f) GetComponent<Button>().interactable = false;
            if (change < 0 && result == 0.2f) GetComponent<Button>().interactable = false;
            if (change > 0 && result == 1f) GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        float result = sizeChanger.AllowChangeValues();
        if (result != 0)
        {
            if (change > 0 && result == 0.12f)
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
        if (GetComponent<Button>().interactable == false) return;
        ispressed = true;
        if (result == 0) sizeChanger.ChangeValues(change);
        buttonPressedTime = Time.timeSinceLevelLoad;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
        buttonPressedTime = 0;
    }
}
