using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SizeChanger sizeChanger;
    public float change;

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
        sizeChanger.ChangeValues(change);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == false) return;
        ispressed = true;
        sizeChanger.ChangeValues(change);
        buttonPressedTime = Time.timeSinceLevelLoad;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //if (GetComponent<Button>().interactable == false) return;
        ispressed = false;
        buttonPressedTime = 0;
    }
}
