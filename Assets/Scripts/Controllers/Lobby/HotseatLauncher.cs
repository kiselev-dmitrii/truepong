using System;
using TruePong.Utils;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public class HotseatLauncher : IGameLauncher {
        public void StartGame(Action<Scene> onSuccess, Action onFailed) {
            PhotonNetwork.offlineMode = true;
            SceneLoader.LoadScene("Scenes/Game", onSuccess);
        }

        public void LeaveGame(Action<Scene> onSuccess) {
            SceneLoader.LoadScene("Scenes/Lobby", onSuccess);
        }
    }
}
