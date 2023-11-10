using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    // For quick race
    public string sumType;
    
    // Player variables
    public int mapNumber;
    public int raceNumber;
    public int playerCarType;
    public int playerColourType;
    public int playerWheelType;
    public int playerCharacterType;

    // NPC variables
    public int NPC1car;
    public int NPC1colour;
    public int NPC2car;
    public int NPC2colour;
    public int NPC3car;
    public int NPC3colour;
    public int NPC1character;
    public int NPC2character;
    public int NPC3character;
    public int NPC1wheel;
    public int NPC2wheel;
    public int NPC3wheel;

    // Avialable cars (list of integers)
    public List<int> avialableCars;
    // Avialable colours (list of integers)
    public List<int> avialableColours;
    // Avialable wheels (list of integers)
    public List<int> avialableWheels;
    // Avialable characters (list of integers)
    public List<int> avialableCharacters;

    private StarManager starManager; // Add a reference to the StarManager
    public bool carsLoaded;

    // Singleton instance
    private static SpriteManager instance;

    public static SpriteManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpriteManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("SpriteManager");
                    instance = managerObject.AddComponent<SpriteManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        // Initialize the avialable sprites list
        InitializeAvialableSprites();

        // Make carsLoaded false
        carsLoaded = false;
    }

    // Function to initialize the list of avialable sprites
    public void InitializeAvialableSprites()
    {
        avialableCars = new List<int>();
        for (int i = 1; i <= 10; i++)
        {
            avialableCars.Add(i);
        }

        avialableColours = new List<int>();
        for (int i = 1; i <= 7; i++)
        {
            avialableColours.Add(i);
        }

        avialableWheels = new List<int>();
        for (int i = 1; i <= 5; i++)
        {
            avialableWheels.Add(i);
        }

        avialableCharacters = new List<int>();
        for (int i = 1; i <= 18; i++)
        {
            avialableCharacters.Add(i);
        }

        // print the available cars and colours
        /*Debug.Log("Available cars count:" + avialableCars.Count);
        foreach (int car in avialableCars)
        {
            Debug.Log("Car: " + car);
        }

        Debug.Log("Available colours count:" + avialableColours.Count);
        foreach (int colour in avialableColours)
        {
            Debug.Log("Colour: " + colour);
        }*/
    }

    // Function to initialize the map number
    public void InitializeMapNumber(int mapNum)
    {
        mapNumber = mapNum;
    }

    // Function to initialize the race number
    public void InitializeRaceNumber(int raceNum)
    {
        raceNumber = raceNum;
    }

    // Function to get player sprite
    public (int, int, int, int) GetPlayerSprite()
    {
        playerCarType = starManager.body;
        playerColourType = starManager.colour;
        playerWheelType = starManager.wheel;
        playerCharacterType = starManager.character;

        // Remove the player sprite from the avialable sprites list
        avialableCars.Remove(playerCarType);
        avialableColours.Remove(playerColourType);
        avialableWheels.Remove(playerWheelType);
        avialableCharacters.Remove(playerCharacterType);

        // print the available cars and colours
        /*Debug.Log("Available cars count:" + avialableCars.Count);
        foreach (int car in avialableCars)
        {
            Debug.Log("Car: " + car);
        }

        Debug.Log("Available colours count:" + avialableColours.Count);
        foreach (int colour in avialableColours)
        {
            Debug.Log("Colour: " + colour);
        }*/

        return (playerCarType, playerColourType, playerWheelType, playerCharacterType);
    }
}
