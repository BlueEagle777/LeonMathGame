// Script that makes the car move at a constant speed
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarMove : MonoBehaviour
{
    public SumGenerator sumGenerator;
    public Rigidbody2D backTire;
    public Rigidbody2D frontTire;
    public Rigidbody2D carBody;
    public GameObject sumPannel;
    public GameObject fadingPannel;
    public GameObject countDownLabel;
    private TextMeshProUGUI countDownText;
    private bool countdownStarted = false;
    private bool carMoving = false;
    private bool slowMotionActivated = false;
    private float slowMotionTimer = 0f;
    private bool fadingPanelVisible = false;
    private float fadingDuration = 0.5f;
    public float max_speed;

    public WheelJoint2D backWheel;
    public WheelJoint2D frontWheel;
    JointMotor2D jointMotor; 

    public string carID;
    private int sumsGenerated;

    public float slowMotionFactor = 0.1f; // Set this to control slow motion (0.5 means 50% speed)

    private bool isBoosting = false;
    private bool boostMode = false;
    private float boostDuration = 2f;
    private float boostTimer = 0f;
    private float delayDuration = 3f;
    private float delayTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set variables
        sumsGenerated = 2;

        // Set the cars center of mass lower and show the center of mass with a pink sphere
        carBody.centerOfMass = new Vector2(-2.5f, -1.5f);
        GameObject centerOfMass = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerOfMass.transform.position = carBody.transform.TransformPoint(carBody.centerOfMass);
        centerOfMass.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        centerOfMass.GetComponent<Renderer>().material.color = Color.magenta;
        centerOfMass.transform.parent = carBody.transform;

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
        // Start the countdown after 2 seconds of game launch
        if (Time.timeSinceLevelLoad >= 2f && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(CountdownCoroutine());
        }

        // Move the car forward after the countdown says "GO!"
        if (carMoving && boostMode == false)
        {
            jointMotor.motorSpeed = -max_speed;
            jointMotor.maxMotorTorque = 1000;
            backWheel.motor = jointMotor;
            frontWheel.motor = jointMotor;

            slowMotionTimer += Time.fixedDeltaTime;

            // After 3 seconds, transition to slow motion
            if (slowMotionTimer >= 3f)
            {
                slowMotionActivated = true;
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
                }

                // Gradually make the fading panel visible
                if (!fadingPanelVisible && fadingPannel != null)
                {
                    fadingPanelVisible = true;
                    StartCoroutine(GraduallyShowFadingPanel());
                }

                // Check if the correct button was pressed
                if (sumGenerator.correctAns && !boostMode)
                {
                    // debug print
                    print("Correct button pressed!");

                    // Stop slow motion
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;

                    // Start boosting
                    boostMode = true;
                    isBoosting = true;
                    boostTimer = 0f;

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
            // ... (other code)

            // Check if the car is boosting
            if (isBoosting)
            {

                // debug print
                print("Boosting!");

                boostTimer += Time.fixedDeltaTime;

                // Boost for the specified duration
                if (boostTimer < boostDuration)
                {
                    Boost();
                }
                else
                {
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

        
    }

    // Function that gives the car a boost for 0.5 seconds
    public void Boost()
    {
        jointMotor.motorSpeed = -max_speed * 1.2f;
        jointMotor.maxMotorTorque = 1000;
        backWheel.motor = jointMotor;
        frontWheel.motor = jointMotor;
        Invoke("ResetSpeed", 0.2f);
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
        yield return new WaitForSeconds(2f);

        // Hide the countdown label
        countDownText.text = "";
    }
}