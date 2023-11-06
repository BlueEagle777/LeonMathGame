using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DQLAgent : MonoBehaviour
{
    // Define the initial parameters and variables
    private int C = 1000; // Number of iterations between each update of the target network
    private float alpha = 0.0001f; // Learning rate
    private float gamma = 0.9f; // Discount factor
    private float epsilon = 0.1f; // Epsilon-greedy exploration parameter
    private ReplayBuffer D; // Replay buffer D
    private int minibatchSize = 32;
    private int t = 0; // Counter for the total number of iterations

    // Other variables
    private int time = 0;
    private int next_time = 0;
    private int maxTimeMS;
    private int agentPosition;
    private int cumulativeReward = 0;
    public List<int> cumulativeRewards = new List<int>();

    // Add reference to other scripts
    public NeuralNetwork neuralNetwork;
    private StarManager starManager;
    private PlayerDataLoader dataLoader;

    // Struct to represent the agent's state
    public struct State
    {
        public float xPosition;
        public float relPosition;
        public float timeMs;
    }

    public class ReplayBuffer
    {
        // Tuple of (s, a, r, s', isTerminal)
        private List<(State, int, int, State, bool)> buffer;
        private System.Random random; // Random instance

        // Maximum capacity of the replay buffer
        public int Capacity { get; private set; }

        // Constructor
        public ReplayBuffer(int capacity)
        {
            Capacity = capacity;
            buffer = new List<(State, int, int, State, bool)>(capacity);
            random = new System.Random(); // Initialize Random here
        }

        public void Initialize()
        {
            buffer.Clear(); // Clear the buffer to initialize it
        }

        public void AddEntry(State currentState, int action, int reward, State nextState, bool isTerminal)
        {
            // Add an entry to the replay buffer
            buffer.Add((currentState, action, reward, nextState, isTerminal));

            // If the buffer size exceeds its capacity, remove the oldest entry
            if (buffer.Count > Capacity)
            {
                buffer.RemoveAt(0);
            }
        }

        public List<(State, int, int, State, bool)> SampleMinibatch(int minibatchSize)
        {
            List<(State, int, int, State, bool)> minibatch = new List<(State, int, int, State, bool)>();

            if (buffer.Count < minibatchSize)
            {
                // Not enough samples in the buffer yet.
                // You may want to handle this differently, e.g., return null or an empty list.
                return minibatch;
            }

            // Randomly sample 'minibatchSize' transitions from the buffer
            for (int i = 0; i < minibatchSize; i++)
            {
                int randomIndex = random.Next(0, buffer.Count);
                minibatch.Add(buffer[randomIndex]);
            }

            return minibatch;
        }
    }

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

        // Find the existing NeuralNetwork instance in the scene
        neuralNetwork = FindObjectOfType<NeuralNetwork>();
    }

    public void StartDQL()
    {
        if (dataLoader != null)
        {
            // Load the player's data from the server
            dataLoader.LoadData(starManager.playerID);
            
            // Train the Q-learning agent after the data is loaded
            StartCoroutine(TrainDQLAfterDataLoaded(900));
        }
        else
        {
            Debug.LogError("PlayerDataLoader not found in the scene!");
        }
    }

    private IEnumerator TrainDQLAfterDataLoaded(int numEpisodes)
    {
        while (dataLoader.LoadingData) // Check if data is still being loaded
        {
            yield return null; // Wait for a frame
        }
        
        // Expand the recorded player data
        /*dataLoader.DoubleData(99);

        while (dataLoader.DoublingData) // Check if data is still being doubled
        {
            yield return null;
        }

        // At this point, the data loading is complete*/
        
        // Get the maximum time in milliseconds
        maxTimeMS = dataLoader.GetMaxTime();

        // Train the DQL agent
        TrainDQLAgent(numEpisodes);
    }



    // Training the DQL agent
    private void TrainDQLAgent(int numEpisodes)
    {
        // Line 1 - Initialize replay memory D to capacity N
        int N = 10000; // Replay buffer capacity
        D = new ReplayBuffer(N); // Initialize the replay buffer
        D.Initialize();

        // Line 2 - Initialize action-value function Q with random weights θ
        neuralNetwork.InitializeCurrentNetwork();

        // Line 3 - Initialize target action-value function Q^ with weights θ− = θ
        neuralNetwork.UpdateTargetNetwork();

        // Line 4 - for episode = 1, M do
        for (int episode = 0; episode < numEpisodes; episode++)
        {
            // Line 5 - Initialise initail state s1 and preprocessed sequenced φ1 = φ(s1)
            time = 0; // Variable initialization at the start of each race
            next_time = 2;
            agentPosition = 1;

            (int new_agentPosition, int new_relativePosition, int new_time) = MapState(time, agentPosition);
            (float agentPosition_norm, float relativePosition_norm, float time_norm) = ProcessState(new_agentPosition, new_relativePosition, new_time);
            State currentState = new State { xPosition = agentPosition_norm, relPosition = relativePosition_norm, timeMs = time_norm };
            
            // Line 6 - for t = 1, T do (T is the time steps in an episode)
            do
            //for (int i = 0; i < 20; i++)
            {
                // Line 7 - Sample action at from policy Q(φt, a; θ) [epsilon-greedy]
                int action = SampleAction(epsilon, currentState.xPosition, currentState.relPosition, currentState.timeMs);

                // Line 8 - Execute action at in emulator and observe reward rt and image xt+1
                int newAgentPosition = TakeAction(action, agentPosition); // Kyk dat agentPostion update word
                int reward = CalculateReward(newAgentPosition);
                cumulativeReward += reward; // update the cummalative rewards list
                cumulativeRewards.Add(cumulativeReward);

                // Line 9 - Determine the next state st+1 and preprocess φt+1 = φ(st+1)
                (new_agentPosition, new_relativePosition, new_time) = MapState(time, newAgentPosition);
                (agentPosition_norm, relativePosition_norm, time_norm) = ProcessState(new_agentPosition, new_relativePosition, new_time);
                State nextState = new State { xPosition = agentPosition_norm, relPosition = relativePosition_norm, timeMs = time_norm };

                // Line 10 - Determine if st+1 is terminal
                bool isTerminal = CheckTerminalState();

                // Line 11 - Store transition (φt, at, rt, φt+1, terminal) in D
                StoreExperience(currentState, action, reward, nextState, isTerminal);

                // Line 12 - Sample random minibatch of transitions (φj , aj , rj , φj+1, terminal) from D
                List<(State, int, int, State, bool)> minibatch = D.SampleMinibatch(minibatchSize);


                if (minibatch.Count == minibatchSize)
                {
                    for (int j = 0; j < minibatchSize; j++)
                    {
                        State phi_j = minibatch[j].Item1;
                        int aj = minibatch[j].Item2;
                        int rj = minibatch[j].Item3;
                        State phi_j1 = minibatch[j].Item4;
                        bool isterminal = minibatch[j].Item5;
                        
                        float target;
                        // Line 13 - if terminal is true: (epsode terminates at step j + 1)
                        if (isterminal == true)
                        {
                            // Line 14 - Set yj = rj
                            target = rj;
                        }
                        else // Line 15 - otherwise
                        {
                            // Line 16 - Set yj = rj + γ maxa′ Q_cap(φj+1, a′; θ-)
                            target = rj + gamma * neuralNetwork.GetMaxQValue(phi_j1.xPosition, phi_j1.relPosition, phi_j1.timeMs);
                        } // Line 17 - end if

                        // Line 18 - Perform a gradient descent step on (yj − Q(φj , aj ; θ))^2 with respect to θ
                        neuralNetwork.StochasticGradientDescent(phi_j.xPosition, phi_j.relPosition, phi_j.timeMs, target, alpha);
                    }

                    // Clear the minibatch
                    minibatch.Clear();
                }
                else
                {
                    //Debug.Log("Not enough samples in the buffer yet:" + t);
                }

                // Increment t
                t++;
                
                // Line 19 - Update Q^ = Q every C steps
                if (t % C == 0)
                {
                    neuralNetwork.UpdateTargetNetwork();
                }
                
                // Update state variable
                currentState = nextState;
                agentPosition = newAgentPosition;

            } while (!CheckTerminalState()); // Line 20 - until termination (end of first for)

            // Update the player data
            UpdatePlayerData(episode);

        } // Line 21 - end for

        Debug.Log("Training complete!");

        // Export the cumulative rewards to a CSV file
        ExportCumulativeRewardsToCSV();
    }

    // Export the cumulative rewards to a CSV file
    private void ExportCumulativeRewardsToCSV()
    {
        string filePath =  "C:/Users/Leon/OneDrive - North-West University/Documents/NWU 4/FYP/RL code/Python/CumulativeRewardsDQL.csv";
        string delimiter = ",";

        // Create a StringBuilder to populate the CSV file
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Add the header line
        sb.AppendLine("Episode" + delimiter + "Cumulative Reward");

        // Add the cumulative rewards
        for (int episode = 0; episode < cumulativeRewards.Count; episode++)
        {
            sb.AppendLine(episode + delimiter + cumulativeRewards[episode]);
        }

        // Write the CSV file
        System.IO.File.WriteAllText(filePath, sb.ToString());

        Debug.Log("Cumulative rewards exported to " + filePath);
    }

    private (int, int, int) MapState(int time, int agentPosition)
    {
        int playerPosition;
        int index;
        if (time == 0)
        {
            playerPosition = 3;
        } else {
            index = dataLoader.LoadedTimes.IndexOf(time*1000);
            playerPosition = dataLoader.LoadedPositions[index];
            next_time = dataLoader.LoadedTimes[index+2];
            next_time = next_time / 1000;
        }

        int relativePosition = agentPosition - playerPosition;
        int relPosition;

        
        // BEHIND
        if (relativePosition < -2)
        {
            relPosition = 1;
        }
        // ON
        else if (relativePosition >= -2 && relativePosition < 10)
        {
            relPosition = 2;
        }
        // FRONT
        else
        {
            relPosition = 3;
        }
        
        // 
        return (agentPosition, relPosition, time);
    }


    // Function to process the state (from s to phi)
    private (float, float, float) ProcessState(int xPosition, int relPosition, int time)
    {
        // Normalize the state variables
        float xPosition_norm = (float)xPosition / 800;
        float relPosition_norm = (float)relPosition / 3;
        float timeMs_norm = (float)(time*1000) / maxTimeMS;

        return (xPosition_norm, relPosition_norm, timeMs_norm);
    }
    
    // Function to sample an action
    private int SampleAction(float epsilon, float currentxPosition, float currentRelPosition, float currentTime)
    {
        if (Random.value < epsilon)
        {
            // Explore: Choose a random action
            return Random.Range(0, 2); // Assuming two possible actions (0 and 1)
        }
        else
        {
            // Exploit: Choose the action with the highest Q-value
            int bestAction = neuralNetwork.GetBestAction(currentxPosition, currentRelPosition, currentTime);
            return bestAction;
        }
    }

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
        if (relativePosition < -2)
        {
            reward = -1;
        }
        // ON
        else if (relativePosition > -2 && relativePosition < 10)
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

    private void UpdateTargetNetwork()
    {
        // Implement logic to update the target Q-network weights
        if (t % C == 0)
        {
            // Update wMinus to match w (copy the weights)
            neuralNetwork.UpdateTargetNetwork();
        }
    }

    private void StoreExperience(State currentState, int action, int reward, State nextState, bool isTerminal)
    {
        D.AddEntry(currentState, action, reward, nextState, isTerminal);
    }

    // Function to check if the current state is a terminal state
    private bool CheckTerminalState()
    {
        //Debug.Log("Time: " + time + ", Next time: " + next_time);
        
        if (time < next_time)
        {
            return false;
        }
        else
        {
            return true;
        }
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
