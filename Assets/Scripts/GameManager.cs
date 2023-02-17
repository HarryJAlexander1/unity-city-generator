using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform cityPrefab;
    public Transform planePrefab;
    public Transform plane;

    // Start is called before the first frame update
    void Start()
    {
        CreatePlane(new(45, 0, 213), new(25, 1, 15));
        CreateCity();
    }

    void CreateCity() {
        Vector3 cityPos = new(plane.position.x - (plane.localScale.x * 5), plane.position.y, plane.position.z - (plane.localScale.z * 5));
        Transform city = Instantiate(cityPrefab, cityPos, Quaternion.identity);
        city.SetParent(plane);
    }

    void CreatePlane(Vector3 pos, Vector3 dim) {
        plane = Instantiate(planePrefab, pos, Quaternion.identity);
        plane.localScale = dim;
    }
}
