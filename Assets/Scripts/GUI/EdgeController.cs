using UnityEngine;

public class EdgeController : MonoBehaviour
{
    public Vector3 position1;
    public Vector3 position2;
    private GameObject edge;
    public Color edgeColor;

    void Awake()
    {
        gameObject.tag = "Edge";
        edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        edge.tag = "Edge";
        edge.transform.parent = transform;
    }

    void Update()
    {
        Vector3 direction = position2 - position1;
        float distance = direction.magnitude;
        edge.transform.localScale = new Vector3(0.5f, 0.5f, distance);
        edge.transform.position = position1 + direction / 2;
        edge.transform.rotation = Quaternion.LookRotation(direction);
        edge.GetComponent<Renderer>().material.color = edgeColor;
    }
}