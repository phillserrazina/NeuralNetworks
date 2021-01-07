using UnityEngine;
using Coursework.Core.Entities;
using System.Collections.Generic;

namespace Coursework.Core
{
    public class PopulationManager : MonoBehaviour
    {
        // VARIABLES
        public static PopulationManager Instance { get; private set; }

        [SerializeField] private List<EntityManager> entities = new List<EntityManager>();

        // EXECUTION FUNCTIONS
        private void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        // METHODS
        public void CreateEntity(EntityManager entity, Tile startPos) {
            var bunny = Instantiate(entity);
            bunny.Initialize(startPos);

            entities.Add(bunny);
            bunny.gameObject.name = "Bunny " + entities.Count;
        }
    }
}
