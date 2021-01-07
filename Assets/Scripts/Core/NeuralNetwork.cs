using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Coursework.Core
{
    public class NeuralNetwork
    {
        // VARIABLES
        public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 8);        
        public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
        public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

        public List<Matrix<float>> weights = new List<Matrix<float>>();
        public List<float> biases = new List<float>();
    
        public float fitness;

        // METHODS
        public void Initialize(int inputLayerSize, int hiddenLayerCount, int hiddenNeuronCount) {
            inputLayer = Matrix<float>.Build.Dense(1, inputLayerSize);
            
            inputLayer.Clear();
            hiddenLayers.Clear();
            outputLayer.Clear();
            weights.Clear();
            biases.Clear();

            for (int i = 0; i <= hiddenLayerCount; i++) {
                var f = Matrix<float>.Build.Dense(1, hiddenLayerCount);
                hiddenLayers.Add(f);

                biases.Add(UnityEngine.Random.Range(-1f, 1f));

                // Set up weights
                if (i == 0)
                {
                    var inputToH1 = Matrix<float>.Build.Dense(inputLayerSize, hiddenNeuronCount);
                    weights.Add(inputToH1);
                }

                var hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
                weights.Add(hiddenToHidden);
            }

            var outputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
            weights.Add(outputWeight);
            biases.Add(UnityEngine.Random.Range(-1f, 1f));

            RandomizeWeights();
        }

        public NeuralNetwork InitializeCopy(int inputLayerSize, int hiddenLayerCount, int hiddenNeuronCount) {
            var n = new NeuralNetwork();

            var newWeights = new List<Matrix<float>>();

            for (int i = 0; i < weights.Count; i++) {
                var currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

                for (int x = 0; x < currentWeight.RowCount; x++)
                {
                    for (int y = 0; y < currentWeight.ColumnCount; y++)
                    {
                        currentWeight[x, y] = weights[i][x, y];
                    }
                }

                newWeights.Add(currentWeight);
            }

            var newBiases = new List<float>();
            newBiases.AddRange(biases);

            n.weights = newWeights;
            n.biases = newBiases;

            n.InitializeHidden(inputLayerSize, hiddenLayerCount, hiddenNeuronCount);

            return n;
        }

        public void InitializeHidden(int inputLayerSize, int hiddenLayerCount, int hiddenNeuronCount)
        {
            inputLayer.Clear();
            hiddenLayers.Clear();
            outputLayer.Clear();

            for (int i = 0; i <= hiddenLayerCount; i++)
            {
                var newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
                hiddenLayers.Add(newHiddenLayer);
            }
        }

        private void RandomizeWeights() {
            for (int i = 0; i < weights.Count; i++)
            {
                for (int x = 0; x < weights[i].RowCount; x++)
                {
                    for (int y = 0; y < weights[i].ColumnCount; y++)
                    {
                        weights[i][x, y] = UnityEngine.Random.Range(-1f, 1f);
                    }
                }
            }
        }

        public (float, float) RunNetwork(float[] sensors) {
            for (int i = 0; i < sensors.Length; i++) {
                inputLayer[0, i] = sensors[i];
            }

            inputLayer = inputLayer.PointwiseTanh();

            for (int i = 0; i < hiddenLayers.Count; i++) {
                var layer = (i == 0) ? inputLayer : hiddenLayers[i - 1];
                hiddenLayers[i] = ((layer * weights[i]) + biases[i]).PointwiseTanh();
            }
            
            outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

            // First output is X and second output is Y
            return ((float)Math.Tanh(outputLayer[0, 0]), (float)Math.Tanh(outputLayer[0, 1]));
        }
    }
}

