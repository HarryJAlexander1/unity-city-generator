using System.Collections.Generic;
using UnityEngine;

public class VertexController : MonoBehaviour
{
    public GameObject GUI;
    Vector3 mousePosition;
    bool isDragging = false;

    private void Awake()
    {
        GUI = GameObject.FindGameObjectWithTag("Canvas");
    }

    void OnMouseDown()
    {
        UIController uiController = GUI.GetComponent<UIController>();
        if (uiController.vertices.Contains(gameObject))
        {
            mousePosition = GetMousePos() - transform.position;
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentPosition = GetMousePos() - mousePosition;
            transform.position = new Vector3(currentPosition.x, transform.position.y, currentPosition.z);
            mousePosition = GetMousePos() - transform.position;
        }
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}