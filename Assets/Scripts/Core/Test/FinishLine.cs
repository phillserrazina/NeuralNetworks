using UnityEngine;

namespace Test
{
    public class FinishLine : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private int requiredLaps = 2;
        [SerializeField] private GameObject track = null;
        [SerializeField] private GameObject realWorld = null;

        // EXECUTION FUNCTIONS
        private void OnTriggerEnter(Collider other) {
            var controller = other.GetComponent<TestController>();
            if (controller == null) return;

            controller.FinishLap();

            if (controller.LapsDone >= requiredLaps)
            {
                ActivateRealWorld();
            }
        }

        // METHODS
        private void ActivateRealWorld() 
        {
            track.SetActive(false);
            realWorld.SetActive(true);
        }
    }
}
