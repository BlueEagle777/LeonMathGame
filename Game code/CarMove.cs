// Script that makes the car move at a constant speed
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class CarMove : MonoBehaviour
{
    public SumGenerator sumGenerator;

    public RaceResultsManager raceResultsManager;

    public Rigidbody2D carBody;
    public GameObject sumPannel;
    public GameObject fadingPannel;
    public GameObject countDownLabel;
    public Image progressImage;
    public Image progressImageOutline;
    public Image checkeredFlag;
    public Canvas EndRaceCanvas;
    public TextMeshProUGUI positionText;
    private TextMeshProUGUI countDownText;
    private bool countdownStarted = false;
    private bool carMoving = false;
    private float slowMotionTimer = 0f;
    private bool fadingPanelVisible = false;
    private readonly float fadingDuration = 0.5f;
    public float max_speed;

    public WheelJoint2D backWheel;
    public WheelJoint2D frontWheel;
    JointMotor2D jointMotor; 


    public float slowMotionFactor = 0.1f; // Set this to control slow motion (0.5 means 50% speed)

    private bool isBoosting = false;
    private bool boostMode = false;
    private bool solvingEquation = false;
    private float solvingTime; // Variable to store the time taken by the player to solve the equation
    private float boostDuration = 0f;
    private float boostTimer = 0f;
    private readonly float delayDuration = 3f;
    private float delayTimer = 0f;

    // Additional variable to control the z-axis rotation limit
    public readonly int minRotation = -30;
    public readonly int maxRotation = 30;
    private readonly float rotationCorrectionTorque = 3500f;

    // Variables to track distance and stop the car
    public float initialX;
    private readonly float brakeTargetX = 797f;

    // distanceTraveled is public so that it can be accessed from other scripts
    public float distanceTraveled;
    public float player_x_position;

    // variable to store the time
    private float time;
    // A list to store all the solving times
    private List<int> TimesMS = new List<int>();
    // A list to store the x-positions of the player
    private List<int> playerXPositions = new List<int>();
    private bool savedValues = false;

    private StarManager starManager; // Add a reference to the StarManager


    //------------------Boost variables------------------
    private bool firstBoostEntry = true;
    private float currentBoostTime;
    private float prevBoostTime;
    private float currentBoostPos;
    private float prevBoostPos;
    private int totalBoostEntries = 0;
    private float totalBoostSpeed = 0f;
    private float avgBoostSpeed;
    //---------------------------------------------------
    //------------------Drive variables------------------
    private bool firstDriveEntry = true;
    private float currentDriveTime;
    private float prevDriveTime;
    private float currentDrivePos;
    private float prevDrivePos;
    private int totalDriveEntries = 0;
    private float totalDriveSpeed = 0f;
    private float avgDriveSpeed;
    //---------------------------------------------------

    private void Awake()
    {
        initialX = transform.position.x;

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

        // Set the cars center of mass lower
        carBody.centerOfMass = new Vector2(-2.5f, -1.5f);

        // Hide the end game canvas
        if (EndRaceCanvas != null)
        {
            EndRaceCanvas.enabled = false;
        }

        // Make the sumPannel invisible
        if (sumPannel != null)
        {
            CanvasGroup sumCanvasGroup = sumPannel.GetComponent<CanvasGroup>();
            if (sumCanvasGroup == null)
            {
                sumCanvasGroup = sumPannel.AddComponent<CanvasGroup>();
            }
            sumCanvasGroup.alpha = 0f; // Set alpha to 0 to make it invisible
            sumCanvasGroup.blocksRaycasts = false; // Disable raycast to prevent interactions
        }

        // Make the fadingPannel fully transparent
        if (fadingPannel != null)
        {
            CanvasGroup fadingCanvasGroup = fadingPannel.GetComponent<CanvasGroup>();
            if (fadingCanvasGroup == null)
            {
                fadingCanvasGroup = fadingPannel.AddComponent<CanvasGroup>();
            }
            fadingCanvasGroup.alpha = 0f; // Set alpha to 0 to make it fully transparent
            fadingCanvasGroup.blocksRaycasts = false; // Disable raycast to prevent interactions
        }

        // Hide the progress image, progress image outline, checkered flag and position text
        if (progressImage != null)
        {
            progressImage.enabled = false;
        }
        if (progressImageOutline != null)
        {
            progressImageOutline.enabled = false;
        }
        if (checkeredFlag != null)
        {
            checkeredFlag.enabled = false;
        }
        if (positionText != null)
        {
            positionText.enabled = false;
        }

        // Get the TextMeshProUGUI component from the countDownLabel GameObject
        countDownText = countDownLabel.GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Add torque to back and front tire
    private void FixedUpdate()
    {   
        // If the car body is outside the rotation limits, then add torque to correct rotation
        float rotation = carBody.rotation;
        if (rotation < minRotation)
        {
            carBody.AddTorque(rotationCorrectionTorque * Time.fixedDeltaTime);
        }
        else if (rotation > maxRotation)
        {
            carBody.AddTorque(-rotationCorrectionTorque * Time.fixedDeltaTime);
        }

        
        // Get the player's x-position
        player_x_position = transform.position.x;
        // Print the distance the car has traveled (x-coordinate)
        distanceTraveled = transform.position.x - initialX;
        // Calculate the normalized progress value between 0 and 1
        float normalizedProgress = Mathf.Clamp01(distanceTraveled / brakeTargetX);

        // Update the fill amount of the progress image
        progressImage.fillAmount = normalizedProgress;

        if (distanceTraveled >= brakeTargetX)
        {
            BrakeAndStop();
        }
        
        // Start the countdown after 2 seconds of game launch
        if (Time.timeSinceLevelLoad >= 2f && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(CountdownCoroutine());
        }

        // Store the time and player's x-position every second
        if (carMoving)
        {
            time += Time.fixedDeltaTime;

            // Get the integer value when transitioning from one second to the next
            if ((int)time > (int)(time - Time.fixedDeltaTime))
            {
                TimesMS.Add((int)time*1000);
                // Store the player's x-position
                playerXPositions.Add((int)Math.Round(player_x_position));
            }

            // Print the speed of the car (use the time and transform.position.x variables)
            //Debug.Log("Speed: " + (transform.position.x - initialX) / time);

            // Determine whether the car is stuck and apply an antoclockwise torque to correct rotation
            if (carBody.velocity.magnitude < 0.2f)
            {
                Debug.Log("Car is stuck!");
                carBody.AddTorque(rotationCorrectionTorque * Time.fixedDeltaTime);
            }
        }

        // Move the car forward after the countdown says "GO!"
        if (carMoving && boostMode == false)
        {
            
            //------------------Average drive speed calculation------------------
            // Get the current time and distance
            currentDriveTime = Time.time;
            currentDrivePos = transform.position.x;
            if (firstDriveEntry == true)
            {
                prevDriveTime = currentDriveTime;
                prevDrivePos = currentDrivePos;
                totalDriveEntries++;
                firstDriveEntry = false;
            }
            else
            {
                // Calculate the time difference between the current and previous drive
                float elapsedDriveTime = currentDriveTime - prevDriveTime;
                prevDriveTime = currentDriveTime;
                // Calculate the distance covered during the drive
                float distanceCovered = currentDrivePos - prevDrivePos;
                prevDrivePos = currentDrivePos;
                // Calculate the speed during the drive
                float driveSpeed = distanceCovered / elapsedDriveTime;
                // Increment the total number of drive entries
                totalDriveEntries++;
                // Add the drive speed to the total drive speed
                totalDriveSpeed += driveSpeed;
                // Calculate the average drive speed
                avgDriveSpeed = totalDriveSpeed / totalDriveEntries;
            }
            //-------------------------------------------------------------------

            
            // show the progress image, progress image outline and checkered flag
            ShowProgressBar();

            jointMotor.motorSpeed = -max_speed;
            jointMotor.maxMotorTorque = 1000;
            backWheel.motor = jointMotor;
            frontWheel.motor = jointMotor;

            slowMotionTimer += Time.fixedDeltaTime;

            // After 3 seconds, transition to slow motion
            if (slowMotionTimer >= 3f)
            {
                Time.timeScale = slowMotionFactor;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                // Make the sum panel visible
                if (sumPannel != null)
                {
                    CanvasGroup sumCanvasGroup = sumPannel.GetComponent<CanvasGroup>();
                    if (sumCanvasGroup == null)
                    {
                        sumCanvasGroup = sumPannel.AddComponent<CanvasGroup>();
                    }
                    sumCanvasGroup.alpha = 1f; // Set alpha to 1 to make it visible
                    sumCanvasGroup.blocksRaycasts = true; // Enable raycast to allow interactions

                    // Start the solvingTime timer
                    solvingEquation = true;
                }

                // Gradually make the fading panel visible
                if (!fadingPanelVisible && fadingPannel != null && sumGenerator.wrongAns == false)
                {
                    fadingPanelVisible = true;
                    StartCoroutine(GraduallyShowFadingPanel());
                }

                if (solvingEquation)
                {
                    solvingTime += Time.fixedDeltaTime;
                }

                // Check if the correct button was pressed
                if (sumGenerator.correctAns && !boostMode && sumGenerator.wrongAns == false)
                {
                    // Stop the solvingTime timer
                    solvingEquation = false;

                    // Stop slow motion
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;

                    // Start boosting
                    boostMode = true;
                    isBoosting = true;
                    boostDuration = CalculateBoostDuration(solvingTime); // Calculate the boost duration based on 't'

                    boostTimer = 0f;
                    solvingTime = 0f;

                    // Make the sum panel invisible during boost
                    if (sumPannel != null)
                    {
                        CanvasGroup sumCanvasGroup = sumPannel.GetComponent<CanvasGroup>();
                        if (sumCanvasGroup != null)
                        {
                            sumCanvasGroup.alpha = 0f;
                            sumCanvasGroup.blocksRaycasts = false;
                        }
                    }

                    // Gradually make the fading panel invisible during boost
                    if (fadingPannel != null)
                    {
                        fadingPanelVisible = false;
                        StartCoroutine(GraduallyHideFadingPanel());
                    }
                }

                // Check if the wrong button was pressed
                if (sumGenerator.correctAns == false && sumGenerator.wrongAns == true)
                {
                    delayTimer += Time.fixedDeltaTime;

                    if (delayTimer >= delayDuration) 
                    { 
                        sumGenerator.UpdateAll(); 
                        delayTimer = 0f;
                        solvingTime = 0f;
                        sumGenerator.wrongAns = false;
                    }
                }

                if (sumGenerator.correctAns == true && sumGenerator.wrongAns == true)
                {
                    // Stop slow motion
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;

                    // Gradually make the fading panel invisible
                    if (fadingPannel != null)
                    {
                        fadingPanelVisible = false;
                        StartCoroutine(GraduallyHideFadingPanel());
                    }

                    sumGenerator.correctAns = false;
                }
            }
        }
        else
        {
            // Check if the car is boosting
            if (isBoosting && (distanceTraveled < brakeTargetX))
            {

                firstDriveEntry = true;

                //------------------Average boost speed calculation------------------
                // Get the current time and distance
                currentBoostTime = Time.time;
                currentBoostPos = transform.position.x;
                if (firstBoostEntry == true)
                {
                    prevBoostTime = currentBoostTime;
                    prevBoostPos = currentBoostPos;
                    totalBoostEntries++;
                    firstBoostEntry = false;
                }
                else
                {
                    // Calculate the time difference between the current and previous boost
                    float elapsedBoostTime = currentBoostTime - prevBoostTime;
                    prevBoostTime = currentBoostTime;
                    // Calculate the distance covered during the boost
                    float distanceCovered = currentBoostPos - prevBoostPos;
                    prevBoostPos = currentBoostPos;
                    // Calculate the speed during the boost
                    float boostSpeed = distanceCovered / elapsedBoostTime;
                    // Increment the total number of boost entries
                    totalBoostEntries++;
                    // Add the boost speed to the total boost speed
                    totalBoostSpeed += boostSpeed;
                    // Calculate the average boost speed
                    avgBoostSpeed = totalBoostSpeed / totalBoostEntries;
                }

                //-------------------------------------------------------------------
                
                boostTimer += Time.fixedDeltaTime;

                // Boost for the specified duration
                if (boostTimer < boostDuration)
                {
                    Boost();
                }
                else
                {
                    firstBoostEntry = true;
                    // Stop boosting
                    isBoosting = false;
                    boostMode = false;
                    sumGenerator.UpdateAll();

                    // Show the sum panel with alpha value of 1
                    if (sumPannel != null)
                    {
                        CanvasGroup sumCanvasGroup = sumPannel.GetComponent<CanvasGroup>();
                        if (sumCanvasGroup != null)
                        {
                            sumCanvasGroup.alpha = 1f;
                            sumCanvasGroup.blocksRaycasts = true;
                        }
                    }

                    // Gradually make the fading panel visible after boost
                    if (fadingPannel != null)
                    {
                        fadingPanelVisible = true;
                        StartCoroutine(GraduallyShowFadingPanel());
                    }
                }
            }
        }

        // Limit the rotation of the car body within the specified range
        //LimitRotation();

        
    }

    private void LimitRotation()
    {
        // Get the current rotation of the car
        float currentRotation = carBody.rotation;

        // Clamp the rotation within the specified range
        float clampedRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

        // Set the car's rotation to the clamped value
        carBody.SetRotation(clampedRotation);
    }

    // Function to calculate the boost duration based on the player's solving time 't'
    private float CalculateBoostDuration(float solvingTime)
    {
        // Apply the rules for the boost duration
        if (solvingTime < 1.5f)
        {
            boostDuration = 4f;
        }
        else if (solvingTime > 4f)
        {
            boostDuration = 1f;
        }
        else
        {
            // Calculate the boost duration using the equation: boostDuration = -1.2 * t + 5.8
            boostDuration = (-1.2f * solvingTime) + 5.8f;
        }

        return boostDuration;
    }

    // Function that gives the car a boost for 0.5 seconds
    public void Boost()
    {
        jointMotor.motorSpeed = -max_speed * 1.2f;
        jointMotor.maxMotorTorque = 1000;
        backWheel.motor = jointMotor;
        frontWheel.motor = jointMotor;
    }

    // Coroutine to gradually make the fading panel visible
    private IEnumerator GraduallyShowFadingPanel()
    {
        float elapsedTime = 0f;
        CanvasGroup fadingCanvasGroup = fadingPannel.GetComponent<CanvasGroup>();

        while (elapsedTime < fadingDuration)
        {
            float alpha = Mathf.Lerp(0f, 0.5f, elapsedTime / fadingDuration);
            fadingCanvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadingCanvasGroup.alpha = 0.5f; // Ensure it's fully visible at the end of the coroutine
    }

    // Coroutine to gradually make the fading panel invisible during boost
    private IEnumerator GraduallyHideFadingPanel()
    {
        float elapsedTime = 0f;
        CanvasGroup fadingCanvasGroup = fadingPannel.GetComponent<CanvasGroup>();

        while (elapsedTime < fadingDuration)
        {
            float alpha = Mathf.Lerp(0.5f, 0f, elapsedTime / fadingDuration);
            fadingCanvasGroup.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadingCanvasGroup.alpha = 0f; // Ensure it's fully invisible at the end of the coroutine
    }

    // Coroutine for the countdown
    private IEnumerator CountdownCoroutine()
    {
        // Display "3" for 1 second
        countDownText.text = "3";
        yield return new WaitForSeconds(1f);

        // Display "2" for 1 second
        countDownText.text = "2";
        yield return new WaitForSeconds(1f);

        // Display "1" for 1 second
        countDownText.text = "1";
        yield return new WaitForSeconds(1f);

        // Display "GO!" for 2 seconds
        carMoving = true;
        countDownText.text = "GO!";
        // Start the timer
        time = 0f;
        yield return new WaitForSeconds(2f);

        // Hide the countdown label
        countDownText.text = "";
    }

    public void BrakeAndStop()
    { 
        // Set the car's motor torque to zero to stop the car
        jointMotor.motorSpeed = 0f;
        backWheel.motor = jointMotor;
        frontWheel.motor = jointMotor;

        // Stop slow motion
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Make the sum panel invisible
        if (sumPannel != null)
        {
            CanvasGroup sumCanvasGroup = sumPannel.GetComponent<CanvasGroup>();
            if (sumCanvasGroup != null)
            {
                sumCanvasGroup.alpha = 0f;
                sumCanvasGroup.blocksRaycasts = false;
            }
        }

        // Gradually make the fading panel invisible
        if (fadingPannel != null)
        {
            fadingPanelVisible = false;
            StartCoroutine(GraduallyHideFadingPanel());
        }

        if (carMoving == true)
        {
            Debug.Log("Average boost speed: " + avgBoostSpeed);
            Debug.Log("Average drive speed: " + avgDriveSpeed);
        }

        // Stop executing the code in the FixedUpdate function
        carMoving = false;

        // Show the end game canvas
        if (EndRaceCanvas != null)
        {
            EndRaceCanvas.enabled = true;
        }

        if (raceResultsManager != null)
        {
            raceResultsManager.RecordPlayerFinishTime(Time.time);
        }

        // Hide the position text
        if (positionText != null)
        {
            positionText.enabled = false;
        }

        if (savedValues == false && !raceResultsManager.bossRaceNumbers.Contains(raceResultsManager.raceNumber) && raceResultsManager.playerFinishedFirst == true && !sumGenerator.isQuickRace && starManager.isAI == true)
        {
            // Start the coroutine to save the performance data
            StartCoroutine(SavePerformance());
            savedValues = true;
            // Add racenumber to list
            raceResultsManager.bossRaceNumbers.Add(raceResultsManager.raceNumber);
        }
    }

    private void ShowProgressBar()
    {
        if (progressImage != null)
        {
            progressImage.enabled = true;
        }
        if (progressImageOutline != null)
        {
            progressImageOutline.enabled = true;
        }
        if (checkeredFlag != null)
        {
            checkeredFlag.enabled = true;
        }
        if (positionText != null)
        {
            positionText.enabled = true;
        }
    }

    private IEnumerator SavePerformance()
    {
        // Get playerID from StarManager
        int playerID = starManager.playerID;
        
        string url = starManager.GetURL("SavePerformance.php"); // Replace with your PHP script URL

        // Create a form to send data to the PHP script
        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID.ToString());

        // Create arrays to store values for each field
        List<string> times = new List<string>();
        List<string> positions = new List<string>();

        foreach (int time in TimesMS)
        {
            times.Add(time.ToString());
        }

        foreach (int position in playerXPositions)
        {
            positions.Add(position.ToString());
        }

        // Add arrays to the form
        form.AddField("Time", string.Join(",", times.ToArray()));
        form.AddField("Position", string.Join(",", positions.ToArray()));

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text.Trim();
            if (responseText == "success")
            {
                Debug.Log("Performance data saved successfully.");
            }
            else
            {
                Debug.LogError("Unknown response: " + responseText);
            }
        }
        else
        {
            Debug.LogError("Error sending data to server: " + www.error);
        }

        // Dispose of the UnityWebRequest object to prevent memory leaks
        www.Dispose();
    }

}