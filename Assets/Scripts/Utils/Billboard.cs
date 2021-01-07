using UnityEngine;

namespace Lucerna.UI.Utils
{
    public class Billboard : MonoBehaviour
    {
        // VARIABLES
        private new Camera camera;

        // EXECUTION FUNCTIONS
        private void Awake() => camera = Camera.main;
        private void Update() => transform.LookAt(camera.transform);
    }
}
