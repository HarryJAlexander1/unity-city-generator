using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public Transform blockPrefab;
    public GameObject gameManagerObject;
    public Vector3 size;
    private Transform[,] grid;
    public List<Transform> blocks;
    public int gapSize;
    //private Vector3 blockDimensions;

    private void Awake()
    {
        SetSize();
        CreateGrid();
        SetAdjacents();
    }

    public void SetSize()
    {
        gameManagerObject = GameObject.FindGameObjectWithTag("Game Manager");
        GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        var planeScale = gameManagerScript.plane.localScale;
        size = new(planeScale.x * 10 , planeScale.y, planeScale.z * 10);
    }

    public void CreateGrid()
    {
        // Create a grid of blocks
        grid = new Transform[(int)size.x, (int)size.z];
        gameManagerObject = GameObject.FindGameObjectWithTag("Game Manager");
        GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        var planePos = gameManagerScript.plane.position;

        for (int x = 0; x < size.x; x += gapSize)
        {
            for (int z = 0; z < size.z; z += gapSize)
            {
                Vector3 newBlockPos = new((x - size.x * 0.5f) + planePos.x , 0, (z - size.z * 0.5f) + planePos.z);
                Transform newBlock;
                newBlock = Instantiate(blockPrefab, newBlockPos, Quaternion.identity);
                newBlock.position = new(newBlockPos.x + (newBlock.localScale.x), newBlockPos.y + newBlock.localScale.y * 0.5f, newBlockPos.z + (newBlock.localScale.z ));
                //blockDimensions = newBlock.localScale;
                newBlock.name = string.Format("({0}, 0, {1})", (x / gapSize), (z / gapSize));
                newBlock.SetParent(transform);
                newBlock.GetComponent<BuildingBehaviour>().position = new Vector3(x, 0, z);
                grid[x, z] = newBlock;

                blocks.Add(newBlock);
            }
        }
    }

    public void SetAdjacents()
    {
        for (int x = 0; x < size.x; x += gapSize)
        {
            for (int z = 0; z < size.z; z += gapSize)
            {
                Transform building;
                building = grid[x, z];
                BuildingBehaviour buildingBehaviour = building.GetComponent<BuildingBehaviour>();
                if (x - gapSize >= 0)
                {
                    buildingBehaviour.adjacents.Add(grid[x - gapSize, z]);
                }
                if (x + gapSize < size.x)
                {
                    buildingBehaviour.adjacents.Add(grid[x + gapSize, z]);
                }
                if (z - gapSize >= 0)
                {
                    buildingBehaviour.adjacents.Add(grid[x, z - gapSize]);
                }
                if (z + gapSize < size.x)
                {
                    buildingBehaviour.adjacents.Add(grid[x, z + gapSize]);
                }
            }
        }
    }

    public void ModifyBuilding(List<Transform> buildingsList, int buildingIndex, bool changeLength = true, bool delete = false){

        // Check if building index number is greater than length of blocks list
        if (buildingIndex > buildingsList.Count) {
            Debug.LogError("Building Index cannot be greater than length of blocks list");
            return;
        }

        // Check if 'changeLength' parameter and 'delete' are both true
        if (changeLength && delete) {
            Debug.LogError("'changeLength' and 'delete' cannot both be true");
            return;
        }

        // Iterate through blocks
        for (int i = 0; i < buildingsList.Count; i++) {
            // If buildingIndex is the current building, either change its length or delete
            if (i == buildingIndex && changeLength == true) {
                int modifier = 3;
                float newYScale = (buildingsList[i].localScale.y * modifier);
                buildingsList[i].localScale = new Vector3(buildingsList[i].localScale.x, newYScale, buildingsList[i].localScale.z);
                // adjust position
                buildingsList[i].localPosition = new Vector3(buildingsList[i].localPosition.x, (buildingsList[i].localPosition.y + (modifier * 5) - 5), buildingsList[i].localPosition.z);

            }
            else if (i == buildingIndex && delete == true) {
                Destroy(buildingsList[i].gameObject);
                buildingsList.RemoveAt(i);
                Debug.Log("blocks remaining " + buildingsList.Count);
            }
        }
    }

}
