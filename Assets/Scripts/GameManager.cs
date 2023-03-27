using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // City generator prefabs
    public GameObject lindenmayerPrefab;
    public GameObject voronoiPrefab;

    // Terrain prefabs
    public GameObject planePrefab;

    // Canvas
    public GameObject canvas;

    private GameObject cityGenerator;
    private GameObject terrain;
  

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {
        
    }

    public void CreateCityGenerator(Vector3 pos, GameObject cityGeneratorPrefab) {
       cityGenerator = Instantiate(cityGeneratorPrefab, pos, Quaternion.identity);
    }

    public void CreateTerrrain(Vector3 pos, GameObject terrainPrefab) {
        terrain = Instantiate(terrainPrefab, pos, Quaternion.identity);
        terrain.tag = "Terrain";
    }
       
  }
