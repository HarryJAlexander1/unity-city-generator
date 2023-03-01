using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenLindenmayer : MonoBehaviour
{
    public int angle;
    public string axiom;
    public int maxIterations;
    public int length;

    private bool isExecuting = false;

    List<string> commandSequence = new();

    public GameObject segmentPrefab;

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
        Debug.Log(sentence);
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

        Debug.Log(commandSequence.Count);

    }

    // Coroutine to execute commands one by one
    IEnumerator ExecuteCommands()
    {
        // save original state
        Vector3 position = transform.position;
        Quaternion rotation = transform.localRotation;

        isExecuting = true;
        foreach (string command in commandSequence)
        {
            switch (command)
            {
                case "forwards":
                    Vector3 prevPos = transform.position;
                    transform.Translate(Vector3.forward * length);
                    Vector3 newPos = transform.position;
                    Vector3 segmentPos = (newPos + prevPos) * 0.5f;
                    SpawnSegment(segmentPos, transform.localRotation);
                    break;
                case "backwards":
                    transform.Translate(-Vector3.forward * length);
                    SpawnSegment(transform.position, transform.localRotation);
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
            yield return new WaitForSeconds(0.1f);  // wait for 1 second before executing the next command
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

   public void SpawnSegment(Vector3 pos, Quaternion rot) {
        // spawn road segment at position
        GameObject segment = Instantiate(segmentPrefab, new(pos.x, pos.y, pos.z), Quaternion.identity);
        // adjust shape of segment
        segment.transform.localScale = new Vector3(segment.transform.localScale.x, segment.transform.localScale.y, length);
        segment.transform.localRotation = rot;
    }
}
