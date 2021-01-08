using UnityEngine;

namespace Coursework.Utils
{
    public class CameraManager : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private Camera subjectCamera = null;
        [SerializeField] private Camera birdCamera = null;

        // METHODS
        public void SwitchCamera() {
            subjectCamera.gameObject.SetActive(!subjectCamera.gameObject.activeInHierarchy);
            birdCamera.gameObject.SetActive(!birdCamera.gameObject.activeInHierarchy);
        }
    }
}
