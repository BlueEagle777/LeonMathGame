using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class NPC_Move : MonoBehaviour
{
    public bool AI;
    
    private float[,] QTable;
    public RaceResultsManager raceResultsManager;
    public CarMove carMove;
    
    public string playerID;
    public string NPC_Name;
    public Rigidbody2D carBody;
    public float max_speed;
    public WheelJoint2D backWheel;
    public WheelJoint2D frontWheel;
    JointMotor2D jointMotor;

    private bool carMoving = false;
    private bool boosting = false;
    private float boostDuration = 0f;
    private float boostTimer = 0f;
    private float timeTraveled = 0f;
    private bool solved = false;

    // Additional variable to control the z-axis rotation limit
    public float minRotation = -30f;
    public float maxRotation = 30f;
    private readonly float rotationCorrectionTorque = 3500f;

    // Variables to track distance and stop the car
    private float initialX;
    private float playerInitialx;
    private float gap;
    private readonly float brakeTargetX = 800f;
    private float relative_pos = 0f;
    private bool start_of_race = true;

    // Variable to track distance traveled
    public float distanceTraveled;

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
    }

    private void Start()
    {
        // Set the cars center of mass lower
        carBody.centerOfMass = new Vector2(-2.5f, -1.5f);

        // Calculate the ga between the NPC and the player
        playerInitialx = 3f;
        initialX = transform.position.x;
        gap = playerInitialx - initialX;

        // Set angular constraints to limit rotation
        //carBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        if (AI == true && starManager.isAI == true)
        {
            QTable = starManager.QTable;
        }
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad >= 5f && !carMoving)
        {
            carMoving = true;
            StartCoroutine(RaceCoroutine());
        }

        if (boosting)
        {
            Boost();
            boostTimer += Time.deltaTime;

            if (boostTimer >= boostDuration)
            {
                boosting = false;
                boostTimer = 0f;
                ResetSpeed();
            }
        }
    }

    private void FixedUpdate()
    {
        // If the car body is outside the rotation limits, then add torque to correct rotation
        if (carBody.rotation < (minRotation + 5))
        {
            carBody.AddTorque(rotationCorrectionTorque * Time.fixedDeltaTime);
        }
        else if (carBody.rotation > (maxRotation - 5))
        {
            carBody.AddTorque(-rotationCorrectionTorque * Time.fixedDeltaTime);
        }

        // Calculate the relative position of the NPC to the player
        relative_pos = carMove.player_x_position - transform.position.x;

        distanceTraveled = transform.position.x - initialX - gap;

        if ((distanceTraveled + gap) >= brakeTargetX)
        {
            BrakeAndStop();
        }

        if (carMoving)
        {
            timeTraveled += Time.fixedDeltaTime;
            // Calculate speed in points per second
            float speedInPointsPerSecond = distanceTraveled / timeTraveled;

            if (playerID == "NPC1")
            {
                // Print the speed
                //Debug.Log("Distance: " + distanceTraveled + "Time: " + timeTraveled + "Speed: " + speedInPointsPerSecond + " points/second");
            }
            
        }

        if (carMoving && !boosting)
        {
            jointMotor.motorSpeed = -max_speed;
            jointMotor.maxMotorTorque = 1000;
            backWheel.motor = jointMotor;
            frontWheel.motor = jointMotor;
        }
    }


    private void Boost()
    {
            jointMotor.motorSpeed = -max_speed * 1.2f;
            jointMotor.maxMotorTorque = 1000;
            backWheel.motor = jointMotor;
            frontWheel.motor = jointMotor;
    }

    private IEnumerator RaceCoroutine()
    {
        while (true)
        {
            if (AI == false || starManager.isAI == false)
            {
                float solving_duration = 0f;

                if (start_of_race)
                {
                    yield return new WaitForSeconds(3f);
                    start_of_race = false;
                }

                if (relative_pos > 0 && !solved)
                {
                    solving_duration = CalcSolvingTime(carMove.player_x_position, transform.position.x, playerID);
                    solved = true;
                }

                // This piece of code to determine the NPC bahavour when the NPC is in front of the player
                if (relative_pos < 0 && !solved)
                {
                    if (playerID == "NPC1" && spriteManager.raceNumber == 10)
                    {
                        // Generate a random value between 2 and 3 seconds for the boss level
                        solving_duration = Random.Range(2f, 3f);
                        solved = true;
                    }
                    else if (playerID == "NPC1" && spriteManager.raceNumber != 10)
                    {
                        // Generate a rabdom value between 2.5 and 4 seconds for a normal race
                        solving_duration = Random.Range(2.5f, 4f);
                    }
                }

                if (solved)
                {
                    yield return new WaitForSeconds(solving_duration);

                    boosting = true;
                    boostDuration = (-1.2f * solving_duration) + 5.8f;
                    yield return new WaitForSeconds(boostDuration);
                    solved = false; // Reset solved after boosting is done
                }

                // Add a small delay to prevent rapid toggling of solved
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                // Get the state of the agent
                int state = MapState();
                // Choose an action for the agent
                int action = EpsilonGreedyPolicy(state);
                // Take the action
                TakeAction(action);
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private float CalcSolvingTime(float PlayerPos, float NPCPos, string NPC_ID)
    {
        // Calculate the distance between the player and the NPC
        float distance = PlayerPos - NPCPos;

        // Add extra distance for NPC1 and less distance for NPC3
        if (NPC_ID == "NPC1")
        {
            // Generate a random value between 3 and 10
            float random_distance = Random.Range(3f, 10f);
            distance += random_distance;
        }
        else if (NPC_ID == "NPC3")
        {
            // Generate a random value between 3 and 10
            float random_distance = Random.Range(3f, 10f);
            distance -= random_distance;
        }

        // Calculate the relative boost speed of the NPC to the player
        float relative_boost_speed = 2f;

        // Calculate the boost time (the time needed for the NPC to catch up to the player)
        float boost_time = distance / relative_boost_speed;

        // Calculate the solving time
        float solving_time = (boost_time - 5.8f) / -1.2f;

        // Add a random variance between -0.5s and 0.5s
        float random_variance = Random.Range(-0.5f, 0.5f);
        solving_time += random_variance;

        // Cap the solving time if it is under 0.5s or above 4s
        if (solving_time < 0.5f)
        {
            solving_time = 0.5f;
        }
        else if (solving_time > 4f)
        {
            solving_time = 4f;
        }

        /*if (playerID == "NPC1")
        {
            Debug.Log("Distance: " + distance + " Relative Boost Speed: " + relative_boost_speed + " Boost Time: " + boost_time + " Solving Time: " + solving_time);
        }*/
        return solving_time;
    }

    private void ResetSpeed()
    {
        jointMotor.motorSpeed = -max_speed;
        jointMotor.maxMotorTorque = 1000;
        backWheel.motor = jointMotor;
        frontWheel.motor = jointMotor;
    }

    public void BrakeAndStop()
    {
        if (raceResultsManager != null)
        {
            raceResultsManager.RecordNPCFinishTime(this, Time.time);
        }

        // Set the car's motor torque to zero to stop the car
        jointMotor.motorSpeed = 0f;
        backWheel.motor = jointMotor;
        frontWheel.motor = jointMotor;

        // Stop executing the code in the FixedUpdate function
        carMoving = false;
    }

    //-----------------------------------------AI stuff-----------------------------------------//
    // Map (position_interval, rel_position) to a state in the range State1 - State30
    private int MapState()
    {
        int playerPosition = (int)carMove.player_x_position;
        int relativePosition = (int)transform.position.x - playerPosition;
        int relPosition;

        // Map the playerPosition and the relativePosition to State1 - State30
        int positionInterval = playerPosition / 100;
        // BEHIND
        if (relativePosition < -2)
        {
            relPosition = 0;
        }
        // ON
        else if (relativePosition > -2 && relativePosition < 10)
        {
            relPosition = 1;
        }
        // FRONT
        else
        {
            relPosition = 2;
        }

        // Calculate the mapped state
        int state = positionInterval * 3 + relPosition + 1;


        Debug.Log("State: " + state + " (positionInterval: " + positionInterval + ", relPosition: " + relPosition + ")");
        return state;
    }

    // Function to take an action
    private void TakeAction(int action)
    {
        if (action == 0)
        {
            ResetSpeed();
            Debug.Log("Action: Drives");
        }
        else if (action == 1)
        {
            boosting = true;
            boostDuration = 2f;
            Debug.Log("Action: Boosts");
        }
    }

    // Epsilon-greedy policy for action selection
    private int EpsilonGreedyPolicy(int state)
    {
        // Exploit: Choose the action with the highest Q-value
        float maxQ = QTable[state, 0];
        int bestAction = 0;
        int numActions = 2;

        for (int action = 1; action < numActions; action++)
        {
            if (QTable[state, action] > maxQ)
            {
                maxQ = QTable[state, action];
                bestAction = action;
            }
        }

        return bestAction;
    }
}
