using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class ProfileLoader : MonoBehaviour
{
    public TMP_Text welcomeText; // Reference to a TextMeshPro text for the welcome message
    public TMP_Text starsText; // Reference to a TextMeshPro text for total stars earned
    public TMP_Text xp_level;
    public Image progressbar;

    private StarManager starManager; // Add a reference to the StarManager

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial values for the texts
        if (welcomeText != null)
        {
            welcomeText.text = "Welcome, " + starManager.playerName + "!";
        }

        if (starsText != null)
        {
            starsText.text = "Total Stars: " + starManager.GetTotalStars().ToString();
        }

        // Load the xp level and progress bar
        LoadXP();
    }

    // Update is called once per frame
    void Update()
    {
        // You can update any dynamic text values here if needed
    }

    // This function can be called by a button's onClick event to change the car
    public void ChangeCar()
    {
        // Load the ChooseCharacter scene
        SceneManager.LoadScene("Car v2");
    }

    public void OnClickBack()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main menu");
    }

    // Function to load the xp level and progress bar
    public void LoadXP()
    {
        // Create a form to send data to the PHP script
        WWWForm form = new WWWForm();
        form.AddField("playerID", starManager.playerID.ToString()); // Convert playerID to string

        // Send a request to the PHP script
        StartCoroutine(GetXPValueFromPHP(form));
    }

    // Function to get the xp value from the database
    IEnumerator GetXPValueFromPHP(WWWForm form)
    {
        // Send a request to the PHP script
        string url = starManager.GetURL("GetXPValue.php");
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            // Wait for the request to be sent
            yield return www.SendWebRequest();

            // Check if there is an error
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Cannot connect to database: " + www.error);
            }
            else
            {
                // Get the data from the PHP script
                string dataString = www.downloadHandler.text;

                // Convert the data to an integer
                int xpValue = int.Parse(dataString);

                // Set the xp level text with the following equation: text = sqrt(xpValue) and round to the lowest integer
                xp_level.text = Mathf.FloorToInt(Mathf.Sqrt(xpValue)).ToString();
                
                // get the fill value for the progres bar
                float fillValue = Mathf.Sqrt(xpValue) - Mathf.FloorToInt(Mathf.Sqrt(xpValue));

                // Set the fill amount of the progress bar
                progressbar.fillAmount = fillValue;


            }
        }
    }
}

