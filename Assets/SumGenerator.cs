using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SumGenerator : MonoBehaviour
{
    public TMP_Text sum;
    public TMP_Text button1;
    public TMP_Text button2;
    public TMP_Text button3;
    public TMP_Text button4;
    public Image button1Image;
    public Image button2Image;
    public Image button3Image;
    public Image button4Image;
    public int ans;
    public int correctButton;
    public bool correctAns;
    public bool wrongAns;
    public int numSumsGenerated; 

    // Start is called before the first frame update
    void Start()
    {
        correctAns = false;
        wrongAns = false;   
        numSumsGenerated = 0;
        UpdateAll();
    }

    // Function that updates the sum and the button texts
    public void UpdateAll()
    {
        (ans, correctButton) = GenerateSum(this);
        DisplayButtonText(correctButton, ans);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static (int, int) GenerateSum(SumGenerator sumGenerator)
    {
        // Update vaiables
        sumGenerator.numSumsGenerated++;
        print("Number of sums generated: " + sumGenerator.numSumsGenerated);

        // Generate two random numbers and display them in the sum text
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        //ans = a + b;
        sumGenerator.sum.text = "  " + a + "\n" + "+ " + b;

        // Choose a random button to display the correct answer
        int correctButton = Random.Range(1, 5);
        return (a+b, correctButton);
    }

    public void DisplayButtonText(int correctButton, int answer)
    {
        // Enable all the buttons
        button1Image.enabled = true;
        button2Image.enabled = true;
        button3Image.enabled = true;
        button4Image.enabled = true;

        // Display the correct answer in the correct button
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

        // Display the wrong answers in the other buttons
        for (int i = 1; i <= 4; i++)
        {
            if (i != correctButton)
            {
                int wrongAns = Random.Range(1, 20);
                while (wrongAns == answer)
                {
                    wrongAns = Random.Range(1, 20);
                }
                if (i == 1)
                {
                    button1.text = wrongAns.ToString();
                }
                else if (i == 2)
                {
                    button2.text = wrongAns.ToString();
                }
                else if (i == 3)
                {
                    button3.text = wrongAns.ToString();
                }
                else if (i == 4)
                {
                    button4.text = wrongAns.ToString();
                }

            }
        }

        correctAns = false;
    }

    // Create an oncclick event for button1
    public void Button1Clicked()
    {
        CheckAnswer(button1, ans);
    }

    // Create an oncclick event for button2
    public void Button2Clicked()
    {
        CheckAnswer(button2, ans);
    }

    // Create an oncclick event for button3
    public void Button3Clicked()
    {
        CheckAnswer(button3, ans);
    }

    // Create an oncclick event for button4
    public void Button4Clicked()
    {
        CheckAnswer(button4, ans);
    }

    // Create a function that checks if the answer is correct
    public void CheckAnswer(TMP_Text button, int answer)
    {
        if (button.text == answer.ToString())
        {
            correctAns = true;
        }
        else
        {
            correctAns = false;
            wrongAns = true;
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
}
