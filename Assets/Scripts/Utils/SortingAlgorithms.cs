using UnityEngine;
using Coursework.Core;

namespace Coursework.Utils
{
    public class SortingAlgorithms : MonoBehaviour
    {
        public static void BubbleSort(ref NeuralNetwork[] arr) {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = i; j < arr.Length; j++)
                {
                    if (arr[i].fitness < arr[j].fitness)
                    {
                        NeuralNetwork temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }
            }
        }
    }
}
