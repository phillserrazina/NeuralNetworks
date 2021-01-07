using UnityEngine;
using System.Collections.Generic;
using Coursework.Core.Entities.Controllers;

namespace Coursework.Core.Entities
{
    public class EntityManager : MonoBehaviour
    {
        // VARIABLES
        public EntityStats Stats { get; private set; }
        public EntityMovement Movement { get; private set; }
        public EntityController Controller { get; private set; }

        private GeneticManager geneticManager;

        public List<EntityManager> EntitiesInRange;

        // METHODS
        public void Initialize(Tile startPos) {
            Stats = GetComponent<EntityStats>();
            Movement = GetComponent<EntityMovement>();
            Controller = GetComponent<EntityController>();

            Stats.Initialize(this, Random.Range(1,3), Random.Range(0f, 4f));
            Movement.Initialize(this, startPos);
            Controller.Initialize(this, startPos);

            GetComponentInChildren<EntityDetector>().Initialize(this, Stats.Genes.SensoryDistance);
        }

        public void Kill() {
            Controller.Kill();
        }

        public bool RequestMate(EntityManager male) {
            if (Random.value > male.Stats.Genes.Desirability) {
                return false;
            }

            ((EvolutionController)(Controller)).SetMatingTarget(male);
            return true;
        }
    }
}
