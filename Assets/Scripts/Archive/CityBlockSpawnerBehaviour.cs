using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockSpawnerBehaviour : MonoBehaviour
{
    public Transform roadNetwork;
    public GameObject cityBlockPrefab;
    public GameObject roadGenerator;
    // Start is called before the first frame update
    private void Awake()
    {
        SpawnCityBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MergeSegmentPanels()
    {

        // iterate through segments
        // add panels to list
        // merge adjacent panels together
        // add merged panels to new list
    }

    void SpawnCityBlocks() {
      
        // assign roadgenerator object
        roadGenerator = GameObject.FindGameObjectWithTag("Road Generator");
        RoadGenLindenmayer rgl = roadGenerator.GetComponent<RoadGenLindenmayer>();

        // assign roadnetwork object
        roadNetwork = GameObject.FindGameObjectWithTag("Road Network").transform;

        if (!rgl.isExecuting) {
            List<Vector3> cityBlockPositions = new();
            foreach (Transform child in roadNetwork)
            {
                /*// check space on either side of segment
                Vector3 space = new(child.transform.position.x - (((rgl.length / 2) + (child.localScale.x / 2))), child.transform.position.y, child.transform.position.z - 5);
                // spawn segment if there is space
                cityBlock = Instantiate(cityBlock, space, Quaternion.identity);
                cityBlock.transform.localScale = new(rgl.length * 0.10f, rgl.length * 0.10f, (rgl.length * 0.10f));*/

                // get the position and rotation of the original object
                Vector3 position = child.transform.position;
                Quaternion rotation = child.transform.rotation;

                // get the right vector of the original object to use for rotation
                Vector3 right = child.transform.right;

                // use Quaternion.LookRotation to get a rotation that points in the right direction
                Quaternion newRotation = Quaternion.LookRotation(right);

                Vector3 cityBlockposition = position + newRotation * (Vector3.forward * ((rgl.length / 2) + (child.localScale.x / 2)));
                if (!cityBlockPositions.Contains(cityBlockposition)) {
                 
                    GameObject cityBlock = Instantiate(cityBlockPrefab, cityBlockposition, newRotation);
                    cityBlock.transform.localScale = new(rgl.length * 0.10f, rgl.length * 0.10f, (rgl.length * 0.10f));
                    cityBlockPositions.Add(cityBlock.transform.position);
                }
            }
        }
    }
}
