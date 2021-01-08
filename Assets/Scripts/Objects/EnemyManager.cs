using UnityEngine;

namespace Coursework.Objects
{
    public class EnemyManager : MonoBehaviour
    {
        // VARIABLES
        [Header("Parameters")]
        [SerializeField] private Transform target = null;
        [SerializeField] private float speed = 5f;

        private Enemy[] enemyList;
        public static EnemyManager Instance { get; private set; }

        // EXECUTION FUNCTIONS
        private void Awake() {
            Instance = this;

            enemyList = GetComponentsInChildren<Enemy>();

            foreach (var enemy in enemyList) {
                enemy.Initialize(target, speed);
            }
        }

        // METHODS
        public void Restart() {
            foreach (var enemy in enemyList) {
                enemy.Restart();
            }
        }
    }
}
