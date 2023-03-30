using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldController : MonoBehaviour
{
    public TMP_InputField inputField;
    //TMP_Text text;
    public GameObject placeholderText;

    void Start()
    {

        // Set up a listener to detect changes in the text
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    void OnInputFieldValueChanged(string value)
    {
        // Hide the placeholder text when the user starts typing
        if (!string.IsNullOrEmpty(value))
        {
            placeholderText.SetActive(false);
        }
        else
        {
            placeholderText.SetActive(true);
        }
    }
}
