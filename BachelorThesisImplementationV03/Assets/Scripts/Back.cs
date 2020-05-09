using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class Back for back button functionality.
/// </summary>
public class Back : MonoBehaviour
{
    /// <value> Name of the previous scene given in editor. </value>
    public string scene;

    /// <summary>
    /// Function loads the previous scene.
    /// </summary>
    public void GotoBack()
    {
        SceneManager.LoadScene(scene);
    }
    
}
