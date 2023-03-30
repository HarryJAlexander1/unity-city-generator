using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject terrainMenu;
    public GameObject lsysMenu;
    public Camera mainCamera;
    public GameObject terrainObject;

    private GameObject voronoiGenerator; 
    public GameObject vertexPrefab;
    List<GameObject> vertices = new();

    private bool terrainActive = false;
    private int generator;

 
    public void OpenTerrainMenu() {
        terrainMenu.SetActive(true);
        Debug.Log("Terrain menu opened");
    }

    public void OpenLSysMenu() {
        lsysMenu.SetActive(true);
    }

    public void SpawnPlane(){
        if (!terrainActive) {
            var gameManagerScript = gameManager.GetComponent<GameManager>();
            gameManagerScript.CreateTerrrain(new(0, 0, 0), gameManagerScript.planePrefab);
            terrainActive = true;
            terrainMenu.SetActive(false);
        } 
    }

    public void SelectLindenmayerGenerator() {
        generator = 1;
    }

    public void SelectVoronoiGenerator() {
        generator = 2;
        var gameManagerScript = gameManager.GetComponent<GameManager>();
        gameManagerScript.CreateVoronoiCityGenerator();
    }

    public void CheckSpawnCityGenerator(int generatorChoice) {

        if (Input.GetMouseButtonDown(0))
        {
            terrainObject = GameObject.FindGameObjectWithTag("Terrain");
  
            var gameManagerScript = gameManager.GetComponent<GameManager>();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == terrainObject)
            {
                Vector3 spawnPosition = hit.point;
                if (generatorChoice == 1)
                {
                    // Open L-sys menu
                    OpenLSysMenu();
                    lsysMenu.GetComponent<LsysMenu>().spawnPosition = spawnPosition;
                    generator = 0;
                }
                else if (generatorChoice == 2)
                {
                    GameObject v = Instantiate(vertexPrefab, new(spawnPosition.x, spawnPosition.y+2, spawnPosition.z), Quaternion.identity);
                    vertices.Add(v);
                }
                else { return; }
            }
        }
    }

    public void ClearAll()
    {
        // Get all active game objects in the scene
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // Loop through all game objects and destroy them if they have "City" or "RoadGenerator" tags
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("City") || obj.CompareTag("CityGenerator"))
            {
                Destroy(obj);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // The Escape key has been pressed, do something here
            Debug.Log("Escape key was pressed!");
            // Exit out of any open menu
            if (terrainMenu.activeInHierarchy) {
                terrainMenu.SetActive(false);
            }
            if (lsysMenu.activeInHierarchy) {
                lsysMenu.SetActive(false);
            }
        }
        CheckSpawnCityGenerator(generator);

        if (generator == 2 && vertices.Count > 2) {
            // send vertices to Voronoi generator by calling triangulate function
            voronoiGenerator = GameObject.FindGameObjectWithTag("Voronoi");
            RoadGenVoronoi voronoiScript = voronoiGenerator.GetComponent<RoadGenVoronoi>();
            // call the pipeline in RoadGenVoronoi
            voronoiScript.Pipeline(vertices);
            //voronoiScript.SpawnRoads(voronoiScript.voronoiEdgeList);
        }
        
    }
}
