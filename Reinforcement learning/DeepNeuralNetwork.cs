using UnityEngine;
using UnityEngine.UI;

public class DeepNeuralNetwork : MonoBehaviour
{
    public int inputSize = 3;
    public int hiddenLayerSize1 = 16;
    public int hiddenLayerSize2 = 16;
    public int outputSize = 2;

    // Neural network weights and biases
    private float[,] inputToHidden1Weights;
    private float[] hidden1Biases;
    private float[,] hidden1ToHidden2Weights;
    private float[] hidden2Biases;
    private float[,] hidden2ToOutputWeights;
    private float[] outputBiases;

    public TargetNetwork targetNetwork; // Reference to the target network

    private System.Random random = new System.Random();

    public void InitializeCurrentNetwork()
    {
        // Initialize weights and biases randomly
        inputToHidden1Weights = InitializeWeights(3, hiddenLayerSize1);

        // Initialize weights and biases for hidden layers
        hidden1ToHidden2Weights = InitializeWeights(hiddenLayerSize1, hiddenLayerSize2);
        hidden2ToOutputWeights = InitializeWeights(hiddenLayerSize2, outputSize);

        hidden1Biases = InitializeBiases(hiddenLayerSize1);
        hidden2Biases = InitializeBiases(hiddenLayerSize2);

        outputBiases = InitializeBiases(outputSize);
    }

    public void UpdateTargetNetwork()
    {
        // Copy weights and biases from the current network to the target network
        targetNetwork.targetinputToHidden1Weights = (float[,])inputToHidden1Weights.Clone();

        // debug dimensions of inputtohidden1weights
        //Debug.Log("Dimensions of inputToHidden1Weights: " + inputToHidden1Weights.GetLength(0) + " x " + inputToHidden1Weights.GetLength(1));

        targetNetwork.targethidden1Biases = (float[])hidden1Biases.Clone();
        targetNetwork.targethidden1ToHidden2Weights = (float[,])hidden1ToHidden2Weights.Clone();
        targetNetwork.targethidden2Biases = (float[])hidden2Biases.Clone();
        targetNetwork.targethidden2ToOutputWeights = (float[,])hidden2ToOutputWeights.Clone();
        targetNetwork.targetoutputBiases = (float[])outputBiases.Clone();
    }

    private float[,] InitializeWeights(int input_size, int output_size)
    {
        float[,] weights = new float[input_size, output_size];
        for (int i = 0; i < input_size; i++)
        {
            for (int j = 0; j < output_size; j++)
            {
                weights[i, j] = (float)random.NextDouble() * 2 - 1; // Random values between -1 and 1
            }
        }
        return weights;
    }

    private float[] InitializeBiases(int size)
    {
        float[] biases = new float[size];
        for (int i = 0; i < size; i++)
        {
            biases[i] = (float)random.NextDouble() * 2 - 1; // Random values between -1 and 1
        }
        return biases;
    }

    private (float[], float[], float[]) ForwardPass(float[] inputVector)
    {
        float[] hiddenLayerOutput1 = new float[hiddenLayerSize1];
        float[] hiddenLayerOutput2 = new float[hiddenLayerSize2];
        float[] outputLayerOutput = new float[outputSize];

        // Forward pass for the first hidden layer
        for (int i = 0; i < hiddenLayerSize1; i++)
        {
            hiddenLayerOutput1[i] = 0.0f;
            for (int j = 0; j < inputSize; j++)
            {
                hiddenLayerOutput1[i] += inputVector[j] * inputToHidden1Weights[j, i];
            }
            hiddenLayerOutput1[i] += hidden1Biases[i];
            hiddenLayerOutput1[i] = Mathf.Max(0.0f, hiddenLayerOutput1[i]); // ReLU activation
            //hiddenLayerOutput1[i] = Tanh(hiddenLayerOutput1[i]);
        }

        // Forward pass for the second hidden layer
        for (int i = 0; i < hiddenLayerSize2; i++)
        {
            hiddenLayerOutput2[i] = 0.0f;
            for (int j = 0; j < hiddenLayerSize1; j++)
            {
                hiddenLayerOutput2[i] += hiddenLayerOutput1[j] * hidden1ToHidden2Weights[j, i];
            }
            hiddenLayerOutput2[i] += hidden2Biases[i];
            hiddenLayerOutput2[i] = Mathf.Max(0.0f, hiddenLayerOutput2[i]); // ReLU activation
            //hiddenLayerOutput2[i] = Tanh(hiddenLayerOutput2[i]);
        }

        // Forward pass for the output layer
        for (int i = 0; i < outputSize; i++)
        {
            outputLayerOutput[i] = 0.0f;
            for (int j = 0; j < hiddenLayerSize2; j++)
            {
                outputLayerOutput[i] += hiddenLayerOutput2[j] * hidden2ToOutputWeights[j, i];
            }
            outputLayerOutput[i] += outputBiases[i];
        }

        return (outputLayerOutput, hiddenLayerOutput1, hiddenLayerOutput2);
    }

    public int GetBestAction(float currentXPosition, float currentRelPosition, float currentTime)
    {
        float[] inputVector = { currentXPosition, currentRelPosition, currentTime };
        (float[] outputLayerOutput, float[] hiddenLayerOutput1, float[] hiddenLayerOutput2) = ForwardPass(inputVector);

        // Determine the best action (0 or 1) based on the Q-values
        int bestAction = (outputLayerOutput[0] > outputLayerOutput[1]) ? 0 : 1;

        return bestAction;
    }

    public float GetMaxQValue(float currentXPosition, float currentRelPosition, float currentTime)
    {
        float[] inputVector = { currentXPosition, currentRelPosition, currentTime };
        (float[] outputLayerOutput, float[] hiddenLayerOutput1, float[] hiddenLayerOutput2) = targetNetwork.DeepForwardPass(inputVector);

        return Mathf.Max(outputLayerOutput[0], outputLayerOutput[1]);
    }

    // Function to update the neural network using stochastic gradient descent
    public void StochasticGradientDescent(float xPosition, float relPosition, float timeMs, float target, float learningRate)
    {
        // Create an input vector from the provided parameters
        float[] inputVector = { xPosition, relPosition, timeMs };
        
        // Forward pass to compute the network's output
        (float[] output, float[] hiddenLayerOutput1, float[] hiddenLayerOutput2) = ForwardPass(inputVector);

        // Calculate the loss (e.g., mean squared error) based on the provided target
        float[] loss = new float[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            loss[i] = (target - output[i]) * (target - output[i]); // (yj − Q(φj , aj ; θ))^2
        }

        // Backpropagation
        float[] deltaOutput = new float[outputSize];
        float[] deltaHidden2 = new float[hiddenLayerSize2];
        float[] deltaHidden1 = new float[hiddenLayerSize1];

        // Update the output layer weights and biases
        for (int i = 0; i < outputSize; i++)
        {
            deltaOutput[i] = loss[i]; // Calculate the gradient for the output layer
            for (int j = 0; j < hiddenLayerSize2; j++)
            {
                hidden2ToOutputWeights[j, i] += learningRate * deltaOutput[i] * hiddenLayerOutput2[j];
            }
            outputBiases[i] += learningRate * deltaOutput[i];
        }

        // Backpropagate the gradient to the second hidden layer
        for (int i = 0; i < hiddenLayerSize2; i++)
        {
            deltaHidden2[i] = 0;
            for (int j = 0; j < outputSize; j++)
            {
                deltaHidden2[i] += deltaOutput[j] * hidden2ToOutputWeights[i, j];
            }
            
            // Apply the derivative of the ReLU activation function
            deltaHidden2[i] *= (hiddenLayerOutput2[i] > 0) ? 1 : 0;

            // Update the second hidden layer weights and biases
            for (int j = 0; j < hiddenLayerSize1; j++)
            {
                hidden1ToHidden2Weights[j, i] += learningRate * deltaHidden2[i] * hiddenLayerOutput1[j];
            }
            hidden2Biases[i] += learningRate * deltaHidden2[i];
        }

        // Backpropagate the gradient to the first hidden layer
        for (int i = 0; i < hiddenLayerSize1; i++)
        {
            deltaHidden1[i] = 0;
            for (int j = 0; j < hiddenLayerSize2; j++)
            {
                deltaHidden1[i] += deltaHidden2[j] * hidden1ToHidden2Weights[i, j];
            }
            
            // Apply the derivative of the ReLU activation function
            deltaHidden1[i] *= (hiddenLayerOutput1[i] > 0) ? 1 : 0;

            // Update the first hidden layer weights and biases
            for (int j = 0; j < inputSize; j++)
            {
                inputToHidden1Weights[j, i] += learningRate * deltaHidden1[i] * inputVector[j];
            }
            hidden1Biases[i] += learningRate * deltaHidden1[i];
        }
    }



}

