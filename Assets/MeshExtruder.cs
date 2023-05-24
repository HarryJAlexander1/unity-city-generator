using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshExtruder : MonoBehaviour
{
    public float extrusionHeight = 1f;

    public void Extrude()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh originalMesh = meshFilter.sharedMesh;
        Mesh extrudedMesh = ExtrudeMesh(originalMesh, extrusionHeight);
        meshFilter.sharedMesh = extrudedMesh;
    }

    private Mesh ExtrudeMesh(Mesh originalMesh, float height)
    {
        Mesh extrudedMesh = new Mesh();
        Vector3[] originalVertices = originalMesh.vertices;
        int[] originalTriangles = originalMesh.triangles;
        int vertexCount = originalVertices.Length;

        // Duplicate vertices and offset them along the Y axis
        Vector3[] extrudedVertices = new Vector3[vertexCount * 2];

        // Calculate the average Y value for the top vertices
        float avgY = 0f;
        for (int i = 0; i < vertexCount; i++)
        {
            avgY += originalVertices[i].y;
        }
        avgY /= vertexCount;
        avgY += height;

        for (int i = 0; i < vertexCount; i++)
        {
            extrudedVertices[i] = originalVertices[i];
            extrudedVertices[i + vertexCount] = new Vector3(originalVertices[i].x, avgY, originalVertices[i].z);
        }

        // Create the extrusion faces
        int[] extrudedTriangles = new int[originalTriangles.Length * 2 + vertexCount * 6];
        int triangleIndex = 0;

        // Add the original and duplicated triangles (with corrected winding order)
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            extrudedTriangles[triangleIndex++] = originalTriangles[i];
            extrudedTriangles[triangleIndex++] = originalTriangles[i + 1];
            extrudedTriangles[triangleIndex++] = originalTriangles[i + 2];

            extrudedTriangles[triangleIndex++] = originalTriangles[i] + vertexCount;
            extrudedTriangles[triangleIndex++] = originalTriangles[i + 2] + vertexCount;
            extrudedTriangles[triangleIndex++] = originalTriangles[i + 1] + vertexCount;
        }

        // Add the side faces
        for (int i = 0; i < vertexCount; i++)
        {
            int nextIndex = (i + 1) % vertexCount;

            extrudedTriangles[triangleIndex++] = i;
            extrudedTriangles[triangleIndex++] = nextIndex;
            extrudedTriangles[triangleIndex++] = i + vertexCount;

            extrudedTriangles[triangleIndex++] = nextIndex;
            extrudedTriangles[triangleIndex++] = nextIndex + vertexCount;
            extrudedTriangles[triangleIndex++] = i + vertexCount;
        }

        extrudedMesh.vertices = extrudedVertices;
        extrudedMesh.triangles = extrudedTriangles;

        // Recalculate normals for top and bottom surfaces separately
        extrudedMesh.RecalculateNormals();
        Vector3[] normals = extrudedMesh.normals;

        // Average normals for top and bottom surfaces
        Vector3 topNormal = Vector3.zero;
        Vector3 bottomNormal = Vector3.zero;
        for (int i = 0; i < vertexCount; i++)
        {
            bottomNormal += normals[i];
            topNormal += normals[i + vertexCount];
        }
        bottomNormal.Normalize();
        topNormal.Normalize();

        // Assign averaged normals to top and bottom vertices
        for (int i = 0; i < vertexCount; i++)
        {
            normals[i] = bottomNormal;
            normals[i + vertexCount] = topNormal;
        }

        extrudedMesh.normals = normals;
        extrudedMesh.RecalculateBounds();
        extrudedMesh.RecalculateTangents();

        return extrudedMesh;
    }
}