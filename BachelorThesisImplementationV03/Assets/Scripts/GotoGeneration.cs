using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoGeneration : MonoBehaviour
{
    public GameObject baseMesh;

    public void GotoGenerateMeshScene()
    {
        Destroy(baseMesh.GetComponent<KnotEditor>());
        PrefabUtility.SaveAsPrefabAsset(baseMesh, "Assets/Resources/Knot.prefab");
        PrefabUtility.SaveAsPrefabAsset(baseMesh, "Assets/Resources/KnotForNet.prefab");
        SceneManager.LoadScene("GenerateMesh");
    }

}
