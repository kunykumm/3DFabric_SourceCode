using SFB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private ExtensionFilter[] extensionList;
    private CombineInstance[] combineInstances = { };

    public Text savedInfo;
    public GameObject objectForExport;

    private void Start()
    {
        extensionList = new ExtensionFilter[] {
            new ExtensionFilter("Binary STL", "stl"),
            new ExtensionFilter("Wavefront OBJ", "obj")
        };
    }

    public void ExportNet()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string fileName = PlayerPrefs.GetString("scene");
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extensionList);
        if (filePath.Equals("")) return;
        string check = filePath.Substring(filePath.Length - 3, 3);

        if (check.Equals("stl")) SaveAsStl(filePath);
        if (check.Equals("obj")) SaveAsObj(filePath);
        if (check.Equals("fbx")) SaveAsFbx(filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    private void SaveAsStl(string filePath)
    {
        STL.Export(theWholeMesh, filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    private void SaveAsObj(string filePath)
    {
        CombineMeshes();
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        objectForExport.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances);
        ObjExporter.MeshToFile(objectForExport.transform.GetComponent<MeshFilter>(), filePath);
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        savedInfo.text = "Your net was saved successfully.";
    }

    private void SaveAsFbx(string filePath)
    {
    }

    private void CombineMeshes()
    {
        int count = theWholeMesh.Length;
        MeshFilter[] meshFilters = new MeshFilter[count];
        combineInstances = new CombineInstance[count];

        for (int i = 0; i < count; ++i)
        {
            meshFilters[i] = theWholeMesh[i].GetComponent<MeshFilter>();
        }

        for (int j = 0; j < count; ++j)
        {
            combineInstances[j].mesh = meshFilters[j].sharedMesh;
            combineInstances[j].transform = meshFilters[j].transform.localToWorldMatrix;
        }
    }
}
