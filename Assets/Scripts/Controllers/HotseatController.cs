using TrueSync;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers {
    public class HotseatController {
        public void StartGame() {
            SceneManager.LoadScene("Scenes/Game");
            var scene = SceneManager.GetActiveScene();
        }
    }
}
