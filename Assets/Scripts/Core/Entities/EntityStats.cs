using UnityEngine;
using System.Collections.Generic;
using Coursework.Core.Entities.Stats;

namespace Coursework.Core.Entities
{
    public class EntityStats : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float hungerRate = 1f;

        [Space(10)]
        [SerializeField] private float maxThirst = 50f;
        [SerializeField] private float thirstRate = 1f;

        [Space(10)]
        [SerializeField] private float maxMating = 150f;

        private Dictionary<string, Stat> statsDict;

        private EntityManager manager;
        public EntityGenes Genes { get; private set; }

        public string Gender { get; private set; }

        public string CurrentNecessity {
            get {
                string currentHighest = "";
                float currentHighestValue = -1f;

                foreach (var stat in statsDict.Keys) {
                    if (statsDict[stat].Percentage > currentHighestValue) {
                        currentHighest = stat;
                        currentHighestValue = statsDict[stat].Percentage;
                    }
                }

                return currentHighest;
            }
        }

        // EXECUTION FUNCTIONS
        private void Update() {
            foreach (var stat in statsDict.Keys) {
                bool dead = statsDict[stat].OnUpdate();
                if (dead) manager.Kill();
            }
        }

        // METHODS
        public void Initialize(EntityManager manager, int sensDist, float reprodUrge) {
            this.manager = manager;
            
            Genes = new EntityGenes(sensDist, reprodUrge);
            Gender = Random.value > 0.5f ? "Male" : "Female";

            GetComponentInChildren<Renderer>().material.color = Gender == "Male" ? Color.yellow : Color.white;

            statsDict = new Dictionary<string, Stat> {
                { "Hunger", new Stat(maxHunger, hungerRate) },
                { "Thirst", new Stat(maxThirst, thirstRate) },
                { "Mating", new Mating(maxMating, reprodUrge) }
            };
        }

        public void Restore(string stat, float val=-1) => statsDict[stat].Restore(val);
        public float Percentage(string stat) => statsDict[stat].Percentage;

        public float[] AllPercentages {
            get {
                var list = new List<float>();

                foreach (var stat in statsDict.Keys) {
                    list.Add(statsDict[stat].Percentage);
                }

                return list.ToArray();
            }
        }

        public void Reset() {
            foreach (var stat in statsDict.Keys) {
                statsDict[stat].OnReset();
            }
        }
    }
}
