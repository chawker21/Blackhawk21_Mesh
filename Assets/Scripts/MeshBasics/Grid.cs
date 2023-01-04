using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


// Require that the GameObject this script is attached to has a MeshFilter and MeshRenderer component
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    // Array of vertices that make up the mesh
    private Vector3[] vertices;

    // Number of vertices in the x and y directions
    public int xSize, ySize;

    // Mesh object
    private Mesh mesh;

    private void Awake () {
        // Generate the mesh when the script is first run
        Generate();
    }

    private void Generate()
    {
        // Get the MeshFilter component and set its mesh to a new mesh object
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        // Set the name of the mesh
        mesh.name = "Procedural Grid";
        
        // Initialize the array of vertices, UV coordinates, and tangents
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        
        // For each vertex, set its position, UV coordinate, and tangent
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        // Set the vertices, UV coordinates, and tangents of the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
        
        // Initialize the array of triangles
        int[] triangles = new int[xSize * ySize * 6];
        // For each quad, set the indices of its vertices to form two triangles
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        // Set the triangles of the mesh
        mesh.triangles = triangles;
        // Recalculate the normals of the mesh
        mesh.RecalculateNormals();
    }


    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}