using UnityEngine;
using UnityEngine.UI;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    public Text savedInfo;

    public void ExportToBinarySTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string fileName = PlayerPrefs.GetString("scene") + "_net_bin_" + PlayerPrefs.GetInt("binary") + ".stl";
        string filePath = DefaultDirectory() + "/" + fileName;
        STL.Export(theWholeMesh, filePath);
        savedInfo.text = "Your net was saved on Desktop as " + fileName + ".";
        PlayerPrefs.SetInt("binary", PlayerPrefs.GetInt("binary") + 1);
    }
    
    public void ExportToTextSTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string fileName = PlayerPrefs.GetString("scene") + "_net_text_" + PlayerPrefs.GetInt("text") + ".stl";
        string filePath = DefaultDirectory() + "/" + fileName;
        bool asASCII = true;
        STL.Export(theWholeMesh, filePath, asASCII);
        savedInfo.text = "Your net was saved on Desktop as " + fileName + ".";
        PlayerPrefs.SetInt("text", PlayerPrefs.GetInt("text") + 1);
    }

    private static string DefaultDirectory()
    {
        string defaultDirectory = "";
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            defaultDirectory = System.Environment.GetEnvironmentVariable("HOME") + "/Desktop";
        }
        else
        {
            defaultDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }
        return defaultDirectory;
    }
}
