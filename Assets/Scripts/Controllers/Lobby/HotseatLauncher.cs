using System;
using TruePong.Utils;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public class HotseatLauncher : IGameLauncher {
        private GameState state;

        public event Action OnStateChanged;
        public GameState State {
            get { return state; }
            set {
                state = value;
                OnStateChanged.TryCall();
            }
        }

        public HotseatLauncher() {
            state = GameState.Menu;

        }

        public void StartGame() {
            if (State != GameState.Menu) {
                throw new InvalidOperationException();
            }
            State = GameState.Starting;

            PhotonNetwork.offlineMode = true;
            SceneLoader.LoadScene("Scenes/Game", (scene) => {
                State = GameState.InGame;
            });
        }

        public void LeaveGame() {
            if (State != GameState.InGame) {
                throw new InvalidOperationException();
            }

            State = GameState.Leaving;
            SceneLoader.LoadScene("Scenes/Lobby", (scene) => {
                State = GameState.Menu;
            });
        }
    }
}
