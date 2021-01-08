using UnityEngine;
using System.Collections.Generic;

using MathNet.Numerics.LinearAlgebra;
using Coursework.Utils;

namespace Coursework.Core
{
    public class GeneticManager : MonoBehaviour
    {
        // VARIABLES
        [Header("References")]
        public NetworkController controller;

        [Header("Controls")]
        public int initialPopulation = 85;
        [Range(0.0f, 1.0f)]
        public float mutationRate = 0.055f;

        [Header("Crossover Controls")]
        public int bestAgentSelection = 8;
        public int worstAgentSelection = 3;
        public int numberToCrossover;

        private List<int> genePool = new List<int>();

        private int naturallySelected;

        private NeuralNetwork[] population;

        [Header("Public View")]
        public int currentGeneration;
        public int currentGenome = 0;

        public static GeneticManager Instance { get; private set; }

        // EXECUTE FUNCTIONS
        private void Awake() => Instance = this;
        private void Start() => CreatePopulation();

        // METHODS
        private void CreatePopulation()
        {
            population = new NeuralNetwork[initialPopulation];
            FillPopulationWithRandomValues(population, 0);
            ResetToCurrentGenome();
        }

        private void ResetToCurrentGenome() => controller.ResetWithNetwork(population[currentGenome]);

        private void FillPopulationWithRandomValues (NeuralNetwork[] newPopulation, int startingIndex)
        {
            while (startingIndex < initialPopulation)
            {
                newPopulation[startingIndex] = new NeuralNetwork(controller.SENSORS);
                newPopulation[startingIndex].Initialize(controller.LAYERS, controller.NEURONS);
                startingIndex++;
            }
        }

        public void Kill(float fitness, NeuralNetwork network)
        {
            if (currentGenome < population.Length -1)
            {
                population[currentGenome].fitness = fitness;
                currentGenome++;
                ResetToCurrentGenome();
            }
            else Repopulate();
        }

        
        private void Repopulate()
        {
            genePool.Clear();
            currentGeneration++;
            naturallySelected = 0;
            SortingAlgorithms.BubbleSort(ref population);

            var newPopulation = PickBestPopulation();

            Crossover(newPopulation);
            Mutate(newPopulation);

            FillPopulationWithRandomValues(newPopulation, naturallySelected);

            population = newPopulation;

            currentGenome = 0;

            ResetToCurrentGenome();
        }

        private void Mutate(NeuralNetwork[] newPopulation)
        {
            for (int i = 0; i < naturallySelected; i++)
            {
                for (int c = 0; c < newPopulation[i].weights.Count; c++)
                {
                    if (Random.Range(0.0f, 1.0f) < mutationRate)
                    {
                        newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                    }
                }
            }
        }

        private Matrix<float> MutateMatrix(Matrix<float> A)
        {
            int randomPoints = Random.Range(1, (A.RowCount * A.ColumnCount) / 7);

            Matrix<float> C = A;

            for (int i = 0; i < randomPoints; i++)
            {
                int randomColumn = Random.Range(0, C.ColumnCount);
                int randomRow = Random.Range(0, C.RowCount);

                C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
            }

            return C;
        }

        private void Crossover (NeuralNetwork[] newPopulation)
        {
            for (int i = 0; i < numberToCrossover; i+=2)
            {
                int aIndex = i;
                int bIndex = i + 1;

                if (genePool.Count >= 1)
                {
                    for (int l = 0; l < 100; l++)
                    {
                        aIndex = genePool[Random.Range(0, genePool.Count)];
                        bIndex = genePool[Random.Range(0, genePool.Count)];

                        if (aIndex != bIndex)
                            break;
                    }
                }

                var child1 = new NeuralNetwork(controller.SENSORS);
                var child2 = new NeuralNetwork(controller.SENSORS);

                child1.Initialize(controller.LAYERS, controller.NEURONS);
                child2.Initialize(controller.LAYERS, controller.NEURONS);

                child1.fitness = 0;
                child2.fitness = 0;

                for (int w = 0; w < child1.weights.Count; w++)
                {
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        child1.weights[w] = population[aIndex].weights[w];
                        child2.weights[w] = population[bIndex].weights[w];
                    }
                    else
                    {
                        child2.weights[w] = population[aIndex].weights[w];
                        child1.weights[w] = population[bIndex].weights[w];
                    }
                }

                for (int w = 0; w < child1.biases.Count; w++)
                {
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                    {
                        child1.biases[w] = population[aIndex].biases[w];
                        child2.biases[w] = population[bIndex].biases[w];
                    }
                    else
                    {
                        child2.biases[w] = population[aIndex].biases[w];
                        child1.biases[w] = population[bIndex].biases[w];
                    }
                }

                newPopulation[naturallySelected] = child1;
                naturallySelected++;

                newPopulation[naturallySelected] = child2;
                naturallySelected++;
            }
        }

        private NeuralNetwork[] PickBestPopulation()
        {
            var newPopulation = new NeuralNetwork[initialPopulation];

            for (int i = 0; i < bestAgentSelection; i++)
            {
                newPopulation[naturallySelected] = population[i].InitializeCopy(controller.LAYERS, controller.NEURONS);
                newPopulation[naturallySelected].fitness = 0;
                naturallySelected++;
                
                int f = Mathf.RoundToInt(population[i].fitness * 10);

                for (int c = 0; c < f; c++)
                {
                    genePool.Add(i);
                }
            }

            for (int i = 0; i < worstAgentSelection; i++)
            {
                int last = population.Length - 1;
                last -= i;

                int f = Mathf.RoundToInt(population[last].fitness * 10);

                for (int c = 0; c < f; c++)
                {
                    genePool.Add(last);
                }
            }

            return newPopulation;
        }
    }
}