using SFB;
using UnityEngine;
using UnityEngine.UI;

//<Source> Save Dialog: https://github.com/gkngkc/UnityStandaloneFileBrowser
//<Source> STL Exporter: https://assetstore.unity.com/packages/tools/input-management/stl-3397
//<Source> OBJ Exporter: https://wiki.unity3d.com/index.php/ObjExporter

/// <summary>
/// Contains functions for exporting the mesh from the scene.
/// </summary>
public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private ExtensionFilter[] extensionList;
    private CombineInstance[] combineInstances = { };

    public Text savedInfo;
    public GameObject objectForExport;
    private SizeChanger sizeChanger;

    /// <summary>
    /// Sets up the ExtensionFilter array.
    /// Puts new mesh into MeshFilter component of the objectForExport.
    /// Finds the sizeChanger in the scene.
    /// </summary>
    private void Start()
    {
        extensionList = new ExtensionFilter[] {
            new ExtensionFilter("Binary STL", "stl"),
            new ExtensionFilter("Wavefront OBJ", "obj")
        };
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        sizeChanger = GameObject.Find("SizeChanger").GetComponent<SizeChanger>();
    }

    /// <summary>
    /// It is called when 'Export' button is pressed.
    /// Finds all GameObjects with a tag 'knotrow' and saves them into an array.
    /// Opens the save dialog using StandaloneFileBrowser wrapper, calling function SaveFilePanel().
    /// Exports the mesh in the chosen format.
    /// </summary>
    public void ExportNet()
    {
        savedInfo.text = "";
        savedInfo.CrossFadeAlpha(1.0f, 0.0f, false);

        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");

        string fileName = "";
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extensionList);
        if (filePath.Equals(""))
        {
            savedInfo.text = "Incorrect file name. Net was not saved.";
            savedInfo.CrossFadeAlpha(0.0f, 5.0f, false);
            return;
        }
        string check = filePath.Substring(filePath.Length - 3, 3);
        if (check.Equals("stl")) SaveAsStl(filePath);
        if (check.Equals("obj")) SaveAsObj(filePath);
        savedInfo.text = "Your net was saved successfully.";
        savedInfo.CrossFadeAlpha(0.0f, 5.0f, false);
    }

    /// <summary>
    /// Saves the mesh in STL format using STL exporter function Export().
    /// </summary>
    /// <param name="filePath"> File path given by the user. </param>
    private void SaveAsStl(string filePath)
    {
        PrepareObjectForExport();
        STL.Export(objectForExport, filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    /// <summary>
    /// Saves the mesh in OBJ format using OBJ exporter function MeshToFile().
    /// </summary>
    /// <param name="filePath"> File path given by the user. </param>
    private void SaveAsObj(string filePath)
    {
        PrepareObjectForExport();
        ObjExporter.MeshToFile(objectForExport.transform.GetComponent<MeshFilter>(), filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    /// <summary>
    /// Finds MeshFilters of all GameObjects in theWholeMesh array.
    /// Creates combineInstances for all found MeshFilters.
    /// </summary>
    private void CombineMeshes()
    {
        int count = theWholeMesh.Length;
        combineInstances = new CombineInstance[count];

        for (int i = 0; i < count; ++i)
        {
            MeshFilter current = theWholeMesh[i].GetComponent<MeshFilter>();
            combineInstances[i].mesh = current.sharedMesh;
            combineInstances[i].transform = current.transform.localToWorldMatrix;
        }
    }

    /// <summary>
    /// Combines meshes from combineInstances array to one mesh.
    /// Gets all vertices of this mesh and scales using current scale from sizeChanger.
    /// Replaces vertices of the mesh with scaled vertices.
    /// <code>
    ///    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32   
    /// </code>
    /// Increases the number of vertices in a new mesh (Default is 65k vertices, which is too low).
    /// </summary>
    private void PrepareObjectForExport()
    {
        CombineMeshes();
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        objectForExport.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances);
        float currentScale = sizeChanger.GetCurrentScale();
        Vector3[] meshVertices = objectForExport.transform.GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshVertices[i] *= currentScale;
        }
        objectForExport.transform.GetComponent<MeshFilter>().mesh.vertices = meshVertices;
    }
}
