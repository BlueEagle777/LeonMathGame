using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapsTransition : MonoBehaviour
{
    public TMP_Text messageText;
    public Image minImage;
    public Image mulImage;
    
    private SpriteManager spriteManager; // Add a reference to the SpriteManager
    private StarManager starManager; // Add a reference to the StarManager

    private void Awake()
    {
        // Find the existing SpriteManager instance in the scene
        spriteManager = FindObjectOfType<SpriteManager>();

        if (spriteManager == null)
        {
            Debug.LogError("SpriteManager not found in the scene!");
        }

        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        starManager.GetBossValue(starManager.playerID);
        starManager.GetBossValueMin(starManager.playerID);
        starManager.GetBossValueMul(starManager.playerID);
    }

    private void Start()
    {
        messageText.text = "";
        
        if (starManager.Boss == true)
        {
            // disable the minus image
            minImage.enabled = false;
        }
        if (starManager.BossMin == true)
        {
            // disable the multiplication image
            mulImage.enabled = false;
        }
    }

    public void OnPlusClick()
    {
        // Load the plus scene
        spriteManager.InitializeMapNumber(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Plus menu");
    }

    public void OnMinusClick()
    {
        if (starManager.Boss == true)
        {
            // Load the minus scene
            spriteManager.InitializeMapNumber(2);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Minus menu");
        }
        else
        {
            messageText.text = "You must complete the plus map to unlock the minus map.";
        }
    }

    public void OnMultiplicationClick()
    {
        // Load the times scene
        spriteManager.InitializeMapNumber(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Mul menu");
    }

    // Onclick event for back button
    public void OnBackClick()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
    }
}
