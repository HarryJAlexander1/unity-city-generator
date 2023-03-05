using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadGenLindenmayer : MonoBehaviour
{
    public int angle;
    public string axiom;
    public int maxIterations;
    public int length;

    public bool isExecuting = false;

    List<string> commandSequence = new();

    public GameObject segmentPrefab;
    public GameObject blockPrefab;

    private GameObject RoadNetwork;

    private void Awake()
    {
       string s = GenerateSentence();
       LoadCommands(s);
    }

    private void FixedUpdate()
    {
        StartExecution();
    }

    string GenerateSentence() {
        // set initial iterations to 1
        int iterations = 1;
        // set rules
        Dictionary<char, string> rules = new() {['F'] = "F+A++A-F--FF-A+", ['A'] = "-F+AA++F+A--F" };
        // set sentence to axiom
        string sentence = axiom;
        while (iterations <= maxIterations) {

            string nextResult = "";
            // iterate through each character in sentence
            foreach (char c in sentence)
            {
                // check if character is equal to each rule key
                if (rules.ContainsKey(c))
                {
                    nextResult += rules[c];
                }
                else 
                {
                    nextResult += c;
                }
            }
            // set sentence to the result from n iteration
            sentence = nextResult;
            iterations++;
        }
        //Debug.Log(sentence);
        return sentence; 
    }

    void LoadCommands(string sentence) {

        // Actions
        Dictionary<char, string> actions = new() { ['F'] = "forwards", ['-'] = "left", ['+'] = "right", ['['] = "save", [']'] = "load", ['A'] = "do nothing"};
        
        foreach (char c in sentence)
        {
            commandSequence.Add(actions[c]);
        }

        /*  foreach (var x in commandSequence)
          {
              Debug.Log(x.ToString());
          }*/

        //Debug.Log(commandSequence.Count);

    }

    // Coroutine to execute commands one by one
    IEnumerator ExecuteCommands()
    {
        List<Vector3> allSegmentPositions = new();
        List<Vector3> allBlockPositions = new();
        // save original state
        Vector3 position = transform.position;
        Quaternion rotation = transform.localRotation;

        RoadNetwork = new GameObject("Road Network");
        RoadNetwork.tag = "Road Network";

        isExecuting = true;
        foreach (string command in commandSequence)
        {
            
            switch (command)
            {
                case "forwards":
                    // assign current position
                    Vector3 prevPos = transform.position;
                    // generator moves forward
                    transform.Translate(Vector3.forward * length);
                    // assign new position
                    Vector3 newPos = transform.position;
                    // assign segment spawn position
                    Vector3 segmentSpawnPosition = ((newPos + prevPos) * 0.5f);
                    // if segment position is new...
                    if (!IsApproximatelyInList(segmentSpawnPosition,1f,allSegmentPositions)) {
                        // add segment position to the list
                        allSegmentPositions.Add(segmentSpawnPosition);
                        // spawn the segment at the position
                        GameObject segment = SpawnSegment(segmentSpawnPosition, transform.localRotation);


                        // assign city block spawn location for right side of segment
                        Vector3 blockSpawnPosition = segmentSpawnPosition + (segment.transform.right * (length * 0.5f));
                        if (!IsApproximatelyInList(blockSpawnPosition, 1f, allBlockPositions)) {
                            // spawn city block at location
                            GameObject b = SpawnBlock(blockSpawnPosition);
                            // assign cityblock rotation to equal segment rotation
                            b.transform.rotation = segment.transform.rotation;
                            // add block spawn position to list
                            allBlockPositions.Add(blockSpawnPosition);
                            
                        }
                        // assign city block spawn location for left side of segment
                        blockSpawnPosition = segment.transform.position + (-segment.transform.right * (length * 0.5f));
                        if (!IsApproximatelyInList(blockSpawnPosition, 1f, allBlockPositions)) {
                            // spawn city block at location
                            GameObject b = SpawnBlock(blockSpawnPosition);
                            // assign cityblock rotation to equal segment rotation
                            b.transform.rotation = segment.transform.rotation;
                            // add block spawn position to list
                            allBlockPositions.Add(blockSpawnPosition);
                        }
                    }
               
                    break;
                case "left":
                    transform.Rotate(Vector3.up, -angle);
        
                    break;
                case "right":
                    transform.Rotate(Vector3.up, angle);
             
                    break;
                case "save":
                    // save state
                    position = transform.position;
                    rotation = transform.localRotation;
                    break;
                case "load":
                    // load state
                    transform.position = position;
                    transform.localRotation = rotation;
                    break;
                default:
                    //Debug.Log("Unknown command: " + command);
                    break;
            }
            yield return new WaitForSeconds(0.25f);  // wait for 1 second before executing the next command
        }
        commandSequence.Clear();
        
        isExecuting = false;
    }

    // Start executing commands
    public void StartExecution()
    {
        if (!isExecuting && commandSequence.Count > 0)
        {
            StartCoroutine(ExecuteCommands());
        }
    }

   public GameObject SpawnSegment(Vector3 pos, Quaternion rot) {
        // spawn road segment at position
        GameObject segment = Instantiate(segmentPrefab, new(pos.x, pos.y, pos.z), Quaternion.identity);
        // adjust shape of segment
        segment.transform.localScale = new Vector3(segment.transform.localScale.x, segment.transform.localScale.y, length);
        segment.transform.localRotation = rot;
        segment.transform.parent = RoadNetwork.transform;
        return segment;
    }

    public GameObject SpawnBlock(Vector3 pos) {
        GameObject cityBlock = Instantiate(blockPrefab, pos, Quaternion.identity);
        cityBlock.transform.localScale = cityBlock.transform.localScale * ((length * 0.1f)* 0.8f);
        return cityBlock;
    }

    public bool IsApproximatelyInList(Vector3 targetPosition, float threshold, List<Vector3> vectorList)
    {
        foreach (Vector3 vector in vectorList)
        {
            if (Vector3.Distance(targetPosition, vector) < threshold)
            {
                return true;
            }
           // Debug.Log("Distance: " + Vector3.Distance(targetPosition, vector));
        }
        return false;
    }
}

