using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Dreamteck.Splines;

public class ExportMesh : MonoBehaviour
{
    private GameObject[] theWholeMesh;
    private int binCount = 1;
    private int textCount = 1;

    public void ExportToBinarySTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string filePath = DefaultDirectory() + "/stl_example_binary" + binCount + ".stl";
        STL.Export(theWholeMesh, filePath);
        binCount++;
    }
    
    public void ExportToTextSTL()
    {
        theWholeMesh = GameObject.FindGameObjectsWithTag("knotrow");
        string filePath = DefaultDirectory() + "/stl_example_text" + textCount + ".stl";
        bool asASCII = true;
        STL.Export(theWholeMesh, filePath, asASCII);
        textCount++;
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
