using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoEditKnotBack : MonoBehaviour
{
    public string scene;

    public void GotoBack()
    {
        SceneManager.LoadScene(scene);
    }
    
}
