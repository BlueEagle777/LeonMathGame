using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;

public class AddToDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    // Public lists to store names and surnames
    public List<string> namesList = new List<string>();
    public List<string> surnamesList = new List<string>();

    private StarManager starManager;

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }
    }

    private void Start()
    {
        StartCoroutine(FetchPlayerNames());
    }

    private IEnumerator FetchPlayerNames()
    {
        string url = starManager.GetURL("get_player_names.php");
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;

            JSONNode jsonNode = JSON.Parse(response);
            JSONNode playersArray = jsonNode["players"];

            List<string> combinedNames = new List<string>();

            foreach (JSONNode playerNode in playersArray)
            {
                string name = playerNode["name"];
                string surname = playerNode["surname"];

                // Store names and surnames in lists
                namesList.Add(name);
                surnamesList.Add(surname);

                combinedNames.Add(name + " " + surname);
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(combinedNames);
        }
        else
        {
            Debug.LogError("Failed to fetch player names: " + request.error);
        }
    }
}