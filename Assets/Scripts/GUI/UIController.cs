using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject terrainMenu;
    public GameObject lsysMenu;
    public GameObject saveVoronoiCityMenu;
    public GameObject saveButton;
    public Camera mainCamera;
    public GameObject terrainObject;

    private GameObject voronoiGenerator; 
    public GameObject vertexPrefab;
    public List<GameObject> vertices = new();

    private bool terrainActive = false;
    public bool voronoiGeneratorExists = false;
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
        if (!voronoiGeneratorExists) {
            generator = 2;
            var gameManagerScript = gameManager.GetComponent<GameManager>();
            gameManagerScript.CreateVoronoiCityGenerator();
        }
        voronoiGeneratorExists = true;
    }

    public void OpenSaveMenu() {

        saveVoronoiCityMenu.SetActive(true);
        saveButton.SetActive(false);
        generator = 0;
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
                    if (generator == 2 && vertices.Count >= 4)
                    {
                        // send vertices to Voronoi generator by calling triangulate function
                        voronoiGenerator = GameObject.FindGameObjectWithTag("Voronoi");
                        RoadGenVoronoi voronoiScript = voronoiGenerator.GetComponent<RoadGenVoronoi>();
                        // call the pipeline in RoadGenVoronoi
                        voronoiScript.Pipeline(vertices);
                    }
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
            if (obj.CompareTag("City") || obj.CompareTag("CityGenerator") || obj.CompareTag("Edge"))
            {
                Destroy(obj);
                
            }
            if (obj.CompareTag("Vertex"))
            {
                vertices.Clear();
                Destroy(obj);
            }
            if (obj.CompareTag("Road"))
            {
                var gameManagerScript = gameManager.GetComponent<GameManager>();
                gameManagerScript.edges.Clear();
                Destroy(obj);
            }
            if (obj.CompareTag("Voronoi")) {
                Destroy(obj);
                voronoiGeneratorExists = false;
            }
        }
        // Close all menus
        if (saveButton.activeInHierarchy) {
            saveButton.SetActive(false);
        }
        if (lsysMenu.activeInHierarchy) {
            saveButton.SetActive(false);
        }
        if (saveVoronoiCityMenu.activeInHierarchy) {
            saveVoronoiCityMenu.SetActive(false);
        }
        generator = 0;
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
        if (Input.GetKey(KeyCode.Space) && generator == 2)
        {
            saveButton.SetActive(true);

            voronoiGenerator = GameObject.FindGameObjectWithTag("Voronoi");
            RoadGenVoronoi voronoiScript = voronoiGenerator.GetComponent<RoadGenVoronoi>();

            //voronoiScript.DestroyRoads(voronoiScript.roadSegments);
            voronoiScript.DestroyRoads(voronoiScript.roadSegments);
            voronoiScript.SpawnRoads(voronoiScript.voronoiEdgeList);
        }

    }
}
