using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coursework.Utils {
    public class SceneChanger : MonoBehaviour {
        public void GoToScene(string sceneName) => SceneManager.LoadScene(sceneName);
    }
}
