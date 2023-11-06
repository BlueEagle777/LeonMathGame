using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class TargetNetwork : MonoBehaviour
{
    private int targetinputSize = 3;
    private int targethiddenLayerSize = 16;
    private int targetoutputSize = 2;

    // Neural network weights and biases
    public float[,] targetinputToHiddenWeights;
    public float[] targethiddenBiases;
    public float[,] targethiddenToOutputWeights;
    public float[] targetoutputBiases;

    public (float[], float[]) ForwardPass(float[] inputVector)
    {
        float[] hiddenLayerOutput = new float[targethiddenLayerSize];
        float[] outputLayerOutput = new float[targetoutputSize];

        for (int i = 0; i < targethiddenLayerSize; i++)
        {
            hiddenLayerOutput[i] = 0.0f;
            for (int j = 0; j < targetinputSize; j++)
            {
                hiddenLayerOutput[i] += inputVector[j] * targetinputToHiddenWeights[j, i];
            }
            hiddenLayerOutput[i] += targethiddenBiases[i];
            hiddenLayerOutput[i] = Mathf.Max(0.0f, hiddenLayerOutput[i]); // ReLU activation
        }

        for (int i = 0; i < targetoutputSize; i++)
        {
            outputLayerOutput[i] = 0.0f;
            for (int j = 0; j < targethiddenLayerSize; j++)
            {
                outputLayerOutput[i] += hiddenLayerOutput[j] * targethiddenToOutputWeights[j, i];
            }
            outputLayerOutput[i] += targetoutputBiases[i];
        }
        
        return (outputLayerOutput, hiddenLayerOutput);
    }
}