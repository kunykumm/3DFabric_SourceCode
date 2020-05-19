using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Contains function to control dragging of the slider.
/// </summary>
public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public SizeChanger sizeChanger;

    /// <summary>
    /// Executes when mouse stops holding head of a slider, in this case the fibre diameter slider. 
    /// The fibre diameter needs to change accordingly to already applied scales on the object, so by calling this function the
    /// scales are updated when value in the slider changes.
    /// </summary>
    /// <param name="eventData"> Event payload associated with pointer (mouse / touch) events. </param>
    public void OnPointerUp(PointerEventData eventData)
    {
        sizeChanger.UpdateAllValues();
    }
}
