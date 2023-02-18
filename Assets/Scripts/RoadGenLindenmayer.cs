using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenLindenmayer : MonoBehaviour
{
    public int angle;
    public string axiom;
    public int maxIterations;
    public int length;

    private void Awake()
    {
        GenerateSentence();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Debug.Log(sentence);
        return sentence;
    }

    void GenerateRoads(List<string> sentence) {

        // Actions
        Dictionary<string, string> actions = new() { ["F"] = "Forwards", ["-"] = "Left", ["+"] = "Right", ["["] = "Save", ["]"] = "Load" };
    }
}
