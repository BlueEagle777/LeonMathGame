using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveStarCountsMin : MonoBehaviour
{
    private StarManager starManager; // Reference to the StarManager
    private MinusEquations minusEquations; // Reference to the PlusEquations
    private MinusScript minusScript; // Add a reference to the PlusScript
    private int playerID;
    private bool BossMin;

    private void Awake()
    {
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        minusEquations = FindObjectOfType<MinusEquations>();

        if (minusEquations == null)
        {
            Debug.LogError("MinusEquations not found in the scene!");
        }

        minusScript = FindObjectOfType<MinusScript>();

        if (minusScript == null)
        {
            Debug.LogError("MinusScript not found in the scene!");
        }
    }

    private void Start()
    {
        SaveStarCountsToDatabase();
    }

    public void SaveStarCountsToDatabase()
    {
        // Get the star counts from starManager.highestStarCounts
        int[] starCounts = starManager.highestStarCountsMin;

        // calculate the toatl stars
        int totalStars = starManager.GetTotalStars();

        // get the playerID from the starManager
        playerID = starManager.playerID;

        // get the Boss value from the starManager
        BossMin = starManager.BossMin;

        // Create a form to send data to the PHP script
        WWWForm form = new WWWForm();
        form.AddField("totalStars", totalStars.ToString()); // Convert totalStars to string
        form.AddField("playerID", playerID.ToString()); // Convert playerID to string
        form.AddField("starCounts", string.Join(",", starCounts)); // Convert the array to a comma-separated string
        form.AddField("Boss", BossMin.ToString()); // Convert Boss to string

        // Send a request to the PHP script
        StartCoroutine(SendDataToPHP(form));

        starManager.numRaces++;
        //Debug.Log("Number of races: " + starManager.numRaces);
        if (starManager.numRaces > 1)
        {
            // Save the equations to the database
            minusEquations.SaveEquations();
        }
    }

    IEnumerator SendDataToPHP(WWWForm form)
    {
        string url = starManager.GetURL("update_star_counts_min.php");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending data to server: " + www.error);
            }
            /*else
            {
                string response = www.downloadHandler.text;
                if (response == "Success")
                {
                    Debug.Log("Star counts saved successfully!");
                }
                else
                {
                    Debug.LogError("Error saving star counts: " + response);
                }
            }*/
        }
    }
}
