using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDataLoader : MonoBehaviour
{
    public List<int> LoadedTimes = new List<int>();
    public List<int> LoadedPositions = new List<int>();
    public List<int> EpisodeNumbers = new List<int>();
    public bool LoadingData = true;
    public bool DoublingData = true;

    private StarManager starManager;

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }
    }

    public void LoadData(int playerID)
    {
        //StartCoroutine(LoadPlayerData(playerID));
        LoadPlayerDataFromCSV();
    }

    /*private IEnumerator LoadPlayerData(int playerID)
    {
        /*string url = starManager.GetURL("load_player_data.php"); // Replace with your PHP script URL

        // Create a form to send the player ID
        WWWForm form = new WWWForm();
        form.AddField("playerID", playerID.ToString());

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseText = www.downloadHandler.text.Trim();
            if (responseText.StartsWith("Data loaded successfully"))
            {
                // Parse the data and populate the integer lists
                string[] dataLines = responseText.Split('\n');
                for (int i = 1; i < dataLines.Length; i++) // Start from 1 to skip the first line
                {
                    string[] values = dataLines[i].Split(',');
                    if (values.Length == 2)
                    {
                        if (int.TryParse(values[0], out int time) && int.TryParse(values[1], out int position))
                        {
                            LoadedTimes.Add(time);
                            LoadedPositions.Add(position);
                        }
                    }
                }
                Debug.Log("Player data loaded successfully.");
                LoadingData = false;
            }
            else
            {
                Debug.LogError("Unknown response: " + responseText);
            }
        }
        else
        {
            Debug.LogError("Error loading data from server: " + www.error);
        }

        // Dispose of the UnityWebRequest object to prevent memory leaks
        www.Dispose();
    }*/

    // Function to load the player data from csv file
    public void LoadPlayerDataFromCSV()
    {
        // Load the data from a CSV file
        string filePath = "C:/Users/Leon/OneDrive - North-West University/Documents/NWU 4/FYP/RL code/Python/PlayerData.csv";

        string[] lines = System.IO.File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the first line
        {
            string[] values = lines[i].Split(',');
            if (values.Length == 3)
            {
                if (int.TryParse(values[0], out int time) && int.TryParse(values[1], out int position) && int.TryParse(values[2], out int episode))
                {
                    LoadedTimes.Add(time);
                    LoadedPositions.Add(position);
                    EpisodeNumbers.Add(episode);
                }
            }
        }

        Debug.Log("Player data loaded successfully.");
        LoadingData = false;
    }

    public void DoubleData(int repeatCount)
    {
        int originalTimesCount = LoadedTimes.Count;
        int originalPositionsCount = LoadedPositions.Count;

        // Remove duplicate data from the lists
        for (int i = 0; i < LoadedTimes.Count - 1; i++)
        {
            if (LoadedTimes[i] == LoadedTimes[i + 1])
            {
                LoadedTimes.RemoveAt(i);
                LoadedPositions.RemoveAt(i);
                i--;
            }
        }

        // Duplicate the data
        for (int i = 0; i < repeatCount; i++)
        {
            // Step 1: Duplicate LoadedTimes
            for (int j = 0; j < originalTimesCount; j++)
            {
                LoadedTimes.Add(LoadedTimes[j]);
            }

            // Step 2: Duplicate LoadedPositions with Variance
            for (int j = 0; j < originalPositionsCount; j++)
            {
                int originalPosition = LoadedPositions[j];
                int variance = Random.Range(-3, 4); // Random value between -3 and 3
                LoadedPositions.Add(originalPosition + variance);
            }
        }

        // get the total number of episodes
        int totalEpisodes = (repeatCount+1) * 9;
        int currentEpisode = 1;
        int time = 0;
        int next_time = 1000;

        for (int i = 0; i < LoadedTimes.Count; i++)
        {
            if (i == LoadedTimes.Count - 1)
            {
                EpisodeNumbers.Add(currentEpisode);
            }
            else
            {
                time = LoadedTimes[i];
                next_time = LoadedTimes[i + 1];

                if (time < next_time)
                {
                    EpisodeNumbers.Add(currentEpisode);
                }
                else
                {
                    EpisodeNumbers.Add(currentEpisode);
                    currentEpisode++;
                }
            }
        }

        // Check if the max episode number is divisible by 9
        int maxEpisode = currentEpisode;

        if (maxEpisode % 9 != 0)
        {
            // Calculate the number of elements to remove to make it divisible by 9
            int elementsToRemove = maxEpisode % 9;

            // Loop to remove elements from the end of the lists
            for (int i = EpisodeNumbers.Count - 1; i >= 0 && elementsToRemove > 0; i--)
            {
                if (EpisodeNumbers[i] == maxEpisode)
                {
                    EpisodeNumbers.RemoveAt(i);
                    LoadedTimes.RemoveAt(i);
                    LoadedPositions.RemoveAt(i);
                    elementsToRemove--;
                }
            }
        }

        // Remove the last element in the lists
        EpisodeNumbers.RemoveAt(EpisodeNumbers.Count - 1);
        LoadedTimes.RemoveAt(LoadedTimes.Count - 1);
        LoadedPositions.RemoveAt(LoadedPositions.Count - 1);

        Debug.Log("LoadedTimes.Count: " + LoadedTimes.Count);
        Debug.Log("EpisodeNumbers.Count: " + EpisodeNumbers.Count);
        Debug.Log("Highest episode number: " + EpisodeNumbers[EpisodeNumbers.Count - 1]);

        // Save the LoadedTimes, LoadedPositions and EpisodeNumbers lists to a CSV file
        string filePath = "C:/Users/Leon/OneDrive - North-West University/Documents/NWU 4/FYP/RL code/Python/PlayerData.csv";
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
        {
            file.WriteLine("Time,Position,Episode");
            for (int i = 0; i < LoadedTimes.Count; i++)
            {
                file.WriteLine(LoadedTimes[i] + "," + LoadedPositions[i] + "," + EpisodeNumbers[i]);
            }
        }


        DoublingData = false;
    }

    public void DeleteUnnecessaryData()
    {
        // Delete LoadedTimes and LoadedPositions data if it contains data for more than 9 races
        int NumOfRaces = 0;

        for (int i = 0; i < LoadedTimes.Count; i++)
        {
            if (LoadedTimes[i] == 1000)
            {
                NumOfRaces++;
            }

            if (NumOfRaces == 10)
            {
                // Remove all the data in the lists from index i onwards and break out of the loop
                LoadedTimes.RemoveRange(i, LoadedTimes.Count - i);
                LoadedPositions.RemoveRange(i, LoadedPositions.Count - i);
                break;
            }
        }
    }

    public int GetMaxTime()
    {
        int maxTime = 0;
        for (int i = 0; i < LoadedTimes.Count; i++)
        {
            if (LoadedTimes[i] > maxTime)
            {
                maxTime = LoadedTimes[i];
            }
        }
        return maxTime;
    }


}
