using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionManager : MonoBehaviour
{
    public CarMove playerCar;
    public NPC_Move npc1;
    public NPC_Move npc2;
    public NPC_Move npc3;

    public int playerPosition;
    public GameObject PositionLabel;
    private TextMeshProUGUI PositionText;

    // Start is called before the first frame update
    void Start()
    {
        // Get the TextMeshProUGUI component of the PositionLabel
        PositionText = PositionLabel.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = playerCar.distanceTraveled;
        float npc1Distance = npc1.distanceTraveled;
        float npc2Distance = npc2 != null ? npc2.distanceTraveled : float.MinValue;
        float npc3Distance = npc3 != null ? npc3.distanceTraveled : float.MinValue;

        // Store distances in an array and sort in descending order
        float[] distances = { playerDistance, npc1Distance, npc2Distance, npc3Distance };
        System.Array.Sort(distances);
        System.Array.Reverse(distances);

        // Determine player's position based on sorted distances
        playerPosition = System.Array.IndexOf(distances, playerDistance) + 1;

        // Update PositionText based on player's position
        UpdatePositionText(playerPosition);
    }

    // Function to update the PositionText based on player's position
    void UpdatePositionText(int position)
    {
        PositionText.text = position switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            4 => "4th",
            _ => "",
        };
    }
}
