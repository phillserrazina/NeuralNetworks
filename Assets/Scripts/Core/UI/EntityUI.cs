using UnityEngine;
using UnityEngine.UI;
using Coursework.Core.Entities;

namespace Coursework.Core.UI
{
    public class EntityUI : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private Image[] bars = null;
        private EntityManager manager;

        // EXECUTION FUNCTIONS
        private void Awake() => manager = GetComponentInParent<EntityManager>();

        private void Update() {
            float[] percentages = manager.Stats.AllPercentages;

            for (int i = 0; i < percentages.Length; i++) {
                bars[i].fillAmount = percentages[i];
            }
        }
    }
}
