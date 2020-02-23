using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private bool blah;
    private ExtensionFilter[] extensionList;
    public Text savedInfo;

    private void Start()
    {
        blah = false;
        extensionList = new ExtensionFilter[] {
            new ExtensionFilter("Binary STL", "bin.stl"),
            new ExtensionFilter("Text STL", "txt.stl")
        };
    }

    public void ExportNet()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string fileName = PlayerPrefs.GetString("scene");
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", "", fileName, extensionList);
        if (!SaveAsStl(filePath)) blah = true;
        savedInfo.text = "Your net was saved successfully.";
    }

    private bool SaveAsStl(string filePath)
    {
        string check = filePath.Substring(filePath.Length - 3, 3);
        if (!check.Equals("stl")) return false;

        string choice = filePath.Substring(filePath.Length - 7, 3);
        if (choice.Equals("bin"))
        {
            filePath = filePath.Replace(".bin", "");
            STL.Export(theWholeMesh, filePath);
        }
        if (choice.Equals("txt"))
        {
            filePath = filePath.Replace(".txt", "");
            bool asASCII = true;
            STL.Export(theWholeMesh, filePath, asASCII);
        }

        return true;
    }
}
