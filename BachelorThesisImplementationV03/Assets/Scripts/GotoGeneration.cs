using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoGeneration : MonoBehaviour
{
    public GameObject baseMesh;

    public void GotoGenerateMeshScene()
    {
        SaveInformation();
        PreparePrefabs();
        SceneManager.LoadScene("GenerateMesh");
    }

    private void SaveInformation()
    {
        var knotEditor = baseMesh.GetComponent<KnotEditor>();
        if (knotEditor.angle != null) PlayerPrefs.SetFloat("angle", knotEditor.angle.value);
        PlayerPrefs.SetFloat("width", knotEditor.width.value);
        PlayerPrefs.SetFloat("detail", knotEditor.detail.value);
        PlayerPrefs.SetString("lWidth", knotEditor.lineWidth.text);
        PlayerPrefs.SetString("rWidth", knotEditor.realWidth.text);
        PlayerPrefs.SetString("rHeight", knotEditor.realHeight.text);
        PlayerPrefs.SetString("scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("rotation", knotEditor.rotationWhenGenerated);
        Destroy(baseMesh.GetComponent<KnotEditor>());
    }

    private void PreparePrefabs()
    {
        //var points = baseMesh.GetComponent<SplineComputer>().GetPoints();

        //var basePrefab = (GameObject)Resources.Load("Knot");
        //basePrefab.GetComponent<SplineComputer>().SetPoints(points);
        //basePrefab.GetComponent<TubeGenerator>().sides = baseMesh.GetComponent<TubeGenerator>().sides;

        //var netPrefab = (GameObject)Resources.Load("KnotForNet");
        //netPrefab.GetComponent<SplineComputer>().SetPoints(points);    
        //netPrefab.GetComponent<TubeGenerator>().sides = baseMesh.GetComponent<TubeGenerator>().sides;

        //Debug.Log(basePrefab.GetComponent<SplineComputer>().pointCount);
        //Debug.Log(netPrefab.GetComponent<SplineComputer>().pointCount);
    }
}
