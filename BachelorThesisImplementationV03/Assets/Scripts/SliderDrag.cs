using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public SizeChanger sizeChanger;

    public void OnPointerUp(PointerEventData eventData)
    {
        sizeChanger.ChangeValues(0);
    }
}
