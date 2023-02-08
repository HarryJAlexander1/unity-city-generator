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
    public List<Transform> buildings;

    private void Awake()
    {
        CreateGrid();

        for (int i = 0; i < 20; i++) {
            int j = Random.Range(0, buildings.Count);
            ModifyBuilding(buildings, j, false, true);
        }
    }

    public void CreateGrid()
    {

        // Create a grid of buildings
        grid = new Transform[(int)size.x, (int)size.z];

        for (int x = 0; x < size.x; x += 20)
        {
            for (int z = 0; z < size.z; z += 20)
            {
                Transform newBuilding;
                newBuilding = Instantiate(buildingPrefab, new Vector3(x, 0, z), Quaternion.identity);
                newBuilding.name = string.Format("({0}, 0, {1})", (x / 20), (z / 20));
                newBuilding.SetParent(transform);
                newBuilding.GetComponent<BuildingBehaviour>().position = new Vector3(x, 0, z);
                grid[x, z] = newBuilding;

                buildings.Add(newBuilding);
            }
        }

        // Add Plane
        GameObject c = Instantiate(planePrefab, new Vector3((size.x / 2)- 10 , (size.y) - 6, (size.z / 2) - 10), Quaternion.identity);
        c.transform.localScale = new Vector3((size.x / 10) , 5, (size.z / 10));
        c.transform.SetParent(transform);
    }

    public void ModifyBuilding(List<Transform> buildingsList, int buildingIndex, bool changeLength = true, bool delete = false){

        // Check if building index number is greater than length of buildings list
        if (buildingIndex > buildingsList.Count) {
            Debug.LogError("Building Index cannot be greater than length of buildings list");
            return;
        }

        // Check if 'changeLength' parameter and 'delete' are both true
        if (changeLength == true && delete == true) {
            Debug.LogError("'changeLength' and 'delete' cannot both be true");
            return;
        }

        // Iterate through buildings
        for (int i = 0; i < buildingsList.Count; i++) {
            // If buildingIndex is the current building, either change its length or delete
            if (i == buildingIndex && changeLength == true) {
                int modifier = Random.Range(2, 5);
                float newYScale = (buildingsList[i].localScale.y * modifier);
                buildingsList[i].localScale = new Vector3(buildingsList[i].localScale.x, newYScale, buildingsList[i].localScale.z);
                // adjust position
                buildingsList[i].localPosition = new Vector3(buildingsList[i].localPosition.x, (buildingsList[i].localPosition.y + (modifier * 5) - 5), buildingsList[i].localPosition.z);

            }
            else if (i == buildingIndex && delete == true) {
                Destroy(buildingsList[i].gameObject);
                buildingsList.RemoveAt(i);
                Debug.Log("Buildings remaining " + buildingsList.Count);
            }
        }
    }

}
