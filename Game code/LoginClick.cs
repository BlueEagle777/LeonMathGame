using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using SimpleJSON;

public class LoginClick : MonoBehaviour
{
    public AddToDropdown addToDropdown;
    public GameObject login_panel;
    public TMP_InputField passwordInput; // Input field for password
    public TMP_Text errorText; // Text to display error message

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

    [System.Obsolete]
    public void OnLoginClick()
    {
        int selectedIndex = addToDropdown.dropdown.value; // Get the selected index of the dropdown

        // Check if the selected index is valid
        if (selectedIndex >= 0 && selectedIndex < addToDropdown.namesList.Count)
        {
            // Get the selected player's name and surname
            string selectedName = addToDropdown.namesList[selectedIndex];
            string selectedSurname = addToDropdown.surnamesList[selectedIndex];
            string password = passwordInput.text;

            // Determine whether the index is an even number
            if (selectedIndex % 2 == 0)
            {
                Debug.Log("AI is activated in the game");
                starManager.isAI = true;
            }
            else
            {
                Debug.Log("AI is not activated in the game");
                starManager.isAI = false;
            }


            // Call PHP script to verify login
            StartCoroutine(VerifyLogin(selectedName, selectedSurname, password));
        }
        else
        {
            // Handle invalid selection
            errorText.text = "Please select a valid player";
        }
    }

    [System.Obsolete]
    IEnumerator VerifyLogin(string name, string surname, string password)
    {
        string url =  starManager.GetURL("verify_login.php");
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("surname", surname);
        form.AddField("password", password);

        using (WWW www = new WWW(url, form))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error connecting to server: " + www.error);
                yield break;
            }

            string response = www.text.Trim();
            if (response.StartsWith("Success:"))
            {
                // Split the response string based on ":"
                string[] responseParts = response.Split(':');

                if (responseParts.Length >= 3)
                {
                    // Extract the player ID and grade
                    string playerIDString = responseParts[1];
                    string gradeString = responseParts[2];

                    if (int.TryParse(playerIDString, out int playerID) && int.TryParse(gradeString, out int grade))
                    {
                        // Assign the extracted values
                        starManager.playerID = playerID;
                        starManager.playerGrade = grade;
                        starManager.playerName = name;
                    }
                }    

                // Successful login, load the main menu scene

                // Request star counts and pass the player ID
                StartCoroutine(RequestStarCounts(name, surname));
            }
            else
            {
                // Unsuccessful login
                errorText.text = "Incorrect password";
            }
        }
    }

    IEnumerator RequestStarCounts(string name, string surname)
    {
        string starCountsUrl =  starManager.GetURL("get_star_counts.php");
        WWWForm starCountsForm = new WWWForm();
        starCountsForm.AddField("name", name);
        starCountsForm.AddField("surname", surname);

        using (UnityWebRequest starCountsWebRequest = UnityWebRequest.Post(starCountsUrl, starCountsForm))
        {
            yield return starCountsWebRequest.SendWebRequest();

            if (starCountsWebRequest.result == UnityWebRequest.Result.Success)
            {
                if (starCountsWebRequest.downloadHandler.text.Trim() != "Player not found")
                {
                    // Parse the JSON response using SimpleJSON
                    JSONNode jsonNode = JSON.Parse(starCountsWebRequest.downloadHandler.text);

                    int raceOneCount = jsonNode["race_one"].AsInt;
                    int raceTwoCount = jsonNode["race_two"].AsInt;
                    int raceThreeCount = jsonNode["race_three"].AsInt;
                    int raceFourCount = jsonNode["race_four"].AsInt;
                    int raceFiveCount = jsonNode["race_five"].AsInt;
                    int raceSixCount = jsonNode["race_six"].AsInt;
                    int raceSevenCount = jsonNode["race_seven"].AsInt;
                    int raceEightCount = jsonNode["race_eight"].AsInt;
                    int raceNineCount = jsonNode["race_nine"].AsInt;

                    // Update the starManager.highestStarCounts array
                    starManager.highestStarCounts = new int[]
                    {
                        raceOneCount,
                        raceTwoCount,
                        raceThreeCount,
                        raceFourCount,
                        raceFiveCount,
                        raceSixCount,
                        raceSevenCount,
                        raceEightCount,
                        raceNineCount
                    };

                    // Start the coroutine to retrieve car information
                    //StartCoroutine(RetrieveCarInfo(starManager.playerID));


                    // print the values in the array
                    /*for (int i = 0; i < starManager.highestStarCounts.Length; i++)
                    {
                        Debug.Log("from database: " + starManager.highestStarCounts[i]);
                    }*/
                }
                else
                {
                    // Player not found
                    errorText.text = "Player not found";
                }
            }
        }

        StartCoroutine(RequestStarCountsMin(name, surname));
    }

    IEnumerator RequestStarCountsMin(string name, string surname)
    {
        string starCountsUrl = starManager.GetURL("get_star_counts_min.php");
        WWWForm starCountsForm = new WWWForm();
        starCountsForm.AddField("name", name);
        starCountsForm.AddField("surname", surname);

        using (UnityWebRequest starCountsWebRequest = UnityWebRequest.Post(starCountsUrl, starCountsForm))
        {
            yield return starCountsWebRequest.SendWebRequest();

            if (starCountsWebRequest.result == UnityWebRequest.Result.Success)
            {
                if (starCountsWebRequest.downloadHandler.text.Trim() != "Player not found")
                {
                    // Parse the JSON response using SimpleJSON
                    JSONNode jsonNode = JSON.Parse(starCountsWebRequest.downloadHandler.text);

                    int raceOneCount = jsonNode["race_one"].AsInt;
                    int raceTwoCount = jsonNode["race_two"].AsInt;
                    int raceThreeCount = jsonNode["race_three"].AsInt;
                    int raceFourCount = jsonNode["race_four"].AsInt;
                    int raceFiveCount = jsonNode["race_five"].AsInt;
                    int raceSixCount = jsonNode["race_six"].AsInt;
                    int raceSevenCount = jsonNode["race_seven"].AsInt;
                    int raceEightCount = jsonNode["race_eight"].AsInt;
                    int raceNineCount = jsonNode["race_nine"].AsInt;

                    // Update the starManager.highestStarCounts array
                    starManager.highestStarCountsMin = new int[]
                    {
                        raceOneCount,
                        raceTwoCount,
                        raceThreeCount,
                        raceFourCount,
                        raceFiveCount,
                        raceSixCount,
                        raceSevenCount,
                        raceEightCount,
                        raceNineCount
                    };
                    // print the values in the array
                    /*for (int i = 0; i < starManager.highestStarCounts.Length; i++)
                    {
                        Debug.Log("from database: " + starManager.highestStarCounts[i]);
                    }*/
                }
                else
                {
                    // Player not found
                    errorText.text = "Player not found";
                }
            }
        }

        StartCoroutine(RequestStarCountsMul(name, surname));
    }

    IEnumerator RequestStarCountsMul(string name, string surname)
    {
        string starCountsUrl = starManager.GetURL("get_star_counts_mul.php");
        WWWForm starCountsForm = new WWWForm();
        starCountsForm.AddField("name", name);
        starCountsForm.AddField("surname", surname);

        using (UnityWebRequest starCountsWebRequest = UnityWebRequest.Post(starCountsUrl, starCountsForm))
        {
            yield return starCountsWebRequest.SendWebRequest();

            if (starCountsWebRequest.result == UnityWebRequest.Result.Success)
            {
                if (starCountsWebRequest.downloadHandler.text.Trim() != "Player not found")
                {
                    // Parse the JSON response using SimpleJSON
                    JSONNode jsonNode = JSON.Parse(starCountsWebRequest.downloadHandler.text);

                    int raceOneCount = jsonNode["race_one"].AsInt;
                    int raceTwoCount = jsonNode["race_two"].AsInt;
                    int raceThreeCount = jsonNode["race_three"].AsInt;
                    int raceFourCount = jsonNode["race_four"].AsInt;
                    int raceFiveCount = jsonNode["race_five"].AsInt;
                    int raceSixCount = jsonNode["race_six"].AsInt;
                    int raceSevenCount = jsonNode["race_seven"].AsInt;
                    int raceEightCount = jsonNode["race_eight"].AsInt;
                    int raceNineCount = jsonNode["race_nine"].AsInt;

                    // Update the starManager.highestStarCounts array
                    starManager.highestStarCountsMul = new int[]
                    {
                        raceOneCount,
                        raceTwoCount,
                        raceThreeCount,
                        raceFourCount,
                        raceFiveCount,
                        raceSixCount,
                        raceSevenCount,
                        raceEightCount,
                        raceNineCount
                    };
                    // print the values in the array
                    /*for (int i = 0; i < starManager.highestStarCounts.Length; i++)
                    {
                        Debug.Log("from database: " + starManager.highestStarCounts[i]);
                    }*/
                }
                else
                {
                    // Player not found
                    errorText.text = "Player not found";
                }
            }
        }

        // Start the coroutine to retrieve car information
        StartCoroutine(RetrieveCarInfo(starManager.playerID));
    }



    // Coroutine to retrieve car information from the database
    public IEnumerator RetrieveCarInfo(int playerId)
    {
        // Create a URL with parameters
        string url = starManager.GetURL("retrieve_car_info.php");

        WWWForm CarInfoForm = new WWWForm();
        CarInfoForm.AddField("playerID", playerId);

        using (UnityWebRequest starCountsWebRequest = UnityWebRequest.Post(url, CarInfoForm))
        {
            yield return starCountsWebRequest.SendWebRequest();

            if (starCountsWebRequest.result == UnityWebRequest.Result.Success)
            {
                if (starCountsWebRequest.downloadHandler.text.Trim() != "Player not found")
                {
                    // Parse the JSON response using SimpleJSON
                    JSONNode jsonNode = JSON.Parse(starCountsWebRequest.downloadHandler.text);

                    starManager.body = jsonNode["body"].AsInt;
                    starManager.colour = jsonNode["colour"].AsInt;
                    starManager.wheel = jsonNode["wheel"].AsInt;
                    starManager.character = jsonNode["character_type"].AsInt;

                    if (starManager.body != 0)
                    {
                        // Load the main menu scene
                        SceneManager.LoadScene("Main menu");
                    }
                    else
                    {
                        // Load the car selection scene
                        SceneManager.LoadScene("Car v2");
                    }
                }
                else
                {
                    // Player not found
                    errorText.text = "Player not found";
                }
            }
        }  
    } 
}

