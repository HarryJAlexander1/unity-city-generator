using UnityEngine;

public class AdjustMeshPivot : MonoBehaviour
{
    void Start()
    {
        // Get the mesh filter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Mesh filter component not found.");
            return;
        }

        // Create a new empty game object
        GameObject pivotObject = new GameObject("Pivot");
        pivotObject.transform.parent = transform.parent;
        pivotObject.transform.localPosition = Vector3.zero;
        pivotObject.transform.localRotation = Quaternion.identity;
        pivotObject.transform.localScale = Vector3.one;

        // Set the parent of the mesh to the new game object
        meshFilter.transform.parent = pivotObject.transform;

        // Move the new game object to the center of the mesh
        pivotObject.transform.position = meshFilter.sharedMesh.bounds.center;

        // Reset the position of the parent game object
        transform.localPosition = Vector3.zero;

        // Move the new game object back to the parent game object and reset its position, rotation, and scale
        pivotObject.transform.parent = transform;
        pivotObject.transform.localPosition = Vector3.zero;
        pivotObject.transform.localRotation = Quaternion.identity;
        pivotObject.transform.localScale = Vector3.one;
    }
}
