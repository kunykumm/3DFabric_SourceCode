using UnityEngine;

/// <summary>
/// Contains function for exiting the application.
/// This script is attached to the 'Exit' button.
/// </summary>
public class Exit : MonoBehaviour
{
    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitApplication()
    {
        Application.Quit();
    }
}
