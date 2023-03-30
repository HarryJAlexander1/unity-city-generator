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

    private GameObject terrain;
  

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {
        
    }

    public void CreateVoronoiCityGenerator() {
        GameObject cityGenerator = Instantiate(voronoiPrefab, new(-999,-999,-999), Quaternion.identity);
    }

    public void CreateLSysCityGenerator(Vector3 pos, string axiom, int iterations, int roadLength, string name) {
        GameObject cityGenerator = Instantiate(lindenmayerPrefab, pos, Quaternion.identity);
        RoadGenLindenmayer cityGenLindenmayer = cityGenerator.GetComponent<RoadGenLindenmayer>();
        cityGenLindenmayer.length = roadLength;
        cityGenLindenmayer.cityName = name;
        string s = cityGenLindenmayer.GenerateSentence(iterations, axiom);
        cityGenLindenmayer.LoadCommands(s);
    }

    public void CreateTerrrain(Vector3 pos, GameObject terrainPrefab) {
        terrain = Instantiate(terrainPrefab, pos, Quaternion.identity);
        terrain.tag = "Terrain";
    }
  }
