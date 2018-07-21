using TruePong.Controllers;
using TruePong.Controllers.Lobby;
using TruePong.Defs;
using TruePong.UI;
using TrueSync.Physics3D;
using UnityEngine;

namespace TruePong {
    public class Application : MonoBehaviour {
        private static bool isInitialized;

        [SerializeField]
        private GameDef gameDef;

        protected void Awake() {
            if (isInitialized) {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Initialize();
            isInitialized = true;
        }

        private LobbyController lobbyController;

        private void Initialize() {
            lobbyController = new LobbyController(gameDef);
            lobbyController.OnGameStateChanged += OnGameStateChanged;
            OnGameStateChanged();
        }

        private void OnDestroy() {
            if (lobbyController != null) {
                lobbyController.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged() {
            if (lobbyController.GameState == GameState.Menu) {
                var mainScreen = new MainScreen(lobbyController);
                mainScreen.SetActive(true);
            }
        }
    }
}
