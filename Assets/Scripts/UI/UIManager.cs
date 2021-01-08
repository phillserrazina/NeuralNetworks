using UnityEngine;
using UnityEngine.UI;
using Coursework.Core;

namespace Coursework.UI
{
    public class UIManager : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private Text generationText = null;
        [SerializeField] private Slider speedSlider = null;

        // EXECUTION FUNCTIONS
        private void Update() {
            generationText.text = $"Current Generation: { GeneticManager.Instance.currentGeneration }\n";
            generationText.text += $"Current Genome: { GeneticManager.Instance.currentGenome }";

            Time.timeScale = speedSlider.value;
        }
    }
}
