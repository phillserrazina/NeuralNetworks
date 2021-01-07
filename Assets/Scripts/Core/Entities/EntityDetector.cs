using UnityEngine;

namespace Coursework.Core.Entities
{
    public class EntityDetector : MonoBehaviour
    {
        // VARIABLES
        private EntityManager manager;

        // EXECUTION FUNCTIONS

        private void OnTriggerEnter(Collider other) {
            var entity = other.GetComponent<EntityManager>();
            if (entity == null) return;
            else if (entity == manager) return;

            manager.EntitiesInRange.Add(entity);
        }

        private void OnTriggerExit(Collider other) {
            var entity = other.GetComponent<EntityManager>();
            if (entity == null) return;
            else if (entity == manager) return;

            if (manager.EntitiesInRange.Contains(entity))
                manager.EntitiesInRange.Remove(entity);
        }

        // METHODS
        public void Initialize(EntityManager manager, float radius) {
            this.manager = manager;
            GetComponent<SphereCollider>().radius *= radius;
        }
    }
}
