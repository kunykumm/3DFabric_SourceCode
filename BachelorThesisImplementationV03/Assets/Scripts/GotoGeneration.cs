using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoGeneration : MonoBehaviour
{
    public GameObject baseMesh;

    public void GotoGenerateMeshScene()
    {
        var knotEditor = baseMesh.GetComponent<KnotEditor>();
        PlayerPrefs.SetFloat("angle", knotEditor.angle.value);
        PlayerPrefs.SetFloat("width", knotEditor.width.value);
        PlayerPrefs.SetFloat("detail", knotEditor.detail.value);
        PlayerPrefs.SetString("lWidth", knotEditor.lineWidth.text);
        PlayerPrefs.SetString("rWidth", knotEditor.realWidth.text);
        PlayerPrefs.SetString("rHeight", knotEditor.realHeight.text);
        Destroy(baseMesh.GetComponent<KnotEditor>());
        PrefabUtility.SaveAsPrefabAsset(baseMesh, "Assets/Resources/Knot.prefab");
        PrefabUtility.SaveAsPrefabAsset(baseMesh, "Assets/Resources/KnotForNet.prefab");
        SceneManager.LoadScene("GenerateMesh");
    }

}
