using UnityEngine;

public class AppStartInitialiseValues : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        PlayerPrefs.SetString("scene", "");
    }
}
