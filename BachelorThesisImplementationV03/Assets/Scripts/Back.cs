using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public string scene;

    public void GotoBack()
    {
        if (PlayerPrefs.GetString("scene") != null)
        {
            scene = PlayerPrefs.GetString("scene");
            PlayerPrefs.SetString("scene", null);
        }
        SceneManager.LoadScene(scene);
    }
    
}
