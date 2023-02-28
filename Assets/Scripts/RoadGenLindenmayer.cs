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
        Dictionary<char, string> rules = new() {['F'] = "-F" , ['-'] = "+F" };
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
        Dictionary<char, string> actions = new() { ['F'] = "forwards", ['-'] = "left", ['+'] = "right", ['['] = "save", [']'] = "load" };
        
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
        isExecuting = true;
        foreach (string command in commandSequence)
        {
            switch (command)
            {
                case "forwards":
                    transform.Translate(Vector3.forward * length);
                    break;
                case "backwards":
                    transform.Translate(-Vector3.forward * length);
                    break;
                case "left":
                    transform.Rotate(Vector3.up, -angle);
                    break;
                case "right":
                    transform.Rotate(Vector3.up, angle);
                    break;
                default:
                    Debug.Log("Unknown command: " + command);
                    break;
            }
            yield return new WaitForSeconds(1.0f);  // wait for 1 second before executing the next command
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
}
