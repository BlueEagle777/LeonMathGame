using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineDetector : MonoBehaviour
{

    public CarMove carMoveScript; // Reference to the CarMove script
    private bool hasCrossedFinishLine = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasCrossedFinishLine)
        {
            hasCrossedFinishLine = true;
            carMoveScript.BrakeAndStop();
        }
    }
}
