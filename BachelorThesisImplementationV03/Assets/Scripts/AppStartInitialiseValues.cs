using UnityEngine;

public class AppStartInitialiseValues : MonoBehaviour
{
    void Start()
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        PlayerPrefs.SetString("scene", "");
        //PlayerPrefs.SetInt("binary", 1);
        //PlayerPrefs.SetInt("text", 1);
    }
}
