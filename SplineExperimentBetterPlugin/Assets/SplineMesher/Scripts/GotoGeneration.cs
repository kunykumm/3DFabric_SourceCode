using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoGeneration : MonoBehaviour
{
    public GameObject baseMesh;

    public void GotoGenerateMeshScene()
    {
        DontDestroyOnLoad(baseMesh);
        SceneManager.LoadScene("GenerateMesh");
    }

}
