using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickRaceMapsTransition : MonoBehaviour
{

    private SpriteManager spriteManager; // Add a reference to the SpriteManager

    private void Awake()
    {
        // Find the existing SpriteManager instance in the scene
        spriteManager = FindObjectOfType<SpriteManager>();

        if (spriteManager == null)
        {
            Debug.LogError("SpriteManager not found in the scene!");
        }
    }

    // Onclick function for the plus map
    public void OnPlusClick()
    {
        // Load the plus scene
        SpriteManager.Instance.InitializeMapNumber(1);
        spriteManager.sumType = "plus";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Quick race plus");
    }

    // Onclick function for the minus map
    public void OnMinusClick()
    {
        // Load the minus scene
        SpriteManager.Instance.InitializeMapNumber(2);
        spriteManager.sumType = "min";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Quick race min");
    }

    // Onclick function for the multiplication map
    public void OnMulClick()
    {
        // Load the minus scene
        SpriteManager.Instance.InitializeMapNumber(3);
        spriteManager.sumType = "mul";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Quick race mul");
    }

    public void OnBackClick()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
    }
    
}
