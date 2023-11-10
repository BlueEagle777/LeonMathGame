using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    private PlusEquations plusEquations; // Reference to the PlusEquations
    private MinusEquations minusEquations; // Reference to the MinusEquations
    private MulEquations mulEquations; // Reference to the MulEquations
    private SpriteManager spriteManager; // Add a reference to the SpriteManager
    private StarManager starManager; // Add a reference to the StarManager
    private void Awake()
    {
        plusEquations = FindObjectOfType<PlusEquations>();

        if (plusEquations == null)
        {
            Debug.LogError("PlusEquations not found in the scene!");
        }

        minusEquations = FindObjectOfType<MinusEquations>();

        if (minusEquations == null)
        {
            Debug.LogError("MinusEquations not found in the scene!");
        }

        mulEquations = FindObjectOfType<MulEquations>();

        if (mulEquations == null)
        {
            Debug.LogError("MulEquations not found in the scene!");
        }

        spriteManager = FindObjectOfType<SpriteManager>();

        if (spriteManager == null)
        {
            Debug.LogError("SpriteManager not found in the scene!");
        }

        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }
    }
    public void ReloadCurrentScene()
    {
        // Get the current active scene's name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Load the current scene again
        SceneManager.LoadScene(currentSceneName);
        
        // Save the equations to the database
        if (spriteManager.sumType == "plus")
        {
            plusEquations.SaveEquations();
        }
        else if (spriteManager.sumType == "min")
        {
            minusEquations.SaveEquations();
        }
        else if (spriteManager.sumType == "mul")
        {
            mulEquations.SaveEquations();
        }

        // Check if it is the boss race and the player has not beaten the boss yet
        if (currentSceneName == "Boss level plus" && starManager.Boss == false)
        {
            // Update the number of attempts the player has used to beat the boss
            plusEquations.UpdateBossPlayed();
        }
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main menu");
        
        // Save the equations to the database
        if (spriteManager.sumType == "plus")
        {
            plusEquations.SaveEquations();
        }
        else if (spriteManager.sumType == "min")
        {
            minusEquations.SaveEquations();
        }
        else if (spriteManager.sumType == "mul")
        {
            mulEquations.SaveEquations();
        }
    }

    public void LoadMaps()
    {
        // Load the maps scene
        SceneManager.LoadScene("Maps menu");
    }

    public void LoadQuickRace()
    {
        SceneManager.LoadScene("Quick race maps");
    }

    public void LoadProfile()
    {
        SceneManager.LoadScene("Profile");
    }

    public void LoadPlusMenu()
    {
        SceneManager.LoadScene("Plus menu");
    }
}

