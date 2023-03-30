using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CityList : MonoBehaviour
{
    public TMP_Text cityListText;
    public string cityTag = "City";
    public int maxListSize = 5;
    //public ScrollRect scrollRect;

    void Update()
    {
        // Find all GameObjects with the "City" tag
        GameObject[] cityObjects = GameObject.FindGameObjectsWithTag(cityTag);

        // Create a list of city names
        List<string> cityNames = new List<string>();
        foreach (GameObject cityObject in cityObjects)
        {
            cityNames.Add(cityObject.name);
        }

        // Truncate the list if it exceeds the max size
        if (cityNames.Count > maxListSize)
        {
            cityNames.RemoveRange(maxListSize, cityNames.Count - maxListSize);
        }

        // Display the list of city names in the TextMeshPro object
        cityListText.text = string.Join("\n", cityNames.ToArray());

        /*// Resize the scroll rect content to fit the text
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)cityListText.transform);
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, ((RectTransform)cityListText.transform).rect.height);*/
    }
}