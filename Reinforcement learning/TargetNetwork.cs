using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class TargetNetwork : MonoBehaviour
{
    private int targetinputSize = 3;
    private int targethiddenLayerSize1 = 16;
    private int targethiddenLayerSize2 = 16;
    private int targetoutputSize = 2;

    // Shallow neural network weights and biases
    public float[,] targetinputToHiddenWeights;
    public float[] targethiddenBiases;
    public float[,] targethiddenToOutputWeights;
    public float[] targetoutputBiases;

    // Deep neural network weights and biases
    public float[,] targetinputToHidden1Weights;
    public float[] targethidden1Biases;
    public float[,] targethidden1ToHidden2Weights;
    public float[] targethidden2Biases;
    public float[,] targethidden2ToOutputWeights;
    public float[] targetoutputBiases;

    public (float[], float[]) ForwardPass(float[] inputVector)
    {
        float[] hiddenLayerOutput = new float[targethiddenLayerSize1];
        float[] outputLayerOutput = new float[targetoutputSize];

        for (int i = 0; i < targethiddenLayerSize1; i++)
        {
            hiddenLayerOutput[i] = 0.0f;
            for (int j = 0; j < targetinputSize; j++)
            {
                hiddenLayerOutput[i] += inputVector[j] * targetinputToHiddenWeights[j, i];
            }
            hiddenLayerOutput[i] += targethiddenBiases[i];
            //hiddenLayerOutput[i] = Mathf.Max(0.0f, hiddenLayerOutput[i]); // ReLU activation
            hiddenLayerOutput[i] = 1.0f / (1.0f + Mathf.Exp(-hiddenLayerOutput[i])); // Sigmoid activation
        }

        for (int i = 0; i < targetoutputSize; i++)
        {
            outputLayerOutput[i] = 0.0f;
            for (int j = 0; j < targethiddenLayerSize1; j++)
            {
                outputLayerOutput[i] += hiddenLayerOutput[j] * targethiddenToOutputWeights[j, i];
            }
            outputLayerOutput[i] += targetoutputBiases[i];
        }
        
        return (outputLayerOutput, hiddenLayerOutput);
    }

    public (float[], float[], float[]) DeepForwardPass(float[] inputVector)
    {
        float[] targetHiddenLayerOutput1 = new float[targethiddenLayerSize1];
        float[] targetHiddenLayerOutput2 = new float[targethiddenLayerSize2];
        float[] targetOutputLayerOutput = new float[targetoutputSize];

        //Debug.Log("Dimensions of targetinputToHidden1Weights: " + targetinputToHidden1Weights.GetLength(0) + " x " + targetinputToHidden1Weights.GetLength(1));

        // Forward pass for the first hidden layer
        for (int i = 0; i < targethiddenLayerSize1; i++)
        {
            targetHiddenLayerOutput1[i] = 0.0f;
            for (int j = 0; j < targetinputSize; j++)
            {
                targetHiddenLayerOutput1[i] += inputVector[j] * targetinputToHidden1Weights[j, i];
            }
            targetHiddenLayerOutput1[i] += targethidden1Biases[i];
            targetHiddenLayerOutput1[i] = Mathf.Max(0.0f, targetHiddenLayerOutput1[i]); // ReLU activation
            //targetHiddenLayerOutput1[i] = Tanh(targetHiddenLayerOutput1[i]);
        }

        // Forward pass for the second hidden layer
        for (int i = 0; i < targethiddenLayerSize2; i++)
        {
            targetHiddenLayerOutput2[i] = 0.0f;
            for (int j = 0; j < targethiddenLayerSize1; j++)
            {
                targetHiddenLayerOutput2[i] += targetHiddenLayerOutput1[j] * targethidden1ToHidden2Weights[j, i];
            }
            targetHiddenLayerOutput2[i] += targethidden2Biases[i];
            targetHiddenLayerOutput2[i] = Mathf.Max(0.0f, targetHiddenLayerOutput2[i]); // ReLU activation
            //targetHiddenLayerOutput2[i] = Tanh(targetHiddenLayerOutput2[i]);
        }

        // Forward pass for the output layer
        for (int i = 0; i < targetoutputSize; i++)
        {
            targetOutputLayerOutput[i] = 0.0f;
            for (int j = 0; j < targethiddenLayerSize2; j++)
            {
                targetOutputLayerOutput[i] += targetHiddenLayerOutput2[j] * targethidden2ToOutputWeights[j, i];
            }
            targetOutputLayerOutput[i] += targetoutputBiases[i];
        }

        return (targetOutputLayerOutput, targetHiddenLayerOutput1, targetHiddenLayerOutput2);
    }

}