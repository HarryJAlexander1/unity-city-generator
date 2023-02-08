using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public Transform buildingPrefab;
    public GameObject gameManagerObject;
    public GameObject planePrefab;
    public Vector3 size;
    private Transform[,] grid;

    private void Awake()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateGrid()
    {
        // find the game manager and get size parameter
       /* gameManagerObject = GameObject.Find("Game Manager");
        GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        size = gameManagerScript.citySize;*/

        // Create a grid of buildings
        grid = new Transform[(int)size.x, (int)size.z];
        for (int x = 0; x < size.x; x += 20)
        {
            for (int z = 0; z < size.z; z += 20)
            {
                Transform newBuilding;
                newBuilding = Instantiate(buildingPrefab, new Vector3(x, 0, z), Quaternion.identity);
                newBuilding.name = string.Format("({0}, 0, {1})", x, z);
                newBuilding.SetParent(transform);
                newBuilding.GetComponent<BuildingBehaviour>().position = new Vector3(x, 0, z);
                grid[x, z] = newBuilding;
            }
        }

        // Add Plane
        // planeClone = GameObject.Find("Plane");
        GameObject c = Instantiate(planePrefab, new Vector3((size.x / 2)- 10 , (size.y) - 6, (size.z / 2) - 10), Quaternion.identity);
        c.transform.localScale = new Vector3((size.x / 10) , 5, (size.z / 10));
        c.transform.SetParent(transform);
    }

}
