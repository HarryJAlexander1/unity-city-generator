using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LsysMenu : MonoBehaviour
{
    public TMP_InputField axiomInputField;
    public TMP_InputField iterationsInputField;
    public TMP_InputField roadLengthInputField;
    public TMP_InputField nameInputField;
    public Vector3 spawnPosition;

    public GameObject gameManager;
    public bool CheckInputsFilled()
    {
        if (axiomInputField.text != "" && iterationsInputField.text != "" && roadLengthInputField.text != "")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckInputFieldIsValid(TMP_InputField inputField) {
        if (inputField == axiomInputField)
        {
            // check value only contains FA+-[]
            string pattern = @"^[FA\+\-\[\]]{1,5}$"; // regular expression pattern

            // check value length is <= 5 but greater than 0
            if (inputField.text.Length > 0 && inputField.text.Length <= 5 && System.Text.RegularExpressions.Regex.IsMatch(inputField.text, pattern))
            {
                return true; // input value is valid
            }
            else
            {
                Debug.LogError("Error: Axiom is not valid");
                return false; // input value is not valid
            }
        }
        else if (inputField == iterationsInputField)
        {

            // Try to parse the input value as an integer
            if (int.TryParse(inputField.text, out int value))
            {
                // Check if the integer is within the desired range
                if (value > 0 && value <= 5)
                {
                    return true; // input value is valid
                }
            }
            Debug.LogError("Error: Max iterations is not valid");
            return false; // input value is not valid
        }
        else if (inputField == roadLengthInputField)
        {

            // Try to parse the input value as an integer
            if (int.TryParse(inputField.text, out int value))
            {
                // Check if the integer is within the desired range
                if (value > 10 && value <= 100)
                {
                    return true; // input value is valid
                }
            }
            Debug.LogError("Error: Road length is not valid");
            return false; // input value is not valid
        }
        else if (inputField == nameInputField)
        {
            if (inputField.text.Length > 0 && inputField.text.Length <= 15)
            {
                return true; // input value is valid
            }
            else
            {
                Debug.LogError("Error: Name is not valid");
                return false; // input value is not valid
            }

        }
        else 
        { 
            Debug.LogError("Invalid inputField");
            return false;
        }
    }

    public void SendParameters() {
        var gameManagerScript = gameManager.GetComponent<GameManager>();
        gameManagerScript.CreateLSysCityGenerator(spawnPosition, axiomInputField.text, int.Parse(iterationsInputField.text), int.Parse(roadLengthInputField.text), nameInputField.text);
    }

    public void CloseLSysMenu()
    {
        gameObject.SetActive(false);
    }

    public void StartButtonPressed() {
        // Check all input fields are filled
        if (CheckInputsFilled())
        {
            // Check each input field text is valid
            if (CheckInputFieldIsValid(axiomInputField) && CheckInputFieldIsValid(iterationsInputField) && CheckInputFieldIsValid(roadLengthInputField) && CheckInputFieldIsValid(nameInputField))
            {
                // if they are, send data to Game Manager
                SendParameters();
                // close the menu
                CloseLSysMenu();
            }
            else
            {
                Debug.LogError("Input validation failed");
            }
        }
    }
}
