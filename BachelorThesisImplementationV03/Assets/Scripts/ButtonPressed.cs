using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Class ButtonPressed contains functions for events that has to take place when a button is pressed.
/// This functionality is used by '+' and '-' buttons (children of sizeChanger).
/// Besides MonoBehaviour, this class inherits from two interfaces: IPointerDownHandler, IPointerupHandler. 
/// OnPointerDown and OnPointerUp functions of these interfaces are implemented to register clicks 
/// on the gameObject holding this script.

/// </summary>
public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SizeChanger sizeChanger;
    /// <value> Small decimal number that updates the current scale in sizeChanger </value>
    public float change;
    /// <value> For '+' button it is '-' button, for '-' button it is '+' button </value>
    public Button otherButton;

    private bool ispressed;
    private const float minimumHeldDuration = 0.25f;
    private float buttonPressedTime = 0;

    /// <summary>
    /// Sets the ispressed attribute to false, since no button is pressed at the start of the scene.
    /// </summary>
    void Start()
    {
        ispressed = false;
    }

    /// <summary>
    /// Executes function Change() when the button is pressed and current held duration is higher than 
    /// minimum held duration value.
    /// </summary>
    void Update()
    {
        if (ispressed && Time.timeSinceLevelLoad - buttonPressedTime > minimumHeldDuration) Change();
    }

    /// <summary>
    /// Calls function ChangeValues() of sizeChanger when the button is being pressed.
    /// The change is allowed when the fibre diameter of the element is between the allowed minimum 
    /// and maximum width of the fibre diameter.
    /// </summary>
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

    /// <summary>
    /// Sets ispressed and buttonPressedTime attributes the moment the button becomes pressed.
    /// If the button is not interactable, the attributes are not changed and pressing does not happen.
    /// </summary>
    /// <param name="eventData"> Event payload associated with pointer (mouse / touch) events. </param>
    public void OnPointerDown(PointerEventData eventData)
    {
        bool result = MakeButtonInteractable();
        if (GetComponent<Button>().interactable == false) return;
        ispressed = true;
        if (result) sizeChanger.ChangeValues(change);
        buttonPressedTime = Time.timeSinceLevelLoad;
    }

    /// <summary>
    /// Checks if button is interactable when button becomes pressed.
    /// </summary>
    /// <returns> 'true' when button is changed to interactable, 'false' otherwise. </returns>
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
            else if (change > 0 && result == 0.2f)
            {
                otherButton.GetComponent<Button>().interactable = true;
                result = 0;
            }
            else if (change < 0 && result == 1f)
            {
                otherButton.GetComponent<Button>().interactable = true;
                result = 0;
            }
        }
        return (result == 0);
    }

    /// <summary>
    /// Sets ispressed and buttonPressedTime attributes to default values when button stops being pressed.
    /// Thanks to this change the Change() function called in Update() is not called every frame.
    /// </summary>
    /// <param name="eventData"> Event payload associated with pointer (mouse / touch) events. </param>
    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
        buttonPressedTime = 0;
    }
}
