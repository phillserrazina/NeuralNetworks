using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;

namespace Coursework.Core.Entities
{
    public class GeneticManager : MonoBehaviour
    {
        // VARIABLES
        private NNetController controller;

        [Header("Controls")]
        [SerializeField] private int initialPopulation = 50;
        [Range(0.0f, 1.0f)] private float mutationRate = 0.055f;

        [Header("Crossover Controls")]
        [SerializeField] private int bestAgentSelection = 8;
        [SerializeField] private int worstAgentSelection = 3;
        [SerializeField] private int numberToCrossover;

        private List<int> genePool = new List<int>();

        private int naturallySelected;

        private NeuralNetwork[] population;

        [Header("Public View")]
        [SerializeField] private int currentGeneration;
        [SerializeField] private int currentGenome;

        // METHODS
        public void Initialize(NNetController controller) {
            this.controller = controller;
            CreatePopulation();
        }

        private void CreatePopulation() {
            population = new NeuralNetwork[initialPopulation];

            FillPopulationWithRandomValues(population, 0);
            ResetToCurrentGenome();
        }

        private void ResetToCurrentGenome() {
            controller.ResetWithNetwork(population[currentGenome]);
        }

        private void FillPopulationWithRandomValues(NeuralNetwork[] newPopulation, int startingIndex) 
        {
            while(startingIndex < initialPopulation) 
            {
                newPopulation[startingIndex] = new NeuralNetwork();
                newPopulation[startingIndex].Initialize(controller.NumOfSensors, controller.Layers, controller.Neurons);
                startingIndex++;
            }
        }

        public void Kill(float fitness, NeuralNetwork network) 
        {
            
            if (currentGenome < population.Length - 1) 
            {
                population[currentGenome].fitness = fitness;
                currentGenome++;
                ResetToCurrentGenome();
            }
            else
            {
                RePopulate();
            }
        }

        private void RePopulate() {
            genePool.Clear();
            currentGeneration++;
            naturallySelected = 0;

            SortPopulation();

            var newPopulation = PickBestPopulation();

            Crossover(newPopulation);
            Mutate(newPopulation);

            FillPopulationWithRandomValues(newPopulation, naturallySelected);

            population = newPopulation;
            currentGenome = 0;
            ResetToCurrentGenome();
        }

        private void Crossover(NeuralNetwork[] newPopulation)
        {
            for (int i = 0; i < numberToCrossover; i += 2) {
                int AIndex = i;
                int BIndex = i + 1;

                if (genePool.Count >= 1)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        AIndex = genePool[Random.Range(0, genePool.Count)];
                        BIndex = genePool[Random.Range(0, genePool.Count)];

                        if(AIndex != BIndex)
                            break;
                    }
                }

                var child1 = new NeuralNetwork();
                var child2 = new NeuralNetwork();

                child1.Initialize(controller.NumOfSensors, controller.Layers, controller.Neurons);
                child2.Initialize(controller.NumOfSensors, controller.Layers, controller.Neurons);

                child1.fitness = 0;
                child2.fitness = 0;

                // Crossover weights
                for (int k = 0; k < child1.weights.Count; k++)
                {
                    bool randomizer = Random.value < 0.5f;

                    child1.weights[k] = population[randomizer ? AIndex : BIndex].weights[k];
                    child2.weights[k] = population[randomizer ? BIndex : AIndex].weights[k];
                }

                // Crossover biases
                for (int k = 0; k < child1.biases.Count; k++)
                {
                    bool randomizer = Random.value < 0.5f;

                    child1.biases[k] = population[randomizer ? AIndex : BIndex].biases[k];
                    child2.biases[k] = population[randomizer ? BIndex : AIndex].biases[k];
                }

                newPopulation[naturallySelected] = child1;
                naturallySelected++;

                newPopulation[naturallySelected] = child2;
                naturallySelected++;
            }
        }

        private void Mutate(NeuralNetwork[] newPopulation)
        {
            for (int i = 0; i < naturallySelected; i++)
            {
                for ( int c = 0; c < newPopulation[i].weights.Count; c++)
                {
                    if (Random.value < mutationRate)
                    {
                        newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                    }
                }
            }
        }

        private Matrix<float> MutateMatrix(Matrix<float> mat)
        {
            int randomPoints = Random.Range(1, (mat.RowCount * mat.ColumnCount) / 7);

            var c = mat;

            for (int i = 0; i < randomPoints; i++)
            {
                int randomColumn = Random.Range(0, c.ColumnCount);
                int randomRow = Random.Range(0, c.RowCount);

                c[randomRow, randomColumn] = Mathf.Clamp(c[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
            }

            return c;
        }

        private NeuralNetwork[] PickBestPopulation() {
            var newPopulation = new NeuralNetwork[initialPopulation];

            for (int i = 0; i < bestAgentSelection; i++) {
                newPopulation[naturallySelected] = population[i].InitializeCopy(controller.NumOfSensors, controller.Layers, controller.Neurons);
                newPopulation[naturallySelected].fitness = 0;
                naturallySelected++;

                int f = Mathf.RoundToInt(population[i].fitness * 10);

                for (int c = 0; c <= f; c++) {
                    genePool.Add(i);
                }
            }

            for (int i = 0; i < worstAgentSelection; i++) {
                int last = population.Length - 1;
                last -= i;

                int f = Mathf.RoundToInt(population[last].fitness * 10);

                for (int c = 0; c <= f; c++) {
                    genePool.Add(last);
                }
            }


            return newPopulation;
        }

        private void SortPopulation() 
        {
            for (int i = 0; i < population.Length; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {
                    if (population[i].fitness < population[j].fitness)
                    {
                        var temp = population[i];
                        population[i] = population[j];
                        population[j] = temp;
                    }
                }   
            }
        }
    }
}
