using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coursework.Core
{
    public class GameManager : MonoBehaviour
    {
        // VARIABLES
        public static GameManager Instance { get; private set; }

        private GameInitializer initializer;

        // EXECUTION FUNCTIONS
        private void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start() {
            TerrainManager.Instance.Initialize();
            
            initializer = GetComponent<GameInitializer>();
            initializer.Initialize();            
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.R)) {
                ResetScene();
            }
        }

        public void ResetScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
