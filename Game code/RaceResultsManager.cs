using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RaceResultsManager : MonoBehaviour
{   
    public TMP_Text firstPlaceText;
    public TMP_Text secondPlaceText;
    public TMP_Text thirdPlaceText;
    public TMP_Text fourthPlaceText;
    public GameObject firstYellowPanel;
    public GameObject secondYellowPanel;
    public GameObject thirdYellowPanel;
    public GameObject fourthYellowPanel;
    // List that contains the racenumbers for the boss level validation
    public List<int> bossRaceNumbers = new List<int>();
    public int raceNumber;
    private string playerName;

    private bool yellowActivated = false;
    public bool playerFinishedFirst = false;

    private Dictionary<string, float> finishingTimes = new Dictionary<string, float>();

    private StarManager starManager; // Add a reference to the StarManager
    private SpriteManager spriteManager; // Add a reference to spriteManager

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        // Find the existing SpriteManager instance in the scene
        spriteManager = FindObjectOfType<SpriteManager>();

        if (spriteManager == null)
        {
            Debug.LogError("SpriteManager not found in the scene!");
        }

        playerName = starManager.playerName;

        // Call a function to initiliaze the bossRaceNumbers list
        InitializeBossRaceNumbers();
    }


    private void Start()
    {
        // Hide the yellow panels on the endgame canvas
        firstYellowPanel.SetActive(false);
        secondYellowPanel.SetActive(false);

        // Check for null and hide third and fourth panels if they exist
        if (thirdYellowPanel != null)
        {
            thirdYellowPanel.SetActive(false);
        }

        if (fourthYellowPanel != null)
        {
            fourthYellowPanel.SetActive(false);
        }
    }

    public void RecordPlayerFinishTime(float finishTime)
    {
        finishingTimes["Player"] = finishTime;
        UpdatePlaceTexts();  
    }

    public void RecordNPCFinishTime(NPC_Move npcMove, float finishTime)
    {
        finishingTimes[npcMove.NPC_Name] = finishTime;
        if (finishingTimes.ContainsKey("Player"))
        {
            UpdatePlaceTexts();
        }
    }

    private void UpdatePlaceTexts()
    {
        List<string> sortedCars = new List<string>(finishingTimes.Keys);
        sortedCars.Sort((car1, car2) => finishingTimes[car1].CompareTo(finishingTimes[car2]));

        // Update the place texts
        firstPlaceText.text = sortedCars.Count > 0 ? (sortedCars[0] == "Player" ? playerName : sortedCars[0]) : "---";

        // Check if there is a second place
        if (sortedCars.Count > 1)
        {
            secondPlaceText.text = sortedCars[1] == "Player" ? playerName : sortedCars[1];
        }
        else
        {
            secondPlaceText.text = "---";
        }

        // Check if there is a third place
        if (thirdPlaceText != null)
        {
            if (sortedCars.Count > 2)
            {
                thirdPlaceText.text = sortedCars[2] == "Player" ? playerName : sortedCars[2];
            }
            else
            {
                thirdPlaceText.text = "---";
            }
        }

        // Check if there is a fourth place
        if (fourthPlaceText != null)
        {
            if (sortedCars.Count > 3)
            {
                fourthPlaceText.text = sortedCars[3] == "Player" ? playerName : sortedCars[3];
            }
            else
            {
                fourthPlaceText.text = "---";
            }
        }

        // Make the yellow panel of the player position active
        if (!yellowActivated)
        {
            activateYellowPanel();
            yellowActivated = true;
        }
    }



    private void activateYellowPanel()
    {
        // make the yellow panel of the player position active
        if (firstPlaceText.text == playerName)
        {
            playerFinishedFirst = true;
            firstYellowPanel.SetActive(true);
            
            if (spriteManager.mapNumber == 1)
            {
                starManager.UpdateStarCount(raceNumber, 1);
            }
            else if (spriteManager.mapNumber == 2)
            {
                starManager.UpdateStarCountMin(raceNumber, 1);
            }
            else if (spriteManager.mapNumber == 3)
            {
                starManager.UpdateStarCountMul(raceNumber, 1);
            }
            
        }
        else if (secondPlaceText.text == playerName)
        {
            playerFinishedFirst = false;
            secondYellowPanel.SetActive(true);
            
            if (spriteManager.mapNumber == 1)
            {
                starManager.UpdateStarCount(raceNumber, 2);
            }
            else if (spriteManager.mapNumber == 2)
            {
                starManager.UpdateStarCountMin(raceNumber, 2);
            }
            else if (spriteManager.mapNumber == 3)
            {
                starManager.UpdateStarCountMul(raceNumber, 2);
            }
        }
        else if (thirdPlaceText.text == playerName)
        {
            playerFinishedFirst = false;
            thirdYellowPanel.SetActive(true);
            
            if (spriteManager.mapNumber == 1)
            {
                starManager.UpdateStarCount(raceNumber, 3);
            }
            else if (spriteManager.mapNumber == 2)
            {
                starManager.UpdateStarCountMin(raceNumber, 3);
            }
            else if (spriteManager.mapNumber == 3)
            {
                starManager.UpdateStarCountMul(raceNumber, 3);
            }
        }
        else if (fourthPlaceText.text == playerName)
        {
            playerFinishedFirst = false;
            fourthYellowPanel.SetActive(true);
            
            if (spriteManager.mapNumber == 1)
            {
                starManager.UpdateStarCount(raceNumber, 4);
            }
            else if (spriteManager.mapNumber == 2)
            {
                starManager.UpdateStarCountMin(raceNumber, 4);
            }
            else if (spriteManager.mapNumber == 3)
            {
                starManager.UpdateStarCountMul(raceNumber, 4);
            }
        }

        // print all the values in the starManager.highestStarCounts array
        /*for (int i = 0; i < starManager.highestStarCounts.Length; i++)
        {
            Debug.Log("Highest star count at race " + (i + 1) + ": " + starManager.highestStarCounts[i]);
        }*/
    }

    private void InitializeBossRaceNumbers()
    {
        // Loop through the highestStarCount list in starManager
        for (int raceNum = 0; raceNum < 9; raceNum++)
        {
            // Check if the highestStarCount is 3
            if (starManager.highestStarCounts[raceNum] == 3)
            {
                // Add the raceNum to the bossRaceNumbers list
                bossRaceNumbers.Add(raceNum + 1);
            }
        }
    }
}