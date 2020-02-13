using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public string scene;

    public void GotoBack()
    {
        SceneManager.LoadScene(scene);
    }
    
}
