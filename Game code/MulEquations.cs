using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MulEquations : MonoBehaviour
{
    private StarManager starManager; // Add a reference to the StarManager
    // Singleton instance
    private static MulEquations instance;

    public static MulEquations Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MulEquations>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("MulEquations");
                    instance = managerObject.AddComponent<MulEquations>();
                }
            }
            return instance;
        }
    }

    // Custom data structure to represent a summation equation
    public struct MultiplicationEquation
    {
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public int PlayerAnswer { get; set; }
        public bool IsCorrect { get; set; } // Boolean to store if the player's answer was correct
        public int Time { get; set; } // Time in milliseconds it took the player to solve the equation

        public MultiplicationEquation(int firstNumber, int secondNumber, int playerAnswer, bool isCorrect, int time)
        {
            FirstNumber = firstNumber;
            SecondNumber = secondNumber;
            PlayerAnswer = playerAnswer;
            IsCorrect = isCorrect;
            Time = time;
        }
    }

    public List<MultiplicationEquation> equationList = new List<MultiplicationEquation>();

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

        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }
    }

    // Make a public function that can be called to retrieve the list of equations
    public List<MultiplicationEquation> GetEquations()
    {
        return equationList;
    }

    // Make a public function that can be called to add an equation to the list
    public void AddEquation(int firstNumber, int secondNumber, int playerAnswer, bool isCorrect, int time)
    {
        MultiplicationEquation equation = new MultiplicationEquation(firstNumber, secondNumber, playerAnswer, isCorrect, time);

        equationList.Add(equation);

        // Print the contents of the equationList
        /*Debug.Log("Equation List Contents:");
        foreach (var eq in equationList)
        {
            Debug.Log("FirstNumber: " + eq.FirstNumber + ", SecondNumber: " + eq.SecondNumber + ", PlayerAnswer: " + eq.PlayerAnswer + ", IsCorrect: " + eq.IsCorrect);
        }*/
    }

    // Make a public function that can be called to save the list to the database
    public void SaveEquations()
    {
        StartCoroutine(SaveEquationsToDatabase());
    }

    private IEnumerator SaveEquationsToDatabase()
    {
        if (equationList.Count > 1)
        {
            // Get playerID from StarManager
            int playerID = starManager.playerID;
            
            string url = starManager.GetURL("save_equations_mul.php"); // Replace with your PHP script URL

            // Create a form to send data to the PHP script
            WWWForm form = new WWWForm();
            form.AddField("playerID", playerID.ToString());

            // Create arrays to store values for each field
            List<string> firstNumbers = new List<string>();
            List<string> secondNumbers = new List<string>();
            List<string> playerAnswers = new List<string>();
            List<string> isCorrectValues = new List<string>();
            List<string> times = new List<string>();

            foreach (var equation in equationList)
            {
                firstNumbers.Add(equation.FirstNumber.ToString());
                secondNumbers.Add(equation.SecondNumber.ToString());
                playerAnswers.Add(equation.PlayerAnswer.ToString());
                isCorrectValues.Add(equation.IsCorrect.ToString());
                times.Add(equation.Time.ToString());
            }

            // Add arrays to the form
            form.AddField("FirstNumber", string.Join(",", firstNumbers.ToArray()));
            form.AddField("SecondNumber", string.Join(",", secondNumbers.ToArray()));
            form.AddField("PlayerAnswer", string.Join(",", playerAnswers.ToArray()));
            form.AddField("IsCorrect", string.Join(",", isCorrectValues.ToArray()));
            form.AddField("Time", string.Join(",", times.ToArray()));

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text.Trim();
                if (responseText.StartsWith("Accuracy updated successfully."))
                {
                    Debug.Log("Accuracy updated successfully.");
                }
                else if (responseText == "success")
                {
                    Debug.Log("Subtraction equations saved successfully.");
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

        // Clear the lists
        equationList.Clear();
    }
}
