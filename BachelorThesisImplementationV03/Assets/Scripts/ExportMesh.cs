using SFB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private ExtensionFilter[] extensionList;
    private CombineInstance[] combineInstances = { };
    private Mesh[] meshes = { };
    private Matrix4x4[] matrices = { };

    public Text savedInfo;
    public GameObject objectForExport;
    private SizeChanger sizeChanger;

    private void Start()
    {
        extensionList = new ExtensionFilter[] {
            new ExtensionFilter("Binary STL", "stl"),
            new ExtensionFilter("Wavefront OBJ", "obj")
        };
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        sizeChanger = GameObject.Find("SizeChanger").GetComponent<SizeChanger>();
    }

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

    private void SaveAsStl(string filePath)
    {
        float scale = sizeChanger.GetCurrentScale();
        PrepareMeshesForStl();
        STL.Export(meshes, matrices, filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    private void SaveAsObj(string filePath)
    {
        PrepareObjectForExport();
        ObjExporter.MeshToFile(objectForExport.transform.GetComponent<MeshFilter>(), filePath);
        savedInfo.text = "Your net was saved successfully.";
    }

    private void PrepareMeshesForStl()
    {
        int length = theWholeMesh.Length;
        meshes = new Mesh[length];
        matrices = new Matrix4x4[length];
        float scale = sizeChanger.GetCurrentScale();
        Vector3 scaleV = new Vector3(scale, scale, scale);
        for (int i = 0; i < length; ++i)
        {
            meshes[i] = theWholeMesh[i].GetComponent<MeshFilter>().mesh;
            matrices[i] = Matrix4x4.TRS(theWholeMesh[i].transform.position * scale, theWholeMesh[i].transform.rotation, scaleV);

        }
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

    private void PrepareObjectForExport()
    {
        CombineMeshes();
        objectForExport.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        objectForExport.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstances);
        objectForExport.transform.localScale *= sizeChanger.GetCurrentScale();
    }
}
