using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class QuickRace : MonoBehaviour
{
    public List<SpriteShapeRenderer> spriteShapeRenderers = new List<SpriteShapeRenderer>();

    private void Start()
    {
        // Deactivate all SpriteShapeRenderers
        DeactivateAllShapes();

        // Select a random index between 0 and max number of SpriteShapeRenderers
        int randomIndex = Random.Range(0, spriteShapeRenderers.Count);

        //print("Number of SpriteShapeRenderers: " + spriteShapeRenderers.Count);

        // Activate the selected SpriteShapeRenderer
        ActivateShape(spriteShapeRenderers[randomIndex]);

        // Find and activate the paired SpriteShapeRenderer
        ActivatePairedShape(spriteShapeRenderers[randomIndex]);
    }

    // Helper function to deactivate all SpriteShapeRenderers
    private void DeactivateAllShapes()
    {
        foreach (var spriteShapeRenderer in spriteShapeRenderers)
        {
            spriteShapeRenderer.gameObject.SetActive(false);
        }
    }

    // Helper function to activate a SpriteShapeRenderer
    private void ActivateShape(SpriteShapeRenderer shapeRenderer)
    {
        shapeRenderer.gameObject.SetActive(true);
    }

    // Helper function to activate the paired SpriteShapeRenderer
    private void ActivatePairedShape(SpriteShapeRenderer selectedRenderer)
    {
        string selectedName = selectedRenderer.gameObject.name;

        // Check for a name like 'Shape (X.Y)' and find its pair 'Shape (X.Z)'
        if (selectedName.Contains("(") && selectedName.Contains(")"))
        {
            int openParenIndex = selectedName.IndexOf("(");
            int closeParenIndex = selectedName.IndexOf(")");
            if (openParenIndex >= 0 && closeParenIndex >= 0)
            {
                string prefix = selectedName.Substring(0, openParenIndex);
                
                // Extract the numbers using the dot as a separator
                string numbers = selectedName.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
                
                // Split the numbers using the dot as a separator
                string[] numberParts = numbers.Split('.');
                
                if (numberParts.Length == 2)
                {
                    string firstNumber = numberParts[0];
                    string secondNumber = numberParts[1];

                    // Try to find the paired shape by switching the second number
                    string pairedName = prefix + "(" + firstNumber + "." + ToggleNumber(secondNumber) + ")";

                    SpriteShapeRenderer pairedRenderer = spriteShapeRenderers.Find(renderer => renderer.gameObject.name == pairedName);

                    if (pairedRenderer != null)
                    {
                        ActivateShape(pairedRenderer);
                    }
                }
            }
        }
    }


    // Helper function to toggle the number (e.g., 1 <-> 2)
    private string ToggleNumber(string number)
    {
        if (number == "1")
        {
            return "2";
        }
        else if (number == "2")
        {
            return "1";
        }
        else
        {
            return number; // Return the original number if it's not 1 or 2
        }
    }
}