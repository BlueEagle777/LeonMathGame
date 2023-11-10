using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SumGenerator : MonoBehaviour
{
    public string sumType;
    
    public TMP_Text sum;
    public TMP_Text button1;
    public TMP_Text button2;
    public TMP_Text button3;
    public TMP_Text button4;
    public Image button1Image;
    public Image button2Image;
    public Image button3Image;
    public Image button4Image;
    public Image pausePanel;
    public int ans;
    public int correctButton;
    private int levelNum;
    public bool correctAns;
    public bool wrongAns;
    public int numSumsGenerated; 
    public int levelNumber;
    public bool isQuickRace;
    private int maxLevel;

    // Add a refence to the scripts
    private PlusEquations plusEquations;
    private MinusEquations minusEquations;
    private MulEquations mulEquations;
    private StarManager starManager;
    private PositionManager positionManager;
    private DDA dda;

    private static int a;
    private static int b;

    private float solvingTime;

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        plusEquations = FindObjectOfType<PlusEquations>();

        if (plusEquations == null)
        {
            Debug.LogError("PlusEquations not found in the scene!");
        }

        // Find the existing StarManager instance in the scene
        minusEquations = FindObjectOfType<MinusEquations>();

        if (minusEquations == null)
        {
            Debug.LogError("MinusEquations not found in the scene!");
        }

        // Find the existing StarManager instance in the scene
        mulEquations = FindObjectOfType<MulEquations>();

        if (mulEquations == null)
        {
            Debug.LogError("MulEquations not found in the scene!");
        }

        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        // Find the existing DDA instance in the scene
        dda = FindObjectOfType<DDA>();

        if (dda == null)
        {
            Debug.LogError("DDA not found in the scene!");
        }

        // Find the existing PositionManager instance in the scene
        positionManager = FindObjectOfType<PositionManager>();

        if (positionManager == null)
        {
            Debug.LogError("PositionManager not found in the scene!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        correctAns = false;
        wrongAns = false;   
        numSumsGenerated = 0;
        UpdateAll();
        pausePanel.enabled = false; // Disable the parent panel
        maxLevel = 1;

        // Iterate through all child objects and disable them
        foreach (Transform child in pausePanel.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Function that updates the sum and the button texts
    public void UpdateAll()
    {
        // Generate an equation
        int first_num;
        int second_num;
        int temporary_ans;
        (first_num, second_num, correctButton, levelNum) = GenerateSum(this, levelNumber, starManager.playerGrade, isQuickRace, sumType);
        temporary_ans = CalcAns(sumType, first_num, second_num);

        // Determine wether DDA is enabled or not
        (bool DDA_enabled, int new_firstNum, int new_secondNum, int new_ans, int wrongAns1, int wrongAns2, int wrongAns3) = DetermineDDA(sumType, first_num, second_num, temporary_ans);
        //Debug.Log("Wrong answers in sumgenerator: " + wrongAns1 + " " + wrongAns2 + " " + wrongAns3);
        if (DDA_enabled == false)
        {
            ans = temporary_ans;
            DisplayButtonText(correctButton, ans, levelNum, numSumsGenerated, sumType);
        }
        else
        {
            ans = new_ans;
            DisplayButtonTextDDA(new_firstNum, new_secondNum, ans, wrongAns1, wrongAns2, wrongAns3, sumType);
        }
    }

    // FixedUpdate to increment the solving time
    void FixedUpdate()
    {
        solvingTime += Time.deltaTime;
    }

    public (int, int, int, int) GenerateSum(SumGenerator sumGenerator, int levelNum, int playerGrade, bool isQuickRace, string sum_type)
    {
        // Update vaiables
        sumGenerator.numSumsGenerated++;
        a = 0;
        b = 0;

        if (isQuickRace == true)
        {
            
            if (sumGenerator.numSumsGenerated >= 11 && playerGrade == 5)
            {
                levelNum = 9;
            }
            else if (sumGenerator.numSumsGenerated == 10 && playerGrade == 5)
            {
                levelNum = 8;
            }
            else if (sumGenerator.numSumsGenerated == 9 && playerGrade == 5)
            {
                levelNum = 7;
            }
            else if (sumGenerator.numSumsGenerated == 8 && playerGrade >= 4)
            {
                levelNum = 6;
            }
            else if (sumGenerator.numSumsGenerated == 7 && playerGrade >= 3)
            {
                levelNum = 5;
            }
            else if (sumGenerator.numSumsGenerated == 6 && playerGrade >= 3)
            {
                levelNum = 4;
            }
            else if (sumGenerator.numSumsGenerated == 5 && playerGrade >= 3)
            {
                levelNum = 3;
            }
            else if (sumGenerator.numSumsGenerated > 2 && sumGenerator.numSumsGenerated <= 4 && playerGrade >= 2)
            {
                levelNum = 2;
            }
            else if (sumGenerator.numSumsGenerated <= 2 && playerGrade >= 1)
            {
                levelNum = 1;
            }
            else
            {
                levelNum = maxLevel;
            }
            maxLevel = levelNum;
        }

        if (sum_type == "plus")
        {
            // Generate two random numbers between 1 and 9 if the levelNumber is 1, 2 or 3
            if (levelNum == 1)
            {
                // Singel digit without carry
                while (true)
                {
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 9);

                    if (a + b < 10)
                        break;
                }
            }
            else if (levelNum == 2)
            {
                // Singel digit with carry
                while (true)
                {
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 9);

                    if (a + b > 10)
                        break;
                }
            }
            else if (levelNum == 3)
            {
                // Double digit and singel digit without carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(1, 9);

                    // Get the second digit of a
                    int aSecondDigit = a % 10;

                    if (aSecondDigit + b <= 10)
                        break;
                }
            }
            else if (levelNum == 4)
            {
                // Double digit and singel digit with carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(1, 9);

                    // Get the second digit of a
                    int aSecondDigit = a % 10;

                    if (aSecondDigit + b > 10)
                        break;
                }
            }
            else if (levelNum == 5)
            {
                // Double digit and double digit without carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of a and b
                    int aSecondDigit = a % 10;
                    int bSecondDigit = b % 10;
                    int aFirstDigit = a / 10;
                    int bFirstDigit = b / 10;

                    if (aSecondDigit + bSecondDigit < 10 && aFirstDigit + bFirstDigit < 10)
                        break;
                }
            }
            else if (levelNum == 6)
            {
                // Double digit and double digit with carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of a and b
                    int aSecondDigit = a % 10;
                    int bSecondDigit = b % 10;
                    int aFirstDigit = a / 10;
                    int bFirstDigit = b / 10;

                    if (aSecondDigit + bSecondDigit > 10 || aFirstDigit + bFirstDigit > 10)
                        break;
                }
            }
            else if (levelNum == 7)
            {
                // Tripple digit and single/double digit without carry
                while (true)
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(1, 99);

                    // Get the first and second digit of b and third and second digit of a
                    int bSecondDigit = b % 10;
                    int bFirstDigit = b / 10;
                    int aThirdDigit = (a % 100) % 10;
                    int aSecondDigit = (a % 100) / 10;

                    if (aThirdDigit + bSecondDigit < 10 && aSecondDigit + bFirstDigit < 10)
                        break;
                }
            }
            else if (levelNum == 8)
            {
                // Tripple digit and double digit with carry
                while (true)
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of b and third and second digit of a
                    int bSecondDigit = b % 10;
                    int bFirstDigit = b / 10;
                    int aThirdDigit = (a % 100) % 10;
                    int aSecondDigit = (a % 100) / 10;

                    if (aThirdDigit + bSecondDigit >= 10 || aSecondDigit + bFirstDigit >= 10)
                        break;
                }
            }
            else if (levelNum == 9)
            {
                // Tripple digit and tripple digit with or without carry
                a = Random.Range(100, 999);
                b = Random.Range(100, 999);
            }
            else if (levelNum == 10)
            {
                if (sumGenerator.numSumsGenerated <= 4)
                {
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 9);
                }
                else if (sumGenerator.numSumsGenerated > 4 && sumGenerator.numSumsGenerated <= 8)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(10, 99);
                }
                else
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(100, 999);
                }
            }

            // Count the number of digits in the two numbers
            int aDigits = CountDigitsInInteger(a);
            int bDigits = CountDigitsInInteger(b);

            if (aDigits == bDigits)
            {
                sumGenerator.sum.text = "  " + a + "\n" + "+ " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sumGenerator.sum.text = "  " + a + "\n" + " + " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sumGenerator.sum.text = "  " + a + "\n" + "  + " + b;
            }
            

            // Choose a random button to display the correct answer
            int correctButton = Random.Range(1, 5);
            return (a, b, correctButton, levelNum);  // return the levelNum for quick race mode
        }
        else if (sum_type == "minus")
        {
            // Generate two random numbers between 1 and 9 if the levelNumber is 1, 2 or 3
            if (levelNum == 1)
            {
                // Singel digit without carry
                while (true)
                {
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 9);

                    if (b <= a)
                        break;
                }
            }
            else if (levelNum == 2)
            {
                // Singel digit with carry
                while (true)
                {
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 9);

                    if (b <= a)
                        break;
                }
            }
            else if (levelNum == 3)
            {
                // Double digit and singel digit without carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(1, 9);

                    // Get the second digit of a
                    int aSecondDigit = a % 10;

                    if (aSecondDigit >= b && b <= a)
                        break;
                }
            }
            else if (levelNum == 4)
            {
                // Double digit and singel digit with carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(1, 9);

                    // Get the second digit of a
                    int aSecondDigit = a % 10;

                    if (aSecondDigit < b && b <= a)
                        break;
                }
            }
            else if (levelNum == 5)
            {
                // Double digit and double digit without carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of a and b
                    int aSecondDigit = a % 10;
                    int bSecondDigit = b % 10;
                    int aFirstDigit = a / 10;
                    int bFirstDigit = b / 10;

                    if (aSecondDigit >= bSecondDigit && aFirstDigit >= bFirstDigit && b <= a)
                        break;
                }
            }
            else if (levelNum == 6)
            {
                // Double digit and double digit with carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of a and b
                    int aSecondDigit = a % 10;
                    int bSecondDigit = b % 10;
                    int aFirstDigit = a / 10;
                    int bFirstDigit = b / 10;

                    if ((aSecondDigit < bSecondDigit || aFirstDigit < bFirstDigit) && b <= a)
                        break;
                }
            }
            else if (levelNum == 7)
            {
                // Tripple digit and single/double digit without carry
                while (true)
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(1, 99);

                    // Get the first and second digit of b and third and second digit of a
                    int bSecondDigit = b % 10;
                    int bFirstDigit = b / 10;
                    int aThirdDigit = (a % 100) % 10;
                    int aSecondDigit = (a % 100) / 10;

                    if (aThirdDigit >= bSecondDigit && aSecondDigit >= bFirstDigit && b <= a)
                        break;
                }
            }
            else if (levelNum == 8)
            {
                // Tripple digit and double digit with carry
                while (true)
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(10, 99);

                    // Get the first and second digit of b and third and second digit of a
                    int bSecondDigit = b % 10;
                    int bFirstDigit = b / 10;
                    int aThirdDigit = (a % 100) % 10;
                    int aSecondDigit = (a % 100) / 10;

                    if ((aThirdDigit < bSecondDigit || aSecondDigit < bFirstDigit) && b <= a)
                        break;
                }
            }
            else if (levelNum == 9)
            {
                while (true)
                {
                    a = Random.Range(100, 999);
                    b = Random.Range(100, 999);

                    if (b <= a)
                        break;
                }     
            }
            else if (levelNum == 10)
            {
                if (sumGenerator.numSumsGenerated <= 4)
                {
                    // Singel digit without carry
                    while (true)
                    {
                        a = Random.Range(1, 9);
                        b = Random.Range(1, 9);

                        if (b <= a)
                            break;
                    }
                }
                else if (sumGenerator.numSumsGenerated > 4 && sumGenerator.numSumsGenerated <= 8)
                {
                    // Singel digit without carry
                    while (true)
                    {
                        a = Random.Range(10, 99);
                        b = Random.Range(10, 99);

                        if (b <= a)
                            break;
                    }
                }
                else
                {
                    // Singel digit without carry
                    while (true)
                    {
                        a = Random.Range(100, 999);
                        b = Random.Range(100, 999);

                        if (b <= a)
                            break;
                    }
                }
            }

            // Count the number of digits in the two numbers
            int aDigits = CountDigitsInInteger(a);
            int bDigits = CountDigitsInInteger(b);

            if (aDigits == bDigits)
            {
                sumGenerator.sum.text = "  " + a + "\n" + "- " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sumGenerator.sum.text = "  " + a + "\n" + " - " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sumGenerator.sum.text = "  " + a + "\n" + "  - " + b;
            }
            

            // Choose a random button to display the correct answer
            int correctButton = Random.Range(1, 5);
            return (a, b, correctButton, levelNum);  // return the levelNum for quick race mode
        }
        else
        {
            // Generate two random numbers between 1 and 9 if the levelNumber is 1, 2 or 3
            if (levelNum == 1)
            {
                // Singel digit without carry
                while (true)
                {
                    a = Random.Range(1, 7);
                    b = 2;

                    break;
                }
            }
            else if (levelNum == 2)
            {
                // Singel digit with carry
                while (true)
                {
                    a = Random.Range(1, 13);
                    b = 2;

                    break;
                }
            }
            else if (levelNum == 3)
            {
                // Double digit and singel digit without carry
                while (true)
                {
                   a = Random.Range(1, 7);
                   b = 3;

                    break;
                }
            }
            else if (levelNum == 4)
            {
                // Double digit and singel digit with carry
                while (true)
                {
                    a = Random.Range(1, 13);
                    b = 3;

                    break;
                }
            }
            else if (levelNum == 5)
            {
                // Double digit and double digit without carry
                while (true)
                {
                    a = Random.Range(1, 13);
                    b = a;

                    break;
                }
            }
            else if (levelNum == 6)
            {
                // Double digit and double digit with carry
                while (true)
                {
                    a = Random.Range(1, 13);
                    b = Random.Range(1, 13);

                    // Create an integer array with numbers 1, 2, 5, 9, 11
                    int[] numbers = { 1, 2, 5, 9, 10, 11 };

                    // Check if b is in the array and a != b
                    bool bInArray = false;
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if (numbers[i] == b)
                        {
                            bInArray = true;
                            break;
                        }
                    }

                    if (bInArray && a != b)
                    {
                        break;
                    }
                }
            }
            else if (levelNum == 7)
            {
                // Tripple digit and single/double digit without carry
                while (true)
                {
                    a = Random.Range(1, 13);
                    b = Random.Range(1, 13);

                    // Create an integer array with numbers 3, 4, 6, 7, 8, 12
                    int[] numbers = { 3, 4, 6, 7, 8, 12 };

                    // Check if b is in the array and a != b
                    bool bInArray = false;
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if (numbers[i] == b)
                        {
                            bInArray = true;
                            break;
                        }
                    }

                    if (bInArray && a != b)
                    {
                        break;
                    }
                }
            }
            else if (levelNum == 8)
            {
                // Tripple digit and double digit with carry
                while (true)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(2, 4);

                    break;
                }
            }
            else if (levelNum == 9)
            {
                while (true)
                {
                    a = Random.Range(10, 100);
                    b = Random.Range(3, 10);

                    break;
                }     
            }
            else if (levelNum == 10)
            {
                // Generate two random numbers between 1 and 9 if the levelNumber is 1, 2 or 3
                if (sumGenerator.numSumsGenerated <= 1)
                {
                    a = Random.Range(1, 13);
                    b = 2;
                }
                else if (sumGenerator.numSumsGenerated > 1 && sumGenerator.numSumsGenerated <= 2)
                {
                    a = Random.Range(1, 13);
                    b = 3;
                }
                else if (sumGenerator.numSumsGenerated > 2 && sumGenerator.numSumsGenerated <= 3)
                {
                    a = Random.Range(4, 13);
                    b = a;
                }
                else if (sumGenerator.numSumsGenerated > 3 && sumGenerator.numSumsGenerated <= 6)
                {
                    // Double digit and double digit with carry
                    while (true)
                    {
                        a = Random.Range(4, 13);
                        b = Random.Range(4, 13);

                        if (a != b)
                        {
                            break;
                        }
                    }
                }
                else if (sumGenerator.numSumsGenerated > 6 && sumGenerator.numSumsGenerated <= 8)
                {
                    a = Random.Range(10, 99);
                    b = Random.Range(2, 4);
                }
                else 
                {
                    a = Random.Range(10, 100);
                    b = Random.Range(3, 10);
                }
            } 

            // Count the number of digits in the two numbers
            int aDigits = CountDigitsInInteger(a);
            int bDigits = CountDigitsInInteger(b);

            if (aDigits == bDigits)
            {
                sumGenerator.sum.text = "  " + a + "\n" + "× " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sumGenerator.sum.text = "  " + a + "\n" + " × " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sumGenerator.sum.text = "  " + a + "\n" + "  × " + b;
            }
            

            // Choose a random button to display the correct answer
            int correctButton = Random.Range(1, 5);
            return (a, b, correctButton, levelNum);  // return the levelNum for quick race mode
        }

        
    }

    // Function to count the number of digits in an integer
    public static int CountDigitsInInteger(int number)
    {
        // Convert the integer to a string
        string numberString = number.ToString();
        
        // Use the Length property to count the number of digits
        int digitCount = numberString.Length;
        
        return digitCount;
    }

    public void DisplayButtonText(int correctButton, int answer, int levelNum, int numSumsGenerated, string sum_type)
    {
        // Enable all the buttons
        button1Image.enabled = true;
        button2Image.enabled = true;
        button3Image.enabled = true;
        button4Image.enabled = true;

        // Create a list of available numbers to be used for the buttons
        List<int> availableNumbers = new List<int>();

        if (sum_type == "plus")
        {
            if (levelNum == 1)
            {
                for (int i = 1; i <= 10; i++) // Assuming 10 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 2)
            {
                for (int i = 8; i <= 18; i++) // Assuming 18 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 3)
            {
                for (int i = 10; i <= 99; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 4)
            {
                for (int i = 21; i <= 108; i++) // Assuming 108 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 5)
            {
                for (int i = 10; i <= 99; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 6)
            {
                for (int i = 30; i <= 198; i++) // Assuming 198 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 7)
            {
                for (int i = 101; i <= 1098; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 8)
            {
                for (int i = 130; i <= 1098; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 9)
            {
                for (int i = 200; i <= 1998; i++) // Assuming 1998 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 10)
            {
                if (numSumsGenerated <= 4)
                {
                    for (int i = 1; i <= 19; i++) // Assuming 19 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
                else if (numSumsGenerated > 4 && numSumsGenerated <= 8)
                {
                    for (int i = 1; i <= 198; i++) // Assuming 198 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
                else
                {
                    for (int i = 1; i <= 1998; i++) // Assuming 1998 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
            }
        }
        else if (sum_type == "minus")
        {
            if (levelNum == 1)
            {
                for (int i = 0; i <= 10; i++) // Assuming 10 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 2)
            {
                for (int i = 0; i <= 10; i++) // Assuming 18 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 3)
            {
                for (int i = 10; i <= 99; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 4)
            {
                for (int i = 0; i <= 99; i++) // Assuming 108 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 5)
            {
                for (int i = 10; i <= 99; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 6)
            {
                for (int i = 0; i <= 89; i++) // Assuming 198 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 7)
            {
                for (int i = 100; i <= 999; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 8)
            {
                for (int i = 1; i <= 999; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 9)
            {
                for (int i = 0; i <= 899; i++) // Assuming 10 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 10)
            {
                if (numSumsGenerated <= 4)
                {
                    for (int i = 1; i <= 19; i++) // Assuming 19 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
                else if (numSumsGenerated > 4 && numSumsGenerated <= 8)
                {
                    for (int i = 0; i <= 99; i++) // Assuming 108 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i <= 899; i++) // Assuming 10 as the upper limit of available numbers
                    {
                        availableNumbers.Add(i);
                    }
                }
            }
        }
        else
        {
            if (levelNum == 1)
            {
                for (int i = 2; i <= 12; i++) // Assuming 10 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 2)
            {
                for (int i = 2; i <= 24; i++) // Assuming 18 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 3)
            {
                for (int i = 2; i <= 18; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 4)
            {
                for (int i = 2; i <= 36; i++) // Assuming 108 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 5)
            {
                for (int i = 1; i <= 144; i++) // Assuming 99 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 6)
            {
                for (int i = 2; i <= 132; i++) // Assuming 198 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 7)
            {
                for (int i = 2; i <= 132; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 8)
            {
                for (int i = 10; i <= 198; i++) // Assuming 1098 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 9)
            {
                for (int i = 10; i <= 891; i++) // Assuming 10 as the upper limit of available numbers
                {
                    availableNumbers.Add(i);
                }
            }
            else if (levelNum == 10)
            {
                if (numSumsGenerated <= 1)
                {
                    for (int i = 2; i <= 24; i++)
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("First difficulty");
                }
                else if (numSumsGenerated > 1 && numSumsGenerated <= 2)
                {
                    for (int i = 2; i <= 36; i++) 
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("Second difficulty");
                }
                else if (numSumsGenerated > 2 && numSumsGenerated <= 3)
                {
                    for (int i = 46; i <= 144; i++)
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("Third difficulty");
                }
                else if (numSumsGenerated > 3 && numSumsGenerated <= 6)
                {
                    for (int i = 20; i <= 132; i++)
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("Fourth difficulty");
                }
                else if (numSumsGenerated > 6 && numSumsGenerated <= 8)
                {
                    for (int i = 20; i <= 396; i++) 
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("Fifth difficulty");
                }
                else 
                {
                    for (int i = 30; i <= 891; i++)
                    {
                        availableNumbers.Add(i);
                    }
                    Debug.Log("Sixth difficulty");
                }
            }
        }
        
        

        // Remove the correct answer from the available numbers
        availableNumbers.Remove(answer);

        // Assign the correct answer to the correct button
        int correctAnswerIndex = Random.Range(0, availableNumbers.Count);
        int correctAnswer = availableNumbers[correctAnswerIndex];
        availableNumbers.RemoveAt(correctAnswerIndex);

        if (correctButton == 1)
        {
            button1.text = answer.ToString();
        }
        else if (correctButton == 2)
        {
            button2.text = answer.ToString();
        }
        else if (correctButton == 3)
        {
            button3.text = answer.ToString();
        }
        else if (correctButton == 4)
        {
            button4.text = answer.ToString();
        }

        // Assign other unique wrong answers to the remaining buttons
        for (int i = 1; i <= 4; i++)
        {
            if (i != correctButton)
            {
                int wrongAnswerIndex = Random.Range(0, availableNumbers.Count);
                int wrongAnswer = availableNumbers[wrongAnswerIndex];
                availableNumbers.RemoveAt(wrongAnswerIndex);

                if (i == 1)
                {
                    button1.text = wrongAnswer.ToString();
                }
                else if (i == 2)
                {
                    button2.text = wrongAnswer.ToString();
                }
                else if (i == 3)
                {
                    button3.text = wrongAnswer.ToString();
                }
                else if (i == 4)
                {
                    button4.text = wrongAnswer.ToString();
                }
            }
        }

        correctAns = false;
        // Start a timer
        solvingTime = 0f;
    }

    // Display button text for DDA
    private void DisplayButtonTextDDA(int FirstNum, int SecondNum, int Answer, int WrongAns1, int WrongAns2, int WrongAns3, string sumType)
    {
        // Enable all the buttons
        button1Image.enabled = true;
        button2Image.enabled = true;
        button3Image.enabled = true;
        button4Image.enabled = true;
        
        // First update the sum panel with the new numbers and choose a button to display the correct answer
        int aDigits = CountDigitsInInteger(FirstNum);
        int bDigits = CountDigitsInInteger(SecondNum);
        a = FirstNum;
        b = SecondNum;

        if (sumType == "plus")
        {
            if (aDigits == bDigits)
            {
                sum.text = "  " + a + "\n" + "+ " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sum.text = "  " + a + "\n" + " + " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sum.text = "  " + a + "\n" + "  + " + b;
            }
        }
        else if (sumType == "minus")
        {
            if (aDigits == bDigits)
            {
                sum.text = "  " + a + "\n" + "- " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sum.text = "  " + a + "\n" + " - " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sum.text = "  " + a + "\n" + "  - " + b;
            }
        }
        else
        {
            if (aDigits == bDigits)
            {
                sum.text = "  " + a + "\n" + "× " + b;
            }
            else if (aDigits == (bDigits+1))
            {
                sum.text = "  " + a + "\n" + " × " + b;
            }
            else if (aDigits == (bDigits + 2))
            {
                sum.text = "  " + a + "\n" + "  × " + b;
            }
        }
        // Choose a random button to display the correct answer
        int correctButtonIndex = Random.Range(1, 5);

        // Secondly update all the button text
        if (correctButtonIndex == 1)
        {
            button1.text = Answer.ToString();
            //Debug.Log("Correct button is 1 ans answer is: " + Answer);
        }
        else if (correctButtonIndex == 2)
        {
            button2.text = Answer.ToString();
            //Debug.Log("Correct button is 2 ans answer is: " + Answer);
        }
        else if (correctButtonIndex == 3)
        {
            button3.text = Answer.ToString();
            //Debug.Log("Correct button is 3 ans answer is: " + Answer);
        }
        else if (correctButtonIndex == 4)
        {
            button4.text = Answer.ToString();
            //Debug.Log("Correct button is 4 ans answer is: " + Answer);
        }

        // Assign the wrong answers to the remaining buttons
        if (correctButtonIndex == 1)
        {
            button2.text = WrongAns1.ToString();
            button3.text = WrongAns2.ToString();
            button4.text = WrongAns3.ToString();
            Debug.Log("Eq: (" + FirstNum + sumType + SecondNum + "), Ans: (" + Answer + ") Correct button is " + correctButtonIndex + " , with button text: " + button1.text + " Wrong answers are: Button 2: " + WrongAns1 + ", Button 3: " + WrongAns2 + ", Button 4: " + WrongAns3);
        }
        else if (correctButtonIndex == 2)
        {
            button1.text = WrongAns1.ToString();
            button3.text = WrongAns2.ToString();
            button4.text = WrongAns3.ToString();
            Debug.Log("Eq: (" + FirstNum + sumType + SecondNum + "), Ans: (" + Answer + ") Correct button is " + correctButtonIndex + " , with button text: " + button2.text + " Wrong answers are: Button 1: " + WrongAns1 + ", Button 3: " + WrongAns2 + ", Button 4: " + WrongAns3);
        }
        else if (correctButtonIndex == 3)
        {
            button1.text = WrongAns1.ToString();
            button2.text = WrongAns2.ToString();
            button4.text = WrongAns3.ToString();
            Debug.Log("Eq: (" + FirstNum + sumType + SecondNum + "), Ans: (" + Answer + ") Correct button is " + correctButtonIndex + " , with button text: " + button3.text + " Wrong answers are: Button 1: " + WrongAns1 + ", Button 2: " + WrongAns2 + ", Button 4: " + WrongAns3);
        }
        else if (correctButtonIndex == 4)
        {
            button1.text = WrongAns1.ToString();
            button2.text = WrongAns2.ToString();
            button3.text = WrongAns3.ToString();
            Debug.Log("Eq: (" + FirstNum + sumType + SecondNum + "), Ans: (" + Answer + ") Correct button is " + correctButtonIndex + " , with button text: " + button4.text + " Wrong answers are: Button 1: " + WrongAns1 + ", Button 2: " + WrongAns2 + ", Button 3: " + WrongAns3);
        }

        correctAns = false;
        // Start a timer
        solvingTime = 0f;

    }


    // Create an oncclick event for button1
    public void Button1Clicked()
    {
        CheckAnswer(button1, ans, sumType);
        Debug.Log("Button 1 clicked");
    }

    // Create an oncclick event for button2
    public void Button2Clicked()
    {
        CheckAnswer(button2, ans, sumType);
        Debug.Log("Button 2 clicked");
    }

    // Create an oncclick event for button3
    public void Button3Clicked()
    {
        CheckAnswer(button3, ans, sumType);
        Debug.Log("Button 3 clicked");
    }

    // Create an oncclick event for button4
    public void Button4Clicked()
    {
        CheckAnswer(button4, ans, sumType);
        Debug.Log("Button 4 clicked");
    }

    // Create a function that checks if the answer is correct
    public void CheckAnswer(TMP_Text button, int answer, string sum_type)
    {
        // Convert solving time to milliseconds integer
        int solvingTimeInt = (int)(solvingTime * 1000);
        // If it is the first sum, the minus 8000 miliseconds from the solving time
        if (numSumsGenerated == 1)
        {
            solvingTimeInt -= 8000;
        }

        if (button.text == answer.ToString())
        {
            // Add the equation to the list of equations
            if (wrongAns == false)
            {
                if (sum_type == "plus" && (a + b != 0))
                    plusEquations.AddEquation(a, b, int.Parse(button.text), true, solvingTimeInt);
                else if (sum_type == "minus" && (a + b != 0))
                    minusEquations.AddEquation(a, b, int.Parse(button.text), true, solvingTimeInt);
                else if (sum_type == "mul" && (a + b != 0))
                    mulEquations.AddEquation(a, b, int.Parse(button.text), true, solvingTimeInt);
                
            }
            correctAns = true;
        }
        else
        {
            // Add the equation to the list of equations
            if (sum_type == "plus")
                plusEquations.AddEquation(a, b, int.Parse(button.text), false, solvingTimeInt);
            else if (sum_type == "minus")
                minusEquations.AddEquation(a, b, int.Parse(button.text), false, solvingTimeInt);
            else
                mulEquations.AddEquation(a, b, int.Parse(button.text), false, solvingTimeInt);
            
            correctAns = false;
            wrongAns = true;
            dda.DeactivateDDA();
            UpdateWrong(correctButton);
        }
    }

    // Function that hides all the buttons except the correct one
    public void UpdateWrong(int correctButton)
    {
        if (correctButton == 1)
        {
            button2Image.enabled = false;
            button2.text = "";
            button3Image.enabled = false;
            button3.text = "";
            button4Image.enabled = false;
            button4.text = "";

        }
        else if (correctButton == 2)
        {
            button1Image.enabled = false;
            button1.text = "";
            button3Image.enabled = false;
            button3.text = "";
            button4Image.enabled = false;
            button4.text = "";
        }
        else if (correctButton == 3)
        {
            button1Image.enabled = false;
            button1.text = "";
            button2Image.enabled = false;
            button2.text = "";
            button4Image.enabled = false;
            button4.text = "";
        }
        else if (correctButton == 4)
        {
            button1Image.enabled = false;
            button1.text = "";
            button2Image.enabled = false;
            button2.text = "";
            button3Image.enabled = false;
            button3.text = "";
        }
    }

    // Onclick event for pause button
    public void PauseButtonClicked()
    {
        pausePanel.enabled = true;

        // Iterate through all child objects and enable them
        foreach (Transform child in pausePanel.transform)
        {
            child.gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // Onclick event for resume button
    public void ResumeButtonClicked()
    {
        pausePanel.enabled = false; // Disable the parent panel

        // Iterate through all child objects and disable them
        foreach (Transform child in pausePanel.transform)
        {
            child.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    // Onclick event for quit button
    public void QuitButtonClicked()
    {
        Time.timeScale = 1f;

        if (sumType == "plus")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Plus menu");
        }
        else if (sumType == "minus")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Minus menu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Mul menu");
        }
        
    }

    // Onclick event for quick race quit button
    public void QuickRaceQuitButtonClicked()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Quick race maps");
    }

    // Function to calculate the answer of the equation
    private int CalcAns(string sumType, int a, int b)
    {
        if (sumType == "plus")
        {
            return a + b;
        }
        else if (sumType == "minus")
        {
            return a - b;
        }
        else
        {
            return a * b;
        }
    }

    // Function to Determine DDA and new values before the text is updated
    private (bool,int,int,int,int,int,int) DetermineDDA(string sumType, int firstnum, int secondnum, int correctAns)
    {
        if (positionManager.playerPosition == 1)
        {
            (int new_firstNum, int new_secondNum, int new_ans, int wrongAns1, int wrongAns2, int wrongAns3) = dda.ActivateDDA(sumType, firstnum, secondnum, correctAns);
            return (true, new_firstNum, new_secondNum, new_ans, wrongAns1, wrongAns2, wrongAns3);
        }
        else
        {
            dda.DeactivateDDA();
            return (false, firstnum, secondnum, correctAns, 0, 0, 0);
        }
    }
}
