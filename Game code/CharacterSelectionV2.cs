using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;


public class CharacterSelectionV2 : MonoBehaviour
{
    public Image characterImage;

    private int characterNumber;

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
        characterNumber = 1;   
    }

    // Onclick event for right arrow
    public void OnClickRightArrow()
    {
        if (characterNumber < 18)
        {
            characterNumber++;
        }
        else
        {
            characterNumber = 1;
        }
        LoadCharacterSprite(ConvertCharacter(characterNumber).Item1, ConvertCharacter(characterNumber).Item2);
    }

    // Onclick event for left arrow
    public void OnClickLeftArrow()
    {
        if (characterNumber > 1)
        {
            characterNumber--;
        }
        else
        {
            characterNumber = 18;
        }
        LoadCharacterSprite(ConvertCharacter(characterNumber).Item1, ConvertCharacter(characterNumber).Item2);
    }

    // Function to convert the character number to a gender and number
    private (int, int) ConvertCharacter(int num)
    {
        if (num <= 9)
        {
            return (1, num);
        }
        else
        {
            return (2, num - 9);
        }
    }

    // method to load the wheel sprite
    private void LoadCharacterSprite(int gender, int number)
    { 
        // Load and set the appropriate car image sprite
        string characterSpritePath = "Sprites/Characters/Character " + gender + " - Head/Character " + gender + " - Head - " + number;
        Sprite characterSprite = Resources.Load<Sprite>(characterSpritePath);
        if (characterSprite != null)
        {
            characterImage.sprite = characterSprite;
        }
        else
        {
            Debug.LogError("Car sprite not found: " + characterSpritePath);
        }
    }

    //Onclick event for back button
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("Wheel v2");
    }

    // Onclick event for continue button
    public void OnClickContinueButton()
    {
        // Set the car number and colour in the StarManager
        starManager.character = characterNumber;

        // Send the selectedCar and playerID to the server for database update
        StartCoroutine(SendCarSelectionToServer(starManager.playerID));
    }

    // Coroutine to send car selection to the server
    private IEnumerator SendCarSelectionToServer(int playerId)
    {
        // Create a form to send data to PHP script
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("body", starManager.body);
        form.AddField("colour", starManager.colour);
        form.AddField("wheel", starManager.wheel);
        form.AddField("character", starManager.character);

        string url = starManager.GetURL("update_car.php");

        // Send the POST request to the server
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Car selection successfully updated in the database
                Debug.Log("Car selection updated successfully.");
                // Load the main menu scene
                SceneManager.LoadScene("Main menu");
            }
            else
            {
                // Handle the error (e.g., network error or server error)
                Debug.LogError("Error updating car selection: " + www.error);
            }
        }
    }
}
