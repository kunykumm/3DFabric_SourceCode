/*
	AUTHOR
	=============
	Created by Carl Emil Carlsen.
	Copyright 2015-2019 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
    
    
	LICENSE
	=======
	This is a Unity Asset Store product.
	https://www.assetstore.unity3d.com/en/#!/content/3397
	
    
	VERSION
	=============
	1.6.1
	
    
	DESCRIPTION
	=============
	STL is an extension for Unity that enables export of meshes to the STL file format.
	STL files are widely used for rapid prototyping and computer-aided manufacturing (3D printing).
*/	

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class STL
{
	const string logPrepend = "<b>[STL]</b> ";


	/// <summary>
	/// Exports all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game object (and it's children) to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
	/// </summary>
	public static bool Export(GameObject gameObject, string filePath, bool asASCII = false)
    {
        return Export(new GameObject[] { gameObject }, filePath, asASCII);
    }


    /// <summary>
    /// Exports all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game objects (and their children) to a binary stl file at specified file path as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(GameObject[] gameObjects, string filePath, bool asASCII = false)
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes(gameObjects, out meshes, out matrices);
        return Export(meshes, matrices, filePath, asASCII);
    }


    /// <summary>
    /// Exports mesh found in supplied MeshFilter to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(MeshFilter filter, string filePath, bool asASCII = false)
    {
        if (!filter.sharedMesh)
        {
            Debug.LogError(logPrepend + "Export failed. Meshfilter has no mesh.\n");
            return false;
        }
        return Export(new MeshFilter[] { filter }, filePath, asASCII);
    }


    /// <summary>
    /// Exports all meshes found in supplied MeshFilters to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(MeshFilter[] filters, string filePath, bool asASCII = false)
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes(filters, out meshes, out matrices);
        return Export(meshes, matrices, filePath, asASCII);
    }


    /// <summary>
    /// Exports mesh found in supplied SkinnedMeshRenderer component to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(SkinnedMeshRenderer skin, string filePath, bool asASCII = false)
    {
        if (!skin.sharedMesh)
        {
            Debug.LogError(logPrepend + "Export failed. SkinnedMeshRenderer has no mesh.\n");
            return false;
        }
        return Export(new SkinnedMeshRenderer[] { skin }, filePath, asASCII);
    }


    /// <summary>
    /// Exports all meshes found in supplied SkinnedMeshRenderer components to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(SkinnedMeshRenderer[] skins, string filePath, bool asASCII = false)
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes(skins, out meshes, out matrices);
        return Export(meshes, matrices, filePath, asASCII);
    }


    /// <summary>
    /// Exports a mesh to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(Mesh mesh, string filePath, bool asASCII = false)
    {
        return Export(new Mesh[] { mesh }, new Matrix4x4[] { Matrix4x4.identity }, filePath, asASCII);
    }


    /// <summary>
    /// Exports a mesh to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(Mesh[] meshes, string filePath, bool asASCII = false)
    {
        Matrix4x4[] matrices = new Matrix4x4[meshes.Length];
        for (int m = 0; m < matrices.Length; m++) matrices[m] = Matrix4x4.identity;
        return Export(meshes, matrices, filePath, asASCII);
    }

    /// <summary>
    /// Exports a mesh with matrix transformation to a binary stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export(Mesh mesh, Matrix4x4 matrix, string filePath, bool asASCII = false)
    {
        return Export(new Mesh[] { mesh }, new Matrix4x4[] { matrix }, filePath, asASCII);
    }


    /// <summary>
    /// Exports meshes with matrix transformations to a stl file at specified file path formated as binary (default) or ASCII. Returns success status.
    /// </summary>
    public static bool Export( Mesh[] meshes, Matrix4x4[] matrices, string filePath, bool asASCII = false )
    {
        return asASCII ? ExportSTLAsASCII( meshes, matrices, filePath ) : ExportSTLAsBinary( meshes, matrices, filePath );
    }
		

	static bool ExportSTLAsBinary( Mesh[] meshes, Matrix4x4[] matrices, string filePath )
	{
		// Check array lengths.
		if( meshes.Length != matrices.Length ){
			Debug.LogError( logPrepend + "Mesh array length and matrix array length must match.\n" );
			return false;
		}

		try
		{
			using( BinaryWriter writer = new BinaryWriter( File.Open( filePath, FileMode.Create ) ) )
			{
				// Write header.
				writer.Write( new char[ 80 ] );
				
				// Count all triangles and write.
				int triangleIndexCount = 0;
				foreach( Mesh mesh in meshes ) {
					for( int s = 0; s < mesh.subMeshCount; s++ ) triangleIndexCount += mesh.GetTriangles( s ).Length;
				}
				uint triangleCount = (uint) ( triangleIndexCount / 3 );
				writer.Write( triangleCount );
				
				// For each mesh ...
				int i;
				short attribute = 0;
				Vector3 u, v;
				Vector3 normal = Vector3.zero;
				int[] triangles;
				Vector3[] vertices;
                Vector3 tempVec3;
				for( int m=0; m<meshes.Length; m++ )
				{
					// Get matrix and correct mirrored x-axis.
					Matrix4x4 matrix = Matrix4x4.Scale( new Vector3( -1, 1, 1 ) ) * matrices[m];

					// Get vertices and tranform them.
					vertices = meshes[m].vertices;
					for( int vx = 0; vx < vertices.Length; vx++ ) vertices[vx] = matrix.MultiplyPoint( vertices[ vx ] );
					
					// For each sub mesh ...
					for( int s = 0; s < meshes[m].subMeshCount; s++ )
					{
						// Get trianlges.
						triangles = meshes[m].GetTriangles( s );
						
						// For each triangle ...
						for( int t = 0; t < triangles.Length; t += 3 )
						{
							// Calculate and write normal.
							u = vertices[ triangles[t+1] ] - vertices[ triangles[t] ];
							v = vertices[ triangles[t+2] ] - vertices[ triangles[t] ];
							normal.Set( u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x );
							normal.Normalize();
							for( i = 0; i < 3; i++ ) writer.Write( normal[i] );
                            
                            // Write vertices.
                            tempVec3 = vertices[triangles[t+2]];
                            for( i = 0; i < 3; i++ ) writer.Write( tempVec3[i] );
                            tempVec3 = vertices[triangles[t+1]];
                            for( i = 0; i < 3; i++ ) writer.Write( tempVec3[i] );
                            tempVec3 = vertices[triangles[t]];
                            for( i = 0; i < 3; i++ ) writer.Write( tempVec3[i] );
							
							// Write attribute byte count.
							writer.Write( attribute );
						}
					}
				}
				
				// End of file.
				writer.Close();
			}
		}
		catch( Exception e ){
			Debug.LogWarning( logPrepend + "Failed exporting binary STL file at: " + filePath + "\n" + e );
			return false;
		}

		// Success!
		return true;
	}


	static bool ExportSTLAsASCII( Mesh[] meshes, Matrix4x4[] matrices, string filePath )
	{
		// Check array lengths.
		if( meshes.Length != matrices.Length ){
			Debug.LogError( logPrepend + "Mesh array length and matrix array length must match.\n" );
			return false;
		}

		// ASCII STL files may not contain vertices in negative space because of sign-mantissa-"e"-sign-exponent formating.
		// Therefore we first need to transform the vertices and find the needed translation to get all vertices into positive space.
		// https://en.wikipedia.org/wiki/STL_(file_format)
        List<Vector3[]> transformedVertices = TransformAndTranslateVerticesIntoPositiveSpace( meshes, matrices );
		
		// Begin export ...
		try
		{
			bool append = false;
			using( StreamWriter sw = new StreamWriter( filePath, append ) ) 
			{
				const string name = "Unity Mesh";

				// Write header to disk.
				sw.WriteLine( "solid " + name );
				
				// For each mesh filter ...
				Vector3 u, v;
				Vector3 normal = Vector3.zero;
				int[] triangles;
				Vector3[] vertices;
				System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture( "en-US" );
				for( int m=0; m<meshes.Length; m++ )
				{
					// Create a new string builder for each mesh to avoid out of memory errors.
					StringBuilder sb = new StringBuilder();

					// Get transformed vertices.
					vertices = transformedVertices[m];
					
					// For each sub mesh ...
					for( int s = 0; s < meshes[m].subMeshCount; s++ )
					{
						// Get trianlges.
						triangles = meshes[m].GetTriangles( s );
						
						// For each triangle ...
						for( int t = 0; t < triangles.Length; t += 3 )
						{
							// Calculate and write normal.
							u = vertices[ triangles[ t+1 ] ] - vertices[ triangles[t] ];
							v = vertices[ triangles[ t+2 ] ] - vertices[ triangles[t] ];
							normal.Set( u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x );
							normal.Normalize();
							sb.AppendLine( "facet normal " + normal.x.ToString("e",ci) + " " + normal.y.ToString("e",ci) + " " + normal.z.ToString("e",ci) );

							// Begin triangle.
							sb.AppendLine( "outer loop" );
							
							// Write vertices.
							sb.AppendLine( "vertex " + vertices[ triangles[ t+2 ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t+2 ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t+2 ] ].z.ToString("e",ci) );
							sb.AppendLine( "vertex " + vertices[ triangles[ t+1 ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t+1 ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t+1 ] ].z.ToString("e",ci) );
							sb.AppendLine( "vertex " + vertices[ triangles[ t ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t ] ].z.ToString("e",ci) );

							// End triangle.
							sb.AppendLine( "endloop" );
							sb.AppendLine( "endfacet" );
						}
					}
					
					// Write string builder memory to the disk.
					sw.Write( sb.ToString() );
				}
				
				// Write ending to disk and close writer.
				sw.WriteLine( "endsolid " + name );
				sw.Close();
			}
		}
		catch( Exception e ){
			Debug.LogWarning( logPrepend + "Failed exporting ASCII STL file at: " + filePath + "\n" + e );
			return false;
		}

		// Success!
		return true;
	}


    /// <summary>
    /// Converts all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game object (and it's children) to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( GameObject gameObject, out byte[] stlAsBinary )
    {
        return Convert( new GameObject[]{ gameObject }, out stlAsBinary );
    }


    /// <summary>
    /// Converts all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game objects (and their children) to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( GameObject[] gameObjects, out byte[] stlAsBinary )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( gameObjects, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsBinary );
    }


    /// <summary>
    /// Converts mesh found in supplied MeshFilter to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( MeshFilter filter, out byte[] stlAsBinary )
    {
        stlAsBinary = new byte[0];
        if( !filter.sharedMesh ){
            Debug.LogError( logPrepend + "Export failed. Meshfilter has no mesh.\n" );
            return false;
        }
        return Convert( new MeshFilter[]{ filter }, out stlAsBinary );
    }


    /// <summary>
    /// Converts all meshes found in supplied MeshFilters to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( MeshFilter[] filters, out byte[] stlAsBinary )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( filters, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsBinary );
    }


    /// <summary>
    /// Converts mesh found in supplied SkinnedMeshRenderer component to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( SkinnedMeshRenderer skin, out byte[] stlAsBinary )
    {
        stlAsBinary = new byte[0];
        if( !skin.sharedMesh ){
            Debug.LogError( logPrepend + "Export failed. SkinnedMeshRenderer has no mesh.\n" );
            return false;
        }
        return Convert( new SkinnedMeshRenderer[]{ skin }, out stlAsBinary );
    }


    /// <summary>
    /// Converts all meshes found in supplied SkinnedMeshRenderer components to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( SkinnedMeshRenderer[] skins, out byte[] stlAsBinary )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( skins, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsBinary );
    }


    /// <summary>
    /// Converts a mesh to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( Mesh mesh, out byte[] stlAsBinary )
    {
        return Convert( new Mesh[]{ mesh }, new Matrix4x4[]{ Matrix4x4.identity }, out stlAsBinary );
    }


    /// <summary>
    /// Converts a mesh to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( Mesh[] meshes, out byte[] stlAsBinary )
    {
        Matrix4x4[] matrices = new Matrix4x4[meshes.Length];
        for( int m = 0; m < matrices.Length; m++ ) matrices[m] = Matrix4x4.identity;
        return Convert( meshes, matrices, out stlAsBinary );
    }

    /// <summary>
    /// Converts a mesh with matrix transformation to binary stl data. Returns success status.
    /// </summary>
    public static bool Convert( Mesh mesh, Matrix4x4 matrix, out byte[] stlAsBinary )
    {
        return Convert( new Mesh[]{ mesh }, new Matrix4x4[]{ matrix }, out stlAsBinary );
    }


    /// <summary>
    /// Converts all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game object (and it's children) to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( GameObject gameObject, out string stlAsASCII )
    {
        return Convert( new GameObject[]{ gameObject }, out stlAsASCII );
    }


    /// <summary>
    /// Converts all meshes found in MeshFilter and SkinnedMeshRenderer components attached to the supplied game objects (and their children) to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( GameObject[] gameObjects, out string stlAsASCII )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( gameObjects, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsASCII );
    }


    /// <summary>
    /// Converts mesh found in supplied MeshFilter to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( MeshFilter filter, out string stlAsASCII )
    {
        stlAsASCII = string.Empty;
        if( !filter.sharedMesh ){
            Debug.LogError( logPrepend + "Export failed. Meshfilter has no mesh.\n" );
            return false;
        }
        return Convert( new MeshFilter[]{ filter }, out stlAsASCII );
    }


    /// <summary>
    /// Converts all meshes found in supplied MeshFilters to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( MeshFilter[] filters, out string stlAsASCII )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( filters, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsASCII );
    }


    /// <summary>
    /// Converts mesh found in supplied SkinnedMeshRenderer component to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( SkinnedMeshRenderer skin, out string stlAsASCII )
    {
        stlAsASCII = string.Empty;
        if( !skin.sharedMesh ){
            Debug.LogError( logPrepend + "Export failed. SkinnedMeshRenderer has no mesh.\n" );
            return false;
        }
        return Convert( new SkinnedMeshRenderer[]{ skin }, out stlAsASCII );
    }


    /// <summary>
    /// Converts all meshes found in supplied SkinnedMeshRenderer components to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( SkinnedMeshRenderer[] skins, out string stlAsASCII )
    {
        Mesh[] meshes;
        Matrix4x4[] matrices;
        GetMeshesAndMatrixes( skins, out meshes, out matrices );
        return Convert( meshes, matrices, out stlAsASCII );
    }


    /// <summary>
    /// Converts a mesh to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( Mesh mesh, out string stlAsASCII )
    {
        return Convert( new Mesh[]{ mesh }, new Matrix4x4[]{ Matrix4x4.identity }, out stlAsASCII );
    }


    /// <summary>
    /// Converts a mesh to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( Mesh[] meshes, out string stlAsASCII )
    {
        Matrix4x4[] matrices = new Matrix4x4[meshes.Length];
        for( int m = 0; m < matrices.Length; m++ ) matrices[m] = Matrix4x4.identity;
        return Convert( meshes, matrices, out stlAsASCII );
    }

    /// <summary>
    /// Converts a mesh with matrix transformation to stl ASCII. Returns success status.
    /// </summary>
    public static bool Convert( Mesh mesh, Matrix4x4 matrix, out string stlAsASCII )
    {
        return Convert( new Mesh[]{ mesh }, new Matrix4x4[]{ matrix }, out stlAsASCII );
    }


    /// <summary>
    /// Convert meshes with matrix transformations to stl as binary data. Returns success status.
    /// </summary>
    public static bool Convert( Mesh[] meshes, Matrix4x4[] matrices, out byte[] stlAsBinary )
    {
        stlAsBinary = new byte[0];

        // Check array lengths.
        if( meshes.Length != matrices.Length ){
            Debug.LogError( logPrepend + "Mesh array length and matrix array length must match.\n" );
            return false;
        }

        List<byte> byteList = new List<byte>();
        byte[] tempBytes;

        try
        {
            // Write header.
            byteList.AddRange( Encoding.GetEncoding("ascii").GetBytes( new char[ 80 ] ) );

            // Count all triangles and write.
            int triangleIndexCount = 0;
            foreach( Mesh mesh in meshes ) {
                for( int s = 0; s < mesh.subMeshCount; s++ ) triangleIndexCount += mesh.GetTriangles( s ).Length;
            }
            uint triangleCount = (uint) ( triangleIndexCount / 3 );
            tempBytes = BitConverter.GetBytes( triangleCount );
            //if( BitConverter.IsLittleEndian ) Reverse4Bytes( tempBytes );
            byteList.AddRange( tempBytes );

            // For each mesh ...
            int i;
            short attribute = 0; // 16 bit integer
            Vector3 u, v;
            Vector3 normal = Vector3.zero;
            int[] triangles;
            Vector3[] vertices;
            Vector3 tempVec3;
            for( int m=0; m<meshes.Length; m++ )
            {
                // Get matrix and correct mirrored x-axis.
                Matrix4x4 matrix = Matrix4x4.Scale( new Vector3( -1, 1, 1 ) ) * matrices[m];

                // Get vertices and tranform them.
                vertices = meshes[m].vertices;
                for( int vx = 0; vx < vertices.Length; vx++ ) vertices[vx] = matrix.MultiplyPoint( vertices[ vx ] );

                // For each sub mesh ...
                for( int s = 0; s < meshes[m].subMeshCount; s++ )
                {
                    // Get trianlges.
                    triangles = meshes[m].GetTriangles( s );

                    // For each triangle ...
                    for( int t = 0; t < triangles.Length; t += 3 )
                    {
                        // Calculate and write normal.
                        u = vertices[ triangles[t+1] ] - vertices[ triangles[t] ];
                        v = vertices[ triangles[t+2] ] - vertices[ triangles[t] ];
                        normal.Set( u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x );
                        normal.Normalize();
                        for( i = 0; i < 3; i++ ){
                            tempBytes = BitConverter.GetBytes( normal[i] );
                            //if( BitConverter.IsLittleEndian ) Reverse4Bytes( tempBytes );
                            byteList.AddRange( tempBytes );
                        }

                        // Write vertices.
                        tempVec3 = vertices[triangles[t+2]];
                        for( i = 0; i < 3; i++ ){
                            tempBytes = BitConverter.GetBytes( tempVec3[i] );
                            //if( BitConverter.IsLittleEndian ) Reverse4Bytes( tempBytes );
                            byteList.AddRange( tempBytes );
                        }
                        tempVec3 = vertices[triangles[t+1]];
                        for( i = 0; i < 3; i++ ){
                            tempBytes = BitConverter.GetBytes( tempVec3[i] );
                            //if( BitConverter.IsLittleEndian ) Reverse4Bytes( tempBytes );
                            byteList.AddRange( tempBytes );
                        }
                        tempVec3 = vertices[triangles[t]];
                        for( i = 0; i < 3; i++ ){
                            tempBytes = BitConverter.GetBytes( tempVec3[i] );
                            //if( BitConverter.IsLittleEndian ) Reverse4Bytes( tempBytes );
                            byteList.AddRange( tempBytes );
                        }

                        // Write attribute byte count.
                        tempBytes = BitConverter.GetBytes( attribute );
                        byteList.AddRange( tempBytes ); // we don't care checking endianess, because the value is always 0.
                    }
                }
            }
        }
        catch( Exception e ){
            Debug.LogWarning( logPrepend + "Failed converting to binary STL data.\n" + e );
            return false;
        }

        // To array.
        stlAsBinary = byteList.ToArray();

        // Success!
        return true;
    }
	

    /// <summary>
    /// Convert meshes with matrix transformations to a stl formated as ASCII text. Returns success status.
    /// </summary>
    public static bool Convert( Mesh[] meshes, Matrix4x4[] matrices, out string stlAsASCII )
    {
        stlAsASCII = string.Empty;

        // Check array lengths.
        if( meshes.Length != matrices.Length ){
            Debug.LogError( logPrepend + "Mesh array length and matrix array length must match.\n" );
            return false;
        }

        // ASCII STL files may not contain vertices in negative space because of sign-mantissa-"e"-sign-exponent formating.
        // Therefore we first need to transform the vertices and find the needed translation to get all vertices into positive space.
        // https://en.wikipedia.org/wiki/STL_(file_format)
        List<Vector3[]> transformedVertices = TransformAndTranslateVerticesIntoPositiveSpace( meshes, matrices );

        // Create a new string builder. 
		StringBuilder sb = new StringBuilder();

        try // Catch errors, StringBuilder may run out of memory. 
        {
            const string name = "Unity Mesh";

            // Write header to disk.
            sb.AppendLine( "solid " + name );

            // For each mesh filter ...
            Vector3 u, v;
            Vector3 normal = Vector3.zero;
            int[] triangles;
            Vector3[] vertices;
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CreateSpecificCulture( "en-US" );
            for( int m=0; m<meshes.Length; m++ )
            {

                // Get transformed vertices.
                vertices = transformedVertices[m];

                // For each sub mesh ...
                for( int s = 0; s < meshes[m].subMeshCount; s++ )
                {
                    // Get trianlges.
                    triangles = meshes[m].GetTriangles( s );

                    // For each triangle ...
                    for( int t = 0; t < triangles.Length; t += 3 )
                    {
                        // Calculate and write normal.
                        u = vertices[ triangles[ t+1 ] ] - vertices[ triangles[t] ];
                        v = vertices[ triangles[ t+2 ] ] - vertices[ triangles[t] ];
                        normal.Set( u.y * v.z - u.z * v.y, u.z * v.x - u.x * v.z, u.x * v.y - u.y * v.x );
                        normal.Normalize();
                        sb.AppendLine( "facet normal " + normal.x.ToString("e",ci) + " " + normal.y.ToString("e",ci) + " " + normal.z.ToString("e",ci) );

                        // Begin triangle.
                        sb.AppendLine( "outer loop" );

                        // Write vertices.
                        sb.AppendLine( "vertex " + vertices[ triangles[ t+2 ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t+2 ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t+2 ] ].z.ToString("e",ci) );
                        sb.AppendLine( "vertex " + vertices[ triangles[ t+1 ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t+1 ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t+1 ] ].z.ToString("e",ci) );
                        sb.AppendLine( "vertex " + vertices[ triangles[ t ] ].x.ToString("e",ci) + " " + vertices[ triangles[ t ] ].y.ToString("e",ci) + " " + vertices[ triangles[ t ] ].z.ToString("e",ci) );

                        // End triangle.
                        sb.AppendLine( "endloop" );
                        sb.AppendLine( "endfacet" );
                    }
                }
            }

            // Write ending to disk and close writer.
            sb.AppendLine( "endsolid " + name );
        }
        catch( Exception e ){
            Debug.LogWarning( logPrepend + "Failed converting meshes to STL ASCII text.\n" + e );
            return false;
        }

        // To string!
        stlAsASCII = sb.ToString();

        // Success!
        return true;
    }


    static List<Vector3[]> TransformAndTranslateVerticesIntoPositiveSpace( Mesh[] meshes, Matrix4x4[] matrices )
    {
        Bounds bounds = new Bounds();
        List<Vector3[]> transformedVertices = new List<Vector3[]>();
        for( int m=0; m<meshes.Length; m++ )
        {
            // Get matrix and correct mirrored x-axis.
            Matrix4x4 matrix = Matrix4x4.Scale( new Vector3( -1, 1, 1 ) ) * matrices[m];

            // Get vertices and tranform them while computing bounds.
            Vector3[] vertices = meshes[m].vertices;
            for( int vx = 0; vx < vertices.Length; vx++ ){
                vertices[vx] = matrix.MultiplyPoint( vertices[ vx ] );
                if( m == 0 && vx == 0 ) bounds.SetMinMax( vertices[vx], vertices[vx] );
                else bounds.Encapsulate( vertices[vx] );
            }
            transformedVertices.Add( vertices );
        }
        if( bounds.min.x < 0 || bounds.min.y < 0 || bounds.min.z < 0 ){
            Vector3 safeOfffset = - new Vector3( Mathf.Min( bounds.min.x, 0 ), Mathf.Min( bounds.min.y, 0 ), Mathf.Min( bounds.min.z, 0 ) );
            for( int m=0; m<meshes.Length; m++ ){
                Vector3[] vertices = transformedVertices[m];
                for( int vx = 0; vx < vertices.Length; vx++ ) vertices[vx] += safeOfffset;
            }
        }
        return transformedVertices;
    }


    /// <summary>
    /// Gets shared meshes and model matrixes from all MeshFilter and SkinnedMeshRenderer components found in provided GameObjects and their children. SkinnedMeshRenderer meshes will be baked with bone transformations.
    /// </summary>
    public static void GetMeshesAndMatrixes( GameObject[] objects, out Mesh[] meshes, out Matrix4x4[] matrices )
    {
        List<Mesh> meshList = new List<Mesh>();
        List<Matrix4x4> matrixList = new List<Matrix4x4>();

        // Get all objects including children while avoiding duplicates.
        List<Transform> transformList = new List<Transform>();
        foreach( GameObject go in objects ){
            Transform[] family = go.GetComponentsInChildren<Transform>();
            foreach( Transform member in family ) if( !transformList.Contains( member ) ) transformList.Add( member );
        }

        // Get components.
        foreach( Transform t in transformList )
        {
            MeshFilter filter = t.GetComponent<MeshFilter>();
            if( filter ){
                meshList.Add( filter.sharedMesh );
                matrixList.Add( t.localToWorldMatrix );
            }
            SkinnedMeshRenderer skin = t.GetComponent<SkinnedMeshRenderer>();
            if( skin ){
                Mesh mesh = new Mesh();
                mesh.name = skin.sharedMesh.name;
                skin.BakeMesh( mesh );
                meshList.Add( mesh );
                matrixList.Add( Matrix4x4.identity ); // The transformation is baked into the mash at this point.
			}
        }

        meshes = meshList.ToArray();
        matrices = matrixList.ToArray();
    }


    /// <summary>
    /// Gets shared meshes and model matrixes from all provided MeshFilter components.
    /// </summary>
    public static void GetMeshesAndMatrixes( MeshFilter[] filters, out Mesh[] meshes, out Matrix4x4[] matrices )
    {
        List<Mesh> meshList = new List<Mesh>();
        List<Matrix4x4> matrixList = new List<Matrix4x4>();
        for( int f=0; f<filters.Length; f++ ){
            if( filters[f] && filters[f].sharedMesh ){
                meshList.Add( filters[f].sharedMesh );
                matrixList.Add( filters[f].transform.localToWorldMatrix );
            }
        }
        meshes = meshList.ToArray();
        matrices = matrixList.ToArray();
    }


    /// <summary>
    /// Gets shared meshes and model matrixes from all provided SkinnedMeshRenderer components and bakes the current bone pose to the vertices.
    /// </summary>
    public static void GetMeshesAndMatrixes( SkinnedMeshRenderer[] skins, out Mesh[] meshes, out Matrix4x4[] matrices )
    {
        List<Mesh> meshList = new List<Mesh>();
        List<Matrix4x4> matrixList = new List<Matrix4x4>();
        for( int s=0; s<skins.Length; s++ ){
            if( skins[s] && skins[s].sharedMesh ){
                Mesh mesh = new Mesh();
                mesh.name = skins[s].sharedMesh.name;
                skins[s].BakeMesh( mesh );
                meshList.Add( mesh );
                matrixList.Add( Matrix4x4.identity ); // The transformation is baked into the mash at this point.
            }
        }
        meshes = meshList.ToArray();
        matrices = matrixList.ToArray();
    }


    // Array.Reverse is slow, so we do this manually.
    static void Reverse4Bytes( byte[] data )
    {
        byte tmp;
        tmp = data[0]; data[0] = data[3]; data[3] = tmp;
        tmp = data[1]; data[1] = data[2]; data[2] = tmp;
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportBinary(GameObject[] gameObjects, string filePath)
    {
        Export(gameObjects, filePath);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportBinary(MeshFilter[] filters, string filePath)
    {
        Export(filters, filePath);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportBinary(SkinnedMeshRenderer[] skins, string filePath)
    {
        Export(skins, filePath);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportBinary(Mesh mesh, Matrix4x4 matrix, string filePath)
    {
        Export(mesh, filePath);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportBinary(Mesh[] meshes, Matrix4x4[] matrices, string filePath)
    {
        Export(meshes, matrices, filePath);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportText(GameObject[] gameObjects, string filePath)
    {
        bool asASCII = true;
        Export(gameObjects, filePath, asASCII);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportText(Mesh mesh, Matrix4x4 matrix, string filePath)
    {
        bool asASCII = true;
        Export(mesh, filePath, asASCII);
    }


    /// <summary>
    /// Deprecated. Use the Export method instead.
    /// </summary>
    [System.Obsolete("Deprecated. Use the Export method instead.")]
    public static void ExportText(Mesh[] meshes, Matrix4x4[] matrices, string filePath)
    {
        bool asASCII = true;
        Export(meshes, filePath, asASCII);
    }
}