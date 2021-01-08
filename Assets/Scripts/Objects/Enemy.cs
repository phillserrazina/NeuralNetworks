using UnityEngine;

namespace Coursework.Objects
{
    public class Enemy : MonoBehaviour
    {
        // VARIABLES
        private Transform target;
        private float speed;

        private Vector3 initialPosition;

        // EXECUTION FUNCTIONS
        private void FixedUpdate() {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.fixedDeltaTime * speed);
        }

        // METHODS
        public void Initialize(Transform target, float speed) {
            this.target = target;
            this.speed = speed;

            initialPosition = transform.position;
        }

        public void Restart() {
            transform.position = initialPosition;
        }
    }
}
