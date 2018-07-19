using System;
using TruePong.Utils;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public class HotseatLauncher : IGameLauncher {
        private Action<Scene> onSuccess;

        public void StartGame(Action<Scene> onSuccess, Action onFailed) {
            PhotonNetwork.offlineMode = true;
            SceneManager.LoadScene("Scenes/Game");
            SceneManager.sceneLoaded += OnSceneLoaded;
            this.onSuccess = onSuccess;
        }

        public void LeaveGame() {
            SceneManager.LoadScene("Scenes/Lobby");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            onSuccess.TryCall(SceneManager.GetActiveScene());
        }
    }
}
