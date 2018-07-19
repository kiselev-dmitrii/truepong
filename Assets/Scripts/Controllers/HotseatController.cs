using TrueSync;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers {
    public class HotseatController {
        public void StartGame() {
            PhotonNetwork.offlineMode = true;
            SceneManager.LoadScene("Scenes/Game");
        }
    }
}
