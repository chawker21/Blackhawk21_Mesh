using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WebLink : MonoBehaviour
{
    public string csvFileName;
    public GameObject buttonPrefab;
    public Transform buttonParent;

    private List<string> urlList;
    private List<string> labelList;

    void Start()
    {
        urlList = new List<string>();
        labelList = new List<string>();

        // Read the CSV file
        StreamReader reader = new StreamReader(csvFileName);
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            urlList.Add(values[0]);
            labelList.Add(values[1]);
        }
        reader.Close();

        // Initialize the position of the first button
        Vector3 buttonPosition = buttonParent.position;

        // Instantiate a button for each URL
        for (int i = 0; i < urlList.Count; i++)
        {
            // Instantiate the button prefab at the current position
            GameObject button = Instantiate(buttonPrefab, buttonPosition, Quaternion.identity, buttonParent);

            // Set the label for the button
            TextMeshProUGUI buttonLabel = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = labelList[i];

            // Add a click event to the button
            Button buttonComponent = button.GetComponent<Button>();
            int index = i; // need to capture the value of i for the lambda function
            buttonComponent.onClick.AddListener(() => Application.OpenURL(urlList[index]));

            // Update the position for the next button
            buttonPosition.y -= 30;
        }
    }
}