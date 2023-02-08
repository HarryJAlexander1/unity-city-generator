using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform cityPrefab;
    //public Vector3 citySize;
    // Start is called before the first frame update
    void Start()
    {
        CreateCity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateCity() {
        Instantiate(cityPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
