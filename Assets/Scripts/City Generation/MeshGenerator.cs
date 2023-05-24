using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    public int numVertices;
    public List<Vector3> meshFaceTriangleVertices;
    public List<Vector3> bottomMeshVertices;
    public List<Vector3> topMeshVertices;
    public Vector3[] vertices;
    public int[] triangles;

    // Array of UV coordinates
    private Vector2[] uvCoordinates;

    private bool alreadyGenerated = false;

    // Update is called once per frame
    void Update()
    {
        if (meshFaceTriangleVertices.Count != 0 && !alreadyGenerated) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Vector3 bottomCentroid = CalculateCentroid(bottomMeshVertices);
            Vector3 topCentroid = CalculateCentroid(topMeshVertices);

            bottomMeshVertices = OrderVertices(bottomMeshVertices, bottomCentroid);
            topMeshVertices = OrderVertices(topMeshVertices, topCentroid);
            bottomMeshVertices.AddRange(topMeshVertices);
            bottomMeshVertices.AddRange(meshFaceTriangleVertices);
            vertices = PointsRelativeToGameObject(bottomMeshVertices).ToArray();
            CreateShape(vertices);
            UpdateMesh();
            SetScale();
            alreadyGenerated = true;
        }
    }

    Vector3 CalculateCentroid(List<Vector3> vertices)
    {
        Vector3 centroid = Vector3.zero;

        foreach (Vector3 vertex in vertices)
        {
            centroid += vertex;
        }

        return centroid / vertices.Count;
    }

    List<Vector3> OrderVertices(List<Vector3> vertices, Vector3 centroid)
    {
        return vertices.OrderBy(v => Mathf.Atan2(v.z - centroid.z, v.x - centroid.x)).ToList();
    }

    List<Vector3> PointsRelativeToGameObject(List<Vector3> verticesToTransform) {

        var localSpacePoints = new List<Vector3>();

        // Calculate the local space coordinates of each point
        foreach (Vector3 vertex in verticesToTransform)
        {
            Vector3 localSpacePoint = gameObject.transform.InverseTransformPoint(vertex);
            localSpacePoints.Add(localSpacePoint);
        }
        return localSpacePoints;
    }
    void CreateShape(Vector3[] vertices) {

        // generate side triangles
        List<int> sideTriangles = new List<int>();
        numVertices = topMeshVertices.Count;
        for (int i = 0; i < numVertices; i++)
        {
            int bottomIdx1 = i;
            int bottomIdx2 = (i + 1) % numVertices;
            int topIdx1 = i + numVertices; // Assumes the top vertices are stored right after the bottom vertices in the vertex list
            int topIdx2 = ((i + 1) % numVertices) + numVertices;

            // First triangle
            sideTriangles.Add(bottomIdx1);
            sideTriangles.Add(topIdx1);
            sideTriangles.Add(bottomIdx2);

            // Second triangle
            sideTriangles.Add(bottomIdx2);
            sideTriangles.Add(topIdx1);
            sideTriangles.Add(topIdx2);
        }
        List<int> trianglesAsList = new();
        trianglesAsList.AddRange(sideTriangles);
        
        for (int i = topMeshVertices.Count * 2; i < vertices.Length; i++)
        {
            trianglesAsList.Add(i);
        }
        triangles = trianglesAsList.ToArray();
    }

    private void CalculateUVCoordinates()
    {
        // Find the minimum and maximum X and Z values among all vertices
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            minX = Mathf.Min(minX, vertex.x);
            maxX = Mathf.Max(maxX, vertex.x);
            minZ = Mathf.Min(minZ, vertex.z);
            maxZ = Mathf.Max(maxZ, vertex.z);
        }

        // Initialize the UV coordinates array
        uvCoordinates = new Vector2[vertices.Length];

        // Calculate UV coordinates for each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            // Calculate U and V based on X and Z coordinates
            float u = (vertex.x - minX) / (maxX - minX);
            float v = (vertex.z - minZ) / (maxZ - minZ);

            // Assign the UV coordinate to the array
            uvCoordinates[i] = new Vector2(u, v);
        }
    }

    void UpdateMesh() 
    {
        mesh.Clear();


        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Calculate UV coordinates
        CalculateUVCoordinates();
        // Assign the UV coordinates to the mesh
        mesh.uv = uvCoordinates;
        mesh.RecalculateNormals();
    }

    void SetScale() {
        transform.localScale = new(0.65f,0.65f,0.65f);
    }
}
