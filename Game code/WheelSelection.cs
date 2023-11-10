using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class WheelSelection : MonoBehaviour
{
    public Image WheelImage;

    private int wheelNumber;

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
        wheelNumber = 1;   
    }

    // Onclick event for right arrow
    public void OnClickRightArrow()
    {
        if (wheelNumber < 4)
        {
            wheelNumber++;
        }
        else
        {
            wheelNumber = 1;
        }
        LoadWheelSprite();
    }

    // Onclick event for left arrow
    public void OnClickLeftArrow()
    {
        if (wheelNumber > 1)
        {
            wheelNumber--;
        }
        else
        {
            wheelNumber = 4;
        }
        LoadWheelSprite();
    }

    // method to load the wheel sprite
    private void LoadWheelSprite()
    { 
        // Load and set the appropriate car image sprite
        string wheelSpritePath = "Sprites/Wheels/Number Nitro Wheel " + wheelNumber;
        Sprite wheelSprite = Resources.Load<Sprite>(wheelSpritePath);
        if (wheelSprite != null)
        {
            WheelImage.sprite = wheelSprite;
        }
        else
        {
            Debug.LogError("Car sprite not found: " + wheelSpritePath);
        }
    }

    // Onclick event for continue button
    public void OnClickContinueButton()
    {
        // Set the car number and colour in the StarManager
        starManager.wheel = wheelNumber;

        // Load the next scene
        SceneManager.LoadScene("Character v2");
    }

    //Onclick event for back button
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("Car v2");
    }
}
