using UnityEngine;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;

    public void ExportToBinarySTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string filePath = DefaultDirectory() + "/" + PlayerPrefs.GetString("scene") + "_net_bin_" + PlayerPrefs.GetInt("binary") + ".stl";
        STL.Export(theWholeMesh, filePath);
        PlayerPrefs.SetInt("binary", PlayerPrefs.GetInt("binary") + 1);
    }
    
    public void ExportToTextSTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string filePath = DefaultDirectory() + "/" + PlayerPrefs.GetString("scene") + "_net_text_" + PlayerPrefs.GetInt("text") + ".stl";
        bool asASCII = true;
        STL.Export(theWholeMesh, filePath, asASCII);
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
