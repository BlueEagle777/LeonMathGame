using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DDA : MonoBehaviour
{
    private int DDA_Level = 0;
    private int raceNumber;

    private RaceResultsManager raceResultsManager; // Add a reference to the RaceResultsManager
    
    // Singleton instance
    private static DDA instance;

    public static DDA Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DDA>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("DDA");
                    instance = managerObject.AddComponent<DDA>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int GetRaceNumber()
    {
        // Find the existing RaceResultsManager instance in the scene
        raceResultsManager = FindObjectOfType<RaceResultsManager>();

        if (raceResultsManager == null)
        {
            Debug.LogError("RaceResultsManager not found in the scene!");
        }

        return raceResultsManager.raceNumber;
    }

    // Function to Activate DDA and return (num1, num2, correctAns, WrongAns1, WrongAns2, WrongAns3)
    public (int,int,int,int,int,int) ActivateDDA(string sumType, int firstnum, int secondnum, int correctAns)
    {
        raceNumber = GetRaceNumber();
        
        if (DDA_Level < 3)
        {
            DDA_Level++;
        }
        else if (DDA_Level == 3)
        {
            DDA_Level = 2;
        }
        Debug.Log("DDA activated Level: " + DDA_Level);
        
        // Their are 3 levels of DDA

        if (DDA_Level == 1) // Decrease the range of the wrong answers
        {
            // Level 1 DDA
            (int wrongAns1, int wrongAns2, int wrongAns3) = DecreaseRange(correctAns);
            //Debug.Log("Level 1 DDA returns: firstnum: " + firstnum + ", secondnum: " + secondnum + ", correctAns: " + correctAns + ", wrongAns1: " + wrongAns1 + ", wrongAns2: " + wrongAns2 + ", wrongAns3: " + wrongAns3);
            return (firstnum, secondnum, correctAns, wrongAns1, wrongAns2, wrongAns3);
        }
        else if (DDA_Level == 2) // Increase the amount of carries
        {
            // Level 2 DDA
            
            // Firstly get the new numbers
            int new_firstNum = GetSecondNum(sumType, firstnum, secondnum).Item1;
            int new_secondNum = GetSecondNum(sumType, firstnum, secondnum).Item2;

            // Secondly, calculate the new correct answer
            int newCorrectAns = 0;
            if (sumType == "plus")
            {
                newCorrectAns = new_firstNum + new_secondNum;
            }
            else if (sumType == "minus")
            {
                newCorrectAns = new_firstNum - new_secondNum;
            }
            else if (sumType == "mul")
            {
                newCorrectAns = new_firstNum * new_secondNum;
            }

            // Thridly decrease the range and return the new numbers
            (int wrongAns1, int wrongAns2, int wrongAns3) = DecreaseRange(newCorrectAns);
            return (new_firstNum, new_secondNum, newCorrectAns, wrongAns1, wrongAns2, wrongAns3);
            
        }
        else if (DDA_Level == 3) // Make the wrong answer numbers look the same
        {
            // Level 3 DDA

            // Make a new list for the wrong numbers
            List<int> wrongNumbers = new List<int>();
            // Add the reult of the MakeWrongAnswersLookSame function to the list
            (int wrongAns1, int wrongAns2, int wrongAns3) = MakeWrongAnswersLookSame(sumType, correctAns, firstnum, secondnum);
            // Add the wrongAns1 - wrongAns3 variables in a random order to the list
            int rnd = Random.Range(0, 3);
            if (rnd == 0)
            {
                wrongNumbers.Add(wrongAns1);
                wrongNumbers.Add(wrongAns2);
                wrongNumbers.Add(wrongAns3);
            }
            else if (rnd == 1)
            {
                wrongNumbers.Add(wrongAns1);
                wrongNumbers.Add(wrongAns3);
                wrongNumbers.Add(wrongAns2);
            }
            else
            {
                wrongNumbers.Add(wrongAns2);
                wrongNumbers.Add(wrongAns1);
                wrongNumbers.Add(wrongAns3);
            }

            return (firstnum, secondnum, correctAns, wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
        }
        else
        {
            // No DDA - return the original numbers
            return (firstnum, secondnum, correctAns, 0, 0, 0);
        }
    }

    // Function to deactivate DDA
    public void DeactivateDDA()
    {
        Debug.Log("DDA deactivated");
        DDA_Level = 0;
    }

    // Level 1 DDA - decreae the range of the wrong answers
    private (int,int,int) DecreaseRange(int correctAns)
    {
        List<int> wrongNumbers;
        switch (raceNumber)
        {
            case 1:
            case 2:
                wrongNumbers = GenerateWrongAnswers(correctAns, -4, 4);
                return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
            case 3:
            case 4:
                wrongNumbers = GenerateWrongAnswers(correctAns, -10, 10);
                return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
            case 5:
            case 6:
                wrongNumbers = GenerateWrongAnswers(correctAns, -15, 15);
                return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
            case 7:
            case 8:
            case 9:
            case 10:
                wrongNumbers = GenerateWrongAnswers(correctAns, -20, 20);
                return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
            default:
                wrongNumbers = GenerateWrongAnswers(correctAns, -25, 25);
                return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);

        }
    }

    // Level 1 DDA
    List<int> GenerateWrongAnswers(int correctAnswer, int minValue, int maxValue)
    {
        List<int> uniqueNumbers = GetUniqueRandomNumbers(minValue, maxValue, correctAnswer);
        uniqueNumbers[0] = correctAnswer + uniqueNumbers[0];
        uniqueNumbers[1] = correctAnswer + uniqueNumbers[1];
        uniqueNumbers[2] = correctAnswer + uniqueNumbers[2];
        //Debug.Log("Wrong answers: " + uniqueNumbers[0] + ", " + uniqueNumbers[1] + ", " + uniqueNumbers[2]);

        return uniqueNumbers;
    }

    
    // Level 1 DDA - Function to return 3 unique random numbers
    private List<int> GetUniqueRandomNumbers(int min, int max, int correctAnswer)
    {
        List<int> uniqueNumbers = new List<int>();
        while (uniqueNumbers.Count < 3)
        {
            int randomNumber = Random.Range(min, max+1);
            if (!uniqueNumbers.Contains(randomNumber) && randomNumber != 0 && (randomNumber + correctAnswer) > 0)
            {
                uniqueNumbers.Add(randomNumber);
            }
        }
        return uniqueNumbers;
    }


    // Level 2 DDA - increae the amount of carries
    private (int,int) GetSecondNum(string sumType, int firstnum, int secondnum)
    {   
        if (sumType == "plus")
        {
            // Get the rightmost digit of the 2-digit first number
            int rightmostDigit = firstnum % 10;

            // Ensure that the rightmost digit is not 0
            if (rightmostDigit == 0)
            {
                firstnum = firstnum + 1;
            }

            if (raceNumber == 6 || raceNumber == 8 || raceNumber == 9)
            {
                return (firstnum, GenerateNum2(firstnum));
            } else {
                return (firstnum, secondnum);
            }
        }
        else if (sumType == "minus")
        {
            // Get the rightmost digit of the 2-digit second number
            int rightmostDigit = secondnum % 10;

            // Ensure that the rightmost digit is not 0
            if (rightmostDigit == 0)
            {
                secondnum = secondnum + 1;
            }

            if (raceNumber == 8 || raceNumber == 9)
            {
                return (GenerateNum1(secondnum), secondnum);
            } else {
                return (firstnum, secondnum);
            }

        }
        else
        {
            return (firstnum, secondnum);
        }
    }
    

    // Level 2 DDA - (For plus) Function to generate the second number
    private int GenerateNum2(int num1)
    {
        int num2 = 0;
        
        if (raceNumber == 6 || raceNumber == 8)
        {
            num2 = 10; // Initialize num2 to the minimum value
        }
        else if (raceNumber == 9)
        {
            num2 = 100; // Initialize num2 to the minimum value
        }

        while (CountCarries(num1, num2) != 2)
        {
            num2++;
        }

        return num2;
    }

    // Level 2 DDA - Function to count the number of carries
    private int CountCarries(int a, int b)
    {
        int carries = 0;
        int carry = 0;

        while (a > 0 || b > 0)
        {
            int sum = a % 10 + b % 10 + carry;
            if (sum >= 10)
            {
                carries++;
                carry = 1;
            }
            else
            {
                carry = 0;
            }

            a /= 10;
            b /= 10;
        }

        return carries;
    }

    // Level 2 DDA - (For minus) Function to generate the first number
    private int GenerateNum1(int num2)
    {
        int num1 = 0;
        
        num1 = 100; // Initialize num1 to the minimum value for a 3-digit number

        while (CountBorrows(num1, num2) != 2)
        {
            num1++;
        }

        return num1;
    }

    // Level 2 DDA - Function to count the number of borrows
    private int CountBorrows(int a, int b)
    {
        int borrows = 0;
        int borrow = 0;

        while (a > 0 || b > 0)
        {
            int diff = a % 10 - b % 10 - borrow;
            if (diff < 0)
            {
                borrows++;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }

            a /= 10;
            b /= 10;
        }

        return borrows;
    }

    // Level 3 DDA - Make the wrong answer numbers look the same
    private (int,int,int) MakeWrongAnswersLookSame(string sumType, int correctAns, int firstNum, int secondNum)
    {
        List<int> wrongNumbers = new List<int>(); // Initialize the list

        if (sumType == "plus")
        { 
            if (correctAns < 10)
            {
                GenerateOneDigitAnsPlus(firstNum, secondNum, correctAns, wrongNumbers);
            }
            else if (correctAns >= 10 && correctAns <= 99)
            {
                GenerateTwoDigitAns(correctAns, wrongNumbers);
            }
            else if (correctAns >= 100 && correctAns <= 999)
            {
                GenerateThreeDigitAns(correctAns, wrongNumbers);
            }
            else if (correctAns >= 1000 && correctAns <= 9999)
            {
                GenerateFourDigitAns(correctAns, wrongNumbers);
            }
        }
        else if (sumType == "minus")
        {
            if (correctAns < 10)
            {
                GenerateOneDigitAnsMin(firstNum, secondNum, correctAns, wrongNumbers);
            }
            else if (correctAns >= 10 && correctAns <= 99)
            {
                GenerateTwoDigitAns(correctAns, wrongNumbers);
            }
            else if (correctAns >= 100 && correctAns <= 999)
            {
                GenerateThreeDigitAns(correctAns, wrongNumbers);
            }
        }
        else
        {
            if (correctAns < 10)
            {
                GenerateOneDigitAnsMul(firstNum, secondNum, correctAns, wrongNumbers);
            }
            else if (correctAns >= 10 && correctAns <= 99)
            {
                GenerateTwoDigitAns(correctAns, wrongNumbers);
            }
            else if (correctAns >= 100 && correctAns <= 999)
            {
                GenerateThreeDigitAns(correctAns, wrongNumbers);
            }
        }

        return (wrongNumbers[0], wrongNumbers[1], wrongNumbers[2]);
    }

    // Level 3 DDA - Function to return the max and min numer of 2 numbers
    private (int,int) GetMaxMin(int num1, int num2)
    {
        int max = 0;
        int min = 0;

        if (num1 > num2)
        {
            max = num1;
            min = num2;
        }
        else
        {
            max = num2;
            min = num1;
        }

        return (max, min);
    }

    // Level 3 DDA - Function to add unique number to wrongNumbers List
    private void AddUniqueWrongNumber(List<int> wrongNumbers, int number, int correctAns)
    {
        if (number == correctAns)
        {
            number++; // Increment if it's the same as correctAns
        }

        while (wrongNumbers.Contains(number) || number < 0 || number == correctAns)
        {
            number++; // Increment until a unique positive value is found
        }

        wrongNumbers.Add(number);
    }

    // Level 3 DDA - Function to generate 1 digit wrong answers for plus
    private void GenerateOneDigitAnsPlus(int firstNum, int secondNum, int correctAns, List<int> wrongNumbers)
    {
        // Get the max min of firstNum and secondNum
        (int max, int min) = GetMaxMin(firstNum, secondNum);
        AddUniqueWrongNumber(wrongNumbers, correctAns + 1, correctAns);
        AddUniqueWrongNumber(wrongNumbers, correctAns - 1, correctAns);
        AddUniqueWrongNumber(wrongNumbers, max - min, correctAns);
    }

    // Level 3 DDA - Function to generate 1 digit wrong answers for minus
    private void GenerateOneDigitAnsMin(int firstNum, int secondNum, int correctAns, List<int> wrongNumbers)
    {
        AddUniqueWrongNumber(wrongNumbers, correctAns + 1, correctAns);
        AddUniqueWrongNumber(wrongNumbers, correctAns - 1, correctAns);
        AddUniqueWrongNumber(wrongNumbers, firstNum + secondNum, correctAns);
    }
    
    // Level 3 DDA - Function to generate 2 digit wrong answers
    private void GenerateTwoDigitAns(int correctAns, List<int> wrongNumbers)
    {
        // Get the leftmost digit of the 2-digit first number as an int
        int leftmostDigit = correctAns / 10;
        //Create a new leftmost digit
        int newLeftmostDigit1 = leftmostDigit + 1;
        int newLeftmostDigit2 = leftmostDigit - 1;
        // Get the rightmost digit of the correctAns as an int
        int rightmostDigit = correctAns % 10;
        AddUniqueWrongNumber(wrongNumbers, (newLeftmostDigit1*10)+rightmostDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (newLeftmostDigit2*10)+rightmostDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (rightmostDigit*10)+leftmostDigit, correctAns); // Switch the digits
    }

    // Level 3 DDA - Function to generate 3 digit wrong answers
    private void GenerateThreeDigitAns(int correctAns, List<int> wrongNumbers)
    {
        // get the middle digit of the correctAns as an int
        int middleDigit = (correctAns / 10) % 10;
        if (middleDigit == 0) { middleDigit++;}
        if (middleDigit == 9) { middleDigit--;}
                    
        // Create a new middle digit
        int newMiddleDigit1 = middleDigit + 1;
        int newMiddleDigit2 = middleDigit - 1;
        // Get the rightmost digit of the correctAns as an int
        int rightmostDigit = correctAns % 10;
        // Get the leftmost digit of the correctAns as an int
        int leftmostDigit = correctAns / 100;
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*100)+(newMiddleDigit1*10)+rightmostDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*100)+(newMiddleDigit2*10)+rightmostDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*100)+(rightmostDigit*10)+middleDigit, correctAns); // Switch the digits
    }

    // Level 3 DDA - Function to generate 4 digit wrong answers
    private void GenerateFourDigitAns(int correctAns, List<int> wrongNumbers)
    {
        // get the middle digit of the correctAns as an int
        int middleDigit = (correctAns / 10) % 10;
        if (middleDigit == 0) { middleDigit++;}
        if (middleDigit == 9) { middleDigit--;}
                    
        // Create a new middle digit
        int newMiddleDigit1 = middleDigit + 1;
        int newMiddleDigit2 = middleDigit - 1;
        // Get the rightmost digit of the correctAns as an int
        int rightmostDigit = correctAns % 10;
        // Get the leftmost digit of the correctAns as an int
        int leftmostDigit = correctAns / 1000;
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*1000)+(newMiddleDigit1*100)+(rightmostDigit*10)+middleDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*1000)+(newMiddleDigit2*100)+(rightmostDigit*10)+middleDigit, correctAns);
        AddUniqueWrongNumber(wrongNumbers, (leftmostDigit*1000)+(rightmostDigit*100)+(middleDigit*10)+newMiddleDigit1, correctAns); // Switch the digits
    }

    // Level 3 DDA - Function to generate one digit answers for multiplication
    private void GenerateOneDigitAnsMul(int firstNum, int secondNum, int correctAns, List<int> wrongNumbers)
    {
        (int min, int max) = GetMaxMin(firstNum, secondNum);
        AddUniqueWrongNumber(wrongNumbers, max - min, correctAns);
        AddUniqueWrongNumber(wrongNumbers, correctAns + 1, correctAns);
        AddUniqueWrongNumber(wrongNumbers, firstNum + secondNum, correctAns);
    }
}
