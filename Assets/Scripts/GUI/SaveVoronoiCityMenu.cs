using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveVoronoiCityMenu : MonoBehaviour
{
    public GameObject voronoiGenerator;
    public GameObject UI;
    public TMP_InputField nameInputField;

    public bool CheckNameInputField()
    {
        if (nameInputField.text != "" && nameInputField.text.Length <= 15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SaveCity() {
        // merge mesh of segments into one mesh
        voronoiGenerator = GameObject.FindGameObjectWithTag("Voronoi");
        RoadGenVoronoi voronoiScript = voronoiGenerator.GetComponent<RoadGenVoronoi>();
        voronoiScript.GroupIntoCity(nameInputField.text);

        // Clear all drawn edges and vertices and voronoi generator
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Edge"))
            {
                Destroy(obj);
            }
            if (obj.CompareTag("Vertex"))
            {
                UIController uIController = UI.GetComponent<UIController>();
                uIController.vertices.Clear();

                Destroy(obj);
            }
            if (obj.CompareTag("Voronoi"))
            {
                UIController uIController = UI.GetComponent<UIController>();
                uIController.voronoiGeneratorExists = false;
                Destroy(obj);
            }
            gameObject.SetActive(false);
        }
        
    }
    public void OnConfirmButtonClick() {
        if (CheckNameInputField())
        {
            SaveCity();
        }
        else { Debug.Log("Name invalid"); }
    }
}
