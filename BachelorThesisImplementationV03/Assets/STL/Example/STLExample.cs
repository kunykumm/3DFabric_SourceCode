/*
	Part of STL, Asset Store product.
*/

using UnityEngine;
using System.Collections;

namespace STLExamples
{
	public class STLExample : MonoBehaviour
	{
		const int objectCount = 100;

		GameObject[] _objects;


		void Start()
		{
			GenerateNewObjects();
		}


		public void GenerateNewObjects()
		{
			if( _objects != null ){
				for( int i=0; i<_objects.Length; i++ ) Destroy( _objects[i] );
			}
			_objects = new GameObject[objectCount];
			for( int i=0; i<_objects.Length; i++ ){
				_objects[i] = GameObject.CreatePrimitive( PrimitiveType.Sphere );
				_objects[i].transform.parent = transform;
				_objects[i].transform.localScale = Vector3.one * Random.Range( 0.1f, 1f );
				_objects[i].transform.position = Random.insideUnitSphere * 2;
			}
		}
		
		
		public void ExportToBinarySTL()
		{
			string filePath = DefaultDirectory() + "/stl_example_binary.stl";
			bool success = STL.Export( _objects, filePath );
			if( success ){
				Debug.Log( "Exported " + objectCount + " objects to binary STL file." + System.Environment.NewLine + filePath );
			}
		}


		public void ExportToTextSTL()
		{
			string filePath = DefaultDirectory() + "/stl_example_text.stl";
			bool asASCII = true;
			bool success = STL.Export( _objects, filePath, asASCII );
			if( success ){
				Debug.Log( "Exported " + objectCount + " objects to text based STL file." + System.Environment.NewLine + filePath );
			}
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
	}
}