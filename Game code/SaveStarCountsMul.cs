using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveStarCountsMul : MonoBehaviour
{
    private StarManager starManager; // Reference to the StarManager
    private MulEquations mulEquations; // Reference to the PlusEquations
    private MulScript mulScript; // Add a reference to the PlusScript
    private int playerID;
    private bool BossMul;

    private void Awake()
    {
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        mulEquations = FindObjectOfType<MulEquations>();

        if (mulEquations == null)
        {
            Debug.LogError("MulEquations not found in the scene!");
        }

        mulScript = FindObjectOfType<MulScript>();

        if (mulScript == null)
        {
            Debug.LogError("MulScript not found in the scene!");
        }
    }

    private void Start()
    {
        SaveStarCountsToDatabase();
    }

    public void SaveStarCountsToDatabase()
    {
        // Get the star counts from starManager.highestStarCounts
        int[] starCounts = starManager.highestStarCountsMul;

        Debug.Log("Multiplication star counts: " + string.Join(",", starCounts));

        // calculate the toatl stars
        int totalStars = starManager.GetTotalStars();

        // get the playerID from the starManager
        playerID = starManager.playerID;

        // get the Boss value from the starManager
        BossMul = starManager.BossMul;

        // Create a form to send data to the PHP script
        WWWForm form = new WWWForm();
        form.AddField("totalStars", totalStars.ToString()); // Convert totalStars to string
        form.AddField("playerID", playerID.ToString()); // Convert playerID to string
        form.AddField("starCounts", string.Join(",", starCounts)); // Convert the array to a comma-separated string
        form.AddField("Boss", BossMul.ToString()); // Convert Boss to string

        // Send a request to the PHP script
        StartCoroutine(SendDataToPHP(form));

        starManager.numRaces++;
        //Debug.Log("Number of races: " + starManager.numRaces);
        if (starManager.numRaces > 1)
        {
            // Save the equations to the database
            mulEquations.SaveEquations();
        }
    }

    IEnumerator SendDataToPHP(WWWForm form)
    {
        string url = starManager.GetURL("update_star_counts_mul.php");

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
