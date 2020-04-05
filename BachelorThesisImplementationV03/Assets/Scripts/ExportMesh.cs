using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private ExtensionFilter[] extensionList;

    public Text savedInfo;

    private void Start()
    {
        extensionList = new ExtensionFilter[] {
            new ExtensionFilter("Binary STL", "stl")
        };
    }

    public void ExportNet()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string fileName = PlayerPrefs.GetString("scene");
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extensionList);
        string check = filePath.Substring(filePath.Length - 3, 3);

        if (check.Equals("stl")) SaveAsStl(filePath);
        if (check.Equals("obj")) SaveAsObj(filePath);
        if (check.Equals("fbx")) SaveAsFbx(filePath);
    }

    private bool SaveAsStl(string filePath)
    {
        STL.Export(theWholeMesh, filePath);
        savedInfo.text = "Your net was saved successfully.";
        return true;
    }

    private void SaveAsObj(string filePath)
    {

    }

    private void SaveAsFbx(string filePath)
    {

    }
}
