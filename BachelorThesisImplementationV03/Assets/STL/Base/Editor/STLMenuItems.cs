/*
	Part of Unity Asset Store asset package STL.
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;


public class STLMenuItems : ScriptableObject
{   
	[MenuItem( "File/Export/Selected Mesh(es)/STL (Binary)" )]
	public static void ExportSelectedAsBinarySTL()
	{
		ExportSelectedAsSTL();
	}
	
	
	[MenuItem( "File/Export/Selected Mesh(es)/STL (ASCII)" )]
	public static void ExportSelectionAsASCIISTL()
	{
		bool asASCII = true;
		ExportSelectedAsSTL( asASCII );
	}


	[MenuItem("CONTEXT/MeshFilter/Export STL (Binary)")]
	public static void ExportContexMeshFilterAsBinarySTL( MenuCommand menuCommand )
	{
		ExportContextMeshFilterAsSTL( menuCommand );
	}


	[MenuItem("CONTEXT/MeshFilter/Export STL (ASCII)")]
	public static void ExportContexMeshFilterAsASCIISTL( MenuCommand menuCommand )
	{
		bool asASCII = true;
		ExportContextMeshFilterAsSTL( menuCommand, asASCII );
	}


	[MenuItem("CONTEXT/SkinnedMeshRenderer/Export STL (Binary)")]
	public static void ExportContexSkinnedMeshRendererAsBinarySTL( MenuCommand menuCommand )
	{
		ExportContextSkinnedMeshRendererAsSTL( menuCommand );
	}


	[MenuItem("CONTEXT/SkinnedMeshRenderer/Export STL (ASCII)")]
	public static void ExportContexSkinnedMeshRendererAsASCIISTL( MenuCommand menuCommand )
	{
		bool asASCII = true;
		ExportContextSkinnedMeshRendererAsSTL( menuCommand, asASCII );
	}


	public static void ExportContextMeshFilterAsSTL( MenuCommand menuCommand, bool asASCII = false )
	{
		MeshFilter meshFilter = menuCommand.context as MeshFilter;

		// Display dialog and get save path.
		string filePath = EditorUtility.SaveFilePanel( "Save STL file", DefaultDirectory(), DeafultFileName(), "stl" );

		// Export.
		bool success = STL.Export( meshFilter, filePath, asASCII );

		// Display feedback.
		if( success ) DisplaySuccessDialog( filePath, asASCII );
		else DisplayFailDialog();
	}


	public static void ExportContextSkinnedMeshRendererAsSTL( MenuCommand menuCommand, bool asASCII = false )
	{
		SkinnedMeshRenderer meshRenderer = menuCommand.context as SkinnedMeshRenderer;

		// Display dialog and get save path.
		string filePath = EditorUtility.SaveFilePanel( "Save STL file", DefaultDirectory(), DeafultFileName(), "stl" );

		// Export.
		bool success = STL.Export( meshRenderer, filePath, asASCII );

		// Display feedback.
		if( success ) DisplaySuccessDialog( filePath, asASCII );
		else DisplayFailDialog();
	}
		

	static void ExportSelectedAsSTL( bool asASCII = false )
	{
		// Check selection.
		if( Selection.gameObjects == null || Selection.gameObjects.Length == 0 ){
			EditorUtility.DisplayDialog( "Nothing to export", "Select one or more GameObjects with MeshFilter or SkinnedMeshRenderer components attached.", "Close" );
			return;
		}

		bool hasMesh = false;
		foreach( GameObject go in Selection.gameObjects ){
			if( go.GetComponentsInChildren<MeshFilter>().Length != 0 || go.GetComponentsInChildren<SkinnedMeshRenderer>().Length != 0 ){
				hasMesh = true;
				break;
			}
		}
		if( !hasMesh ){
			EditorUtility.DisplayDialog( "Nothing to export", "Select one or more GameObjects with MeshFilter or SkinnedMeshRenderer components attached.", "Close" );
			return;
		}
		
		// Display dialog and get save path.
		string filePath = EditorUtility.SaveFilePanel( "Save STL file", DefaultDirectory(), DeafultFileName(), "stl" );
		
		// Export.
		bool success = STL.Export( Selection.gameObjects, filePath, asASCII );
		
		// Display feedback.
		if( success ) DisplaySuccessDialog( filePath, asASCII );
		else DisplayFailDialog();
	}


	static void DisplaySuccessDialog( string filePath, bool asASCII )
	{
		string typeString = asASCII ? "ASCII" : "binary";
		EditorUtility.DisplayDialog( "Export complete", "Exported " + typeString + " STL file to path:\n" + filePath, "Close" );
	}


	static void DisplayFailDialog()
	{
		EditorUtility.DisplayDialog( "Export failed", "Check console for errors:\n", "Close" );
	}


	static string DeafultFileName()
	{
		string defaultName = DateTimeCode();
		if( SceneManager.GetActiveScene().name != "" ) defaultName = SceneManager.GetActiveScene().name + " " + defaultName;
		return defaultName;
	}
	
	
	static string DefaultDirectory()
	{
		string defaultDirectory = "";
		if( Application.platform == RuntimePlatform.OSXEditor ){
			defaultDirectory = System.Environment.GetEnvironmentVariable( "HOME" ) + "/Desktop";
		} else {
			defaultDirectory = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop );
		}
		return defaultDirectory;
	}
	
	
	static string DateTimeCode()
	{
		return System.DateTime.Now.ToString( "yyMMdd_hhmmss" );
	}
}