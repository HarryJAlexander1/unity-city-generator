using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject terrainMenu;
    public GameObject lsysMenu;
    public Camera mainCamera;
    public GameObject terrainObject;

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
                }
                else if (generatorChoice == 2)
                {
                    
                }
                else { return; }
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
        }
        CheckSpawnCityGenerator(generator);
        
    }
}
