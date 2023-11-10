using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class CarSelectionV2 : MonoBehaviour
{
    public Image CarImage;
    
    //public Image rightArrow;
    //public Image leftArrow;

    // Colour buttons
    public Image redButton;
    public Image blueButton;
    public Image greenButton;
    public Image orangeButton;
    public Image pinkButton;
    public Image purpleButton;
    public Image yellowButton;

    private int carNumber;
    private int colourNr;
    private string colour;

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
        carNumber = 1;
        colourNr = 1;
        redButton.rectTransform.sizeDelta = new Vector2(100, 100);
        colour = "Red";   
    }

    // Onclick event for right arrow
    public void OnClickRightArrow()
    {
        if (carNumber < 10)
        {
            carNumber++;
        }
        else
        {
            carNumber = 1;
        }
        LoadCarSprite();
    }

    // Onclick event for left arrow
    public void OnClickLeftArrow()
    {
        if (carNumber > 1)
        {
            carNumber--;
        }
        else
        {
            carNumber = 10;
        }
        LoadCarSprite();
    }

    // Onclick event for red button
    public void OnClickRedButton()
    {
        colourNr = 1;
        colour = "Red";
        ResetButtonSizes();
        redButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for blue button
    public void OnClickBlueButton()
    {
        colourNr = 2;
        colour = "Blue";
        ResetButtonSizes();
        blueButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for green button

    public void OnClickGreenButton()
    {
        colourNr = 3;
        colour = "Green";
        ResetButtonSizes();
        greenButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for orange button
    public void OnClickOrangeButton()
    {
        colourNr = 4;
        colour = "Orange";
        ResetButtonSizes();
        orangeButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for pink button
    public void OnClickPinkButton()
    {
        colourNr = 5;
        colour = "Pink";
        ResetButtonSizes();
        pinkButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for purple button
    public void OnClickPurpleButton()
    {
        colourNr = 6;
        colour = "Purple";
        ResetButtonSizes();
        purpleButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Onclick event for yellow button
    public void OnClickYellowButton()
    {
        colourNr = 7;
        colour = "Yellow";
        ResetButtonSizes();
        yellowButton.rectTransform.sizeDelta = new Vector2(100, 100);
        LoadCarSprite();
    }

    // Method to reset button sizes to 80 x 80
    private void ResetButtonSizes()
    {
        redButton.rectTransform.sizeDelta = new Vector2(80, 80);
        blueButton.rectTransform.sizeDelta = new Vector2(80, 80);
        greenButton.rectTransform.sizeDelta = new Vector2(80, 80);
        orangeButton.rectTransform.sizeDelta = new Vector2(80, 80);
        pinkButton.rectTransform.sizeDelta = new Vector2(80, 80);
        purpleButton.rectTransform.sizeDelta = new Vector2(80, 80);
        yellowButton.rectTransform.sizeDelta = new Vector2(80, 80);
    }

    // method to load the car sprite
    private void LoadCarSprite()
    { 
        // Load and set the appropriate car image sprite
        string carSpritePath = "Sprites/Car bodies/Car " + carNumber + "/Car Body " + carNumber + " - " + colour;
        Sprite carSprite = Resources.Load<Sprite>(carSpritePath);
        if (carSprite != null)
        {
            CarImage.sprite = carSprite;
        }
        else
        {
            Debug.LogError("Car sprite not found: " + carSpritePath);
        }
    }

    // Onclick event for continue button
    public void OnClickContinueButton()
    {
        // Set the car number and colour in the StarManager
        starManager.body = carNumber;
        starManager.colour = colourNr;

        // Load the next scene
        SceneManager.LoadScene("Wheel v2");
    }
}
