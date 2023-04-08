using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    public Vector3[] worldSpacePoints;
    public Vector3[] vertices;
    public int[] triangles;

    private bool alreadyGenerated = false;

    // Update is called once per frame
    void Update()
    {
        if (worldSpacePoints.Length != 0 && !alreadyGenerated) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            pointsRelativeToGameObject();
            CreateShape();
            UpdateMesh();
            SetScale();
            alreadyGenerated = true;
        }
    }

    void pointsRelativeToGameObject() {

        var localSpacePoints = new List<Vector3>();

        // Calculate the local space coordinates of each point
        foreach (Vector3 worldSpacePoint in worldSpacePoints)
        {
            Vector3 localSpacePoint = gameObject.transform.InverseTransformPoint(worldSpacePoint);
            localSpacePoints.Add(localSpacePoint);
        }
        vertices =  localSpacePoints.ToArray();
    }
    void CreateShape() {

        /*vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0)
        };*/
        List<int> trianglesAsList = new();
        for (int i=0; i < vertices.Length; i++) {
            trianglesAsList.Add(i);
        }
        /* triangles = new int[]
         {
             0, 1, 2,
             1, 3, 2
         };*/

        triangles = trianglesAsList.ToArray();
    }

    void UpdateMesh() 
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    void SetScale() {
        transform.localScale = new(0.65f,0.65f,0.65f);
    }
}
