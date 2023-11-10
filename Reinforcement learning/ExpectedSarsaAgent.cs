using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ExpectedSarsaAgent : MonoBehaviour
{
    private StarManager starManager;
    private PlayerDataLoader dataLoader;

    // Define the number of states for position_interval and rel_position
    private int numRelPositionStates = 3; // BEHIND, ON, FRONT

    private int numStates = 30; // 30 states (State1 to State30)
    private int numActions = 2; // 2 actions (Drive and Boost)

    // Hyperparameters for the ExpectedSarsa algorithm
    private float learningRate = 0.05f; // Learning rate (alpha)
    private float discountFactor = 0.9f; // Discount factor (gamma)
    private float epsilon = 0.1f; // Epsilon-greedy exploration parameter
    private int maxEpisodes = 900; // Maximum number of episodes

    // Initialize the Q-table with zeros
    private float[,] QTable;

    // Other variables
    private int time = 0;
    private int next_time = 0;
    private int agentPosition;
    // List that stores the cumulative rewards
    private int cumulativeReward = 0;
    public List<int> cumulativeRewards = new List<int>();
    public List<int> episodeNumbers = new List<int>();

    private void Awake()
    {
        // Find the existing StarManager instance in the scene
        starManager = FindObjectOfType<StarManager>();

        if (starManager == null)
        {
            Debug.LogError("StarManager not found in the scene!");
        }

        // Find the existing PlayerDataLoader instance in the scene
        dataLoader = FindObjectOfType<PlayerDataLoader>();
    }

    public void StartExpectedSarsa()
    {
        if (dataLoader != null)
        {
            // Load the player's data from the server
            dataLoader.LoadData(starManager.playerID);
            
            // Train the Q-learning agent after the data is loaded
            StartCoroutine(TrainExpectedSarsaAfterDataLoaded(maxEpisodes));
        }
        else
        {
            Debug.LogError("PlayerDataLoader not found in the scene!");
        }
    }

    private IEnumerator TrainExpectedSarsaAfterDataLoaded(int numEpisodes)
    {
        while (dataLoader.LoadingData) // Check if data is still being loaded
        {
            yield return null; // Wait for a frame
        }

        // Expand the recorded player data
        dataLoader.DoubleData((maxEpisodes / 9) - 1);

        while (dataLoader.DoublingData) // Check if data is still being loaded
        {
            yield return null; // Wait for a frame
        }

        // At this point, the data loading is complete

        // Print the Q-table
        /*Debug.Log("Q-table (begin):");
        for (int state = 0; state < numStates; state++)
        {
            for (int action = 0; action < numActions; action++)
            {
                Debug.Log("QTable[" + state + ", " + action + "] = " + QTable[state, action]);
            }
        }*/

        // Train the ExpectedSarsa agent
        TrainExpectedSarsaAgent(numEpisodes);
    }



    // Training the ExpectedSarsa agent
    private void TrainExpectedSarsaAgent(int numEpisodes)
    {
        // Step 1 - Initialize the Q-table with zeros
        QTable = new float[numStates, numActions];

        // Step 2 - Repeat for each episode
        for (int episode = 0; episode < numEpisodes; episode++)
        {
            // Variable initialization at the start of each race
            time = 0;
            next_time = 2;
            agentPosition = 1;
            
            // Step 3 - Initialize the state
            int currentState = MapState(time, agentPosition);

            // Step 4 - Repeat for each step of the episode
            while (!CheckTerminalState(episode))
            { 
                // Step 5 - Choose an action using an epsilon-greedy policy
                int currentAction = EpsilonGreedyPolicy(currentState);
                
                // Step 6 - Take the chosen action, observe reward and next state
                agentPosition = TakeAction(currentAction, agentPosition);
                int reward = CalculateReward(agentPosition);
                int nextState = MapState(time, agentPosition);
                
                // update the cummalative rewards and episodes lists
                cumulativeReward += reward;
                cumulativeRewards.Add(cumulativeReward);
                episodeNumbers.Add(episode);

                // Step 7 - Update Q(s, a) using the Q-learning update rule
                UpdateQValue(currentState, currentAction, reward, nextState);

                // Step 8 - Transition to the next state
                currentState = nextState;
            }
        }

        Debug.Log("Training complete.");
        // print the cumulative rewards
        /*foreach (int reward in cumulativeRewards)
        {
            Debug.Log(reward);
        }*/

        // Print the Q-table
        /*Debug.Log("Q-table (end):");
        for (int state = 0; state < numStates; state++)
        {
            for (int action = 0; action < numActions; action++)
            {
                Debug.Log("QTable[" + state + ", " + action + "] = " + QTable[state, action]);
            }
        }*/

        // Save the Q-table to starManager
        starManager.QTable = QTable;

        // Print the bossActions list
        /*Debug.Log("bossActions:");
        foreach (int action in starManager.bossActions)
        {
            Debug.Log(action);
        }*/

        // Export the cumulative rewards to a CSV file
        ExportCumulativeRewardsToCSV();
    }

    // Export the cumulative rewards to a CSV file
    private void ExportCumulativeRewardsToCSV()
    {
        string filePath =  "C:/Users/Leon/OneDrive - North-West University/Documents/NWU 4/FYP/RL code/Python/CumulativeRewardsExpectedSarsa.csv";
        string delimiter = ",";

        // Create a StringBuilder to populate the CSV file
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Add the header line
        sb.AppendLine("TimeStep" + delimiter + "Episode" + delimiter + "Cumulative Reward");

        // Add the cumulative rewards
        for (int timeStep = 1; timeStep <= cumulativeRewards.Count; timeStep++)
        {
            sb.AppendLine(timeStep + delimiter + episodeNumbers[timeStep-1] + delimiter + cumulativeRewards[timeStep-1]);
        }

        // Write the CSV file
        System.IO.File.WriteAllText(filePath, sb.ToString());

        Debug.Log("Cumulative rewards exported to " + filePath);
    }




    // Map (position_interval, rel_position) to a state in the range State1 - State30
    private int MapState(int time, int agentPosition)
    {
        int playerPosition;
        int index;
        if (time == 0)
        {
            playerPosition = 3;
        } else {
            index = dataLoader.LoadedTimes.IndexOf(time*1000);
            playerPosition = dataLoader.LoadedPositions[index];
            try
            {
                next_time = dataLoader.LoadedTimes[index + 2];
                next_time = next_time / 1000;
            }
            catch (ArgumentOutOfRangeException)
            {
                next_time = 1;
            }
            
        }

        int relativePosition = agentPosition - playerPosition;
        int relPosition;

        // Map the playerPosition and the relativePosition to State1 - State30
        int positionInterval = playerPosition / 100;
        // BEHIND
        if (relativePosition < -5)
        {
            relPosition = 0;
        }
        // ON
        else if (relativePosition > -5 && relativePosition < 15)
        {
            relPosition = 1;
        }
        // FRONT
        else
        {
            relPosition = 2;
        }

        // Calculate the mapped state
        int state = positionInterval * numRelPositionStates + relPosition + 1;


        //Debug.Log("state: " + state + " (positionInterval: " + positionInterval + ", relPosition: " + relPosition + ")");
        return state;
    }

    // Function to take an action
    private int TakeAction(int action, int position)
    {
        float driveSpeed = 12.67f;
        float boostSpeed = 14.63f;
        int duration = 2; // Drive and boost duration time in seconds
        int newPosition = 0;

        if (action == 0)
        {
            newPosition = position + (int)(driveSpeed * duration);
        }
        else if (action == 1)
        {
            newPosition = position + (int)(boostSpeed * duration);
        }

        time += duration;

        return newPosition;
    }

    // Epsilon-greedy policy for action selection
    private int EpsilonGreedyPolicy(int state)
    {
        if (UnityEngine.Random.value < epsilon)
        {
            // Explore: Choose a random action
            return UnityEngine.Random.Range(0, numActions);
        }
        else
        {
            // Exploit: Choose the action with the highest Q-value
            float maxQ = QTable[state, 0];
            int bestAction = 0;

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

    // Update the Q-value using the ExpectedSarsa update rule
    private void UpdateQValue(int state, int action, int reward, int nextState)
    {
        // Expected Sarsa update rule
        float currentQ = QTable[state, action]; // Q(s, a)
        float VsPrime = 0.0f;

        // Calculate the policy from the QTable
        float[,] policy = InitializePolicy();

        // Calculate Vs' by summing over all possible actions in the next state
        for (int a = 0; a < numActions; a++)
        {
            VsPrime += policy[nextState, a] * QTable[nextState, a]; // ∑π(s', a)·Q(s', a)
        }

        // Update the Q-value for the current state-action pair
        float newQ = currentQ + learningRate * (reward + discountFactor * VsPrime - currentQ);
        QTable[state, action] = newQ;
    }


    // Function to check if the current state is a terminal state
    private bool CheckTerminalState(int currentEpisode)
    {
        //Debug.Log("Time: " + time + ", Next time: " + next_time);
        
        if (time < next_time)
        {
            return false;
        }
        else
        {
            UpdatePlayerData(currentEpisode);
            return true;
        }
    }

    // Function to calculate the reward
    private int CalculateReward(int agentPosition)
    {
        int timeMS = time * 1000; // Time in milliseconds
        // Get the index of dataLoader.LoadedTimes where the time is equal to timeMS
        int index = dataLoader.LoadedTimes.IndexOf(timeMS);
        //Debug.Log("time: " + time);
        //Debug.Log("index: " + index);
        int playerPosition = dataLoader.LoadedPositions[index];
        int relativePosition = agentPosition - playerPosition;
        int reward;

        // Calculate the reward
        // BEHIND
        if (relativePosition < -5)
        {
            reward = -1;
        }
        // ON
        else if (relativePosition > -5 && relativePosition < 15)
        {
            reward = 1;
        }
        // FRONT
        else
        {
            reward = -1;
        }

        return reward;

    }

    private float[,] InitializePolicy()
    {
        float[,] policy = new float[numStates, numActions];

        // Sum all the values in the QTable
        float sum = 0.0f;
        for (int state = 0; state < numStates; state++)
        {
            for (int action = 0; action < numActions; action++)
            {
                sum += QTable[state, action];
            }
        }

        // Divide each value in the QTable by the sum to get the policy
        if (sum != 0.0f)
        {
            for (int state = 0; state < numStates; state++)
            {
                for (int action = 0; action < numActions; action++)
                {
                    policy[state, action] = QTable[state, action] / sum;
                }
            }
        }
        else
        {
            policy = QTable;
        }

        return policy;
    }

    // Function to update the player's data
    private void UpdatePlayerData(int currentEpisode)
    {
        if (currentEpisode + 1 < 900)
        {
            // Determine the range of indices where dataLoader.EpisodeNumbers == currentEpisode + 1
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < dataLoader.EpisodeNumbers.Count; i++)
            {
                if (dataLoader.EpisodeNumbers[i] == currentEpisode + 1)
                {
                    startIndex = i;
                    break;
                }
            }

            for (int i = startIndex; i < dataLoader.EpisodeNumbers.Count; i++)
            {
                if (dataLoader.EpisodeNumbers[i] == currentEpisode + 2)
                {
                    endIndex = i;
                    break;
                }
            }

            //Debug.Log("currentEpisode: " + currentEpisode + ", startIndex: " + startIndex + ", endIndex: " + endIndex);

            // Remove all the data in dataLoader.LoadedTimes and dataLoader.LoadedPositions that are in the range of indices
            dataLoader.LoadedTimes.RemoveRange(startIndex, endIndex - startIndex);
            dataLoader.LoadedPositions.RemoveRange(startIndex, endIndex - startIndex);
            //Debug.Log("Success");

            // Re-index the lists to start at index 0 again
            int numItemsToRemove = endIndex - startIndex;
            for (int i = 0; i < numItemsToRemove; i++)
            {
                dataLoader.LoadedTimes.Insert(i, dataLoader.LoadedTimes[startIndex + i]);
                dataLoader.LoadedPositions.Insert(i, dataLoader.LoadedPositions[startIndex + i]);
            }
        }
    }

    
}
