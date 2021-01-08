using UnityEngine;
using System.Collections.Generic;

using System;
using MathNet.Numerics.LinearAlgebra;

using Random = UnityEngine.Random;

namespace Coursework.Core
{
    public class NeuralNetwork
    {
        // VARIABLES
        private const int NUMBER_OF_INPUTS = 13;
        public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, NUMBER_OF_INPUTS);

        public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

        public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

        public List<Matrix<float>> weights = new List<Matrix<float>>();

        public List<float> biases = new List<float>();

        public float fitness;

        private int numberOfInputs;

        // EXECUTION FUNCTIONS
        public NeuralNetwork(int numberOfInputs) { 
            this.numberOfInputs = numberOfInputs;
            inputLayer = Matrix<float>.Build.Dense(1, numberOfInputs); 
        }

        // METHODS
        public void Initialize (int hiddenLayerCount, int hiddenNeuronCount)
        {
            inputLayer.Clear();
            hiddenLayers.Clear();
            outputLayer.Clear();
            weights.Clear();
            biases.Clear();

            for (int i = 0; i < hiddenLayerCount + 1; i++)
            {

                Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);

                hiddenLayers.Add(f);

                biases.Add(Random.Range(-1f, 1f));

                //WEIGHTS
                if (i == 0)
                {
                    Matrix<float> inputToH1 = Matrix<float>.Build.Dense(numberOfInputs, hiddenNeuronCount);
                    weights.Add(inputToH1);
                }

                Matrix<float> HiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
                weights.Add(HiddenToHidden);

            }

            Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
            weights.Add(OutputWeight);
            biases.Add(Random.Range(-1f, 1f));

            RandomiseWeights();

        }

        public NeuralNetwork InitializeCopy (int hiddenLayerCount, int hiddenNeuronCount)
        {
            NeuralNetwork n = new NeuralNetwork(numberOfInputs);

            List<Matrix<float>> newWeights = new List<Matrix<float>>();

            for (int i = 0; i < this.weights.Count; i++)
            {
                Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

                for (int x = 0; x < currentWeight.RowCount; x++)
                {
                    for (int y = 0; y < currentWeight.ColumnCount; y++)
                    {
                        currentWeight[x, y] = weights[i][x, y];
                    }
                }

                newWeights.Add(currentWeight);
            }

            List<float> newBiases = new List<float>();

            newBiases.AddRange(biases);

            n.weights = newWeights;
            n.biases = newBiases;

            n.InitializeHidden(hiddenLayerCount, hiddenNeuronCount);

            return n;
        }

        public void InitializeHidden (int hiddenLayerCount, int hiddenNeuronCount)
        {
            inputLayer.Clear();
            hiddenLayers.Clear();
            outputLayer.Clear();

            for (int i = 0; i < hiddenLayerCount + 1; i ++)
            {
                Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
                hiddenLayers.Add(newHiddenLayer);
            }

        }

        public void RandomiseWeights()
        {
            for (int i = 0; i < weights.Count; i++)
            {
                for (int x = 0; x < weights[i].RowCount; x++)
                {
                    for (int y = 0; y < weights[i].ColumnCount; y++)
                    {
                        weights[i][x, y] = Random.Range(-1f, 1f);
                    }
                }
            }
        }

        public (float, float) RunNetwork (float[] sensors)
        {
            for (int i = 0; i < sensors.Length; i++) {
                inputLayer[0, i] = sensors[i];
            }

            inputLayer = inputLayer.PointwiseTanh();

            for (int i = 0; i < hiddenLayers.Count; i++) {
                var layer = (i == 0) ? inputLayer : hiddenLayers[i - 1];
                hiddenLayers[i] = ((layer * weights[i]) + biases[i]).PointwiseTanh();
            }

            outputLayer = ((hiddenLayers[hiddenLayers.Count-1]*weights[weights.Count-1])+biases[biases.Count-1]).PointwiseTanh();

            //First output is acceleration and second output is steering
            return (Sigmoid(outputLayer[0,0]), (float)Math.Tanh(outputLayer[0,1]));
        }

        private float Sigmoid (float s) => (1 / (1 + Mathf.Exp(-s)));
    }
}