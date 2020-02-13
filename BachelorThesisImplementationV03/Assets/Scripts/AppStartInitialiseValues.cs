using UnityEngine;

public class AppStartInitialiseValues : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("scene", "");
        PlayerPrefs.SetInt("binary", 1);
        PlayerPrefs.SetInt("text", 1);
    }
}
