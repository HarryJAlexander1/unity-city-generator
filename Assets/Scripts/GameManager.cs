using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // City generator prefabs
    public GameObject lindenmayerPrefab;
    public GameObject voronoiPrefab;
    public GameObject edgePrefab;
    public GameObject vertexPrefab;
    public GameObject meshGeneratorPrefab;
    // Terrain prefabs
    public GameObject planePrefab;

    // Canvas
    public GameObject canvas;

    public GameObject segmentPrefab;
    private GameObject terrain;
    public List<GameObject> edges; 


    public GameObject CreateRoadSegment() {
        GameObject seg = Instantiate(segmentPrefab, new(0,0,0), Quaternion.identity);
        return seg;
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

    public void CreateEdge(GameObject edgePrefab, Vector3 VertexAPos, Vector3 VertexBPos, Color colour) {
        GameObject edge = Instantiate(edgePrefab, new(0, 0, 0), Quaternion.identity);
        EdgeController edgeController = edge.GetComponent<EdgeController>();
        edgeController.position1 = VertexAPos;
        edgeController.position2 = VertexBPos;
        edgeController.edgeColor = colour;
        edges.Add(edge);
    }


    public GameObject CreateVertex(GameObject vertexPrefab, Vector3 pos) {
        GameObject v = Instantiate(vertexPrefab, pos, Quaternion.identity);
        return v;
    }

    public GameObject CreateMeshGenerator(Vector3 pos) {
        GameObject g = Instantiate(meshGeneratorPrefab, pos, Quaternion.identity);
        return g;
    }

    public void DestroyEdges() {
        if (edges.Count != 0) {
            for  (int i=0; i < edges.Count; i++)
            {
                Destroy(edges[i]);
            }
            edges.Clear();
        } 
    }
  }
