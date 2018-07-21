using System;
using EZData;
using TruePong.Controllers;
using TruePong.Controllers.Lobby;
using TruePong.Utils;

namespace TruePong.UI {
    public class MainScreen : Window, IDisposable {
        private readonly LobbyController lobbyController;

        #region Property IsLoading
        private readonly Property<bool> _privateIsLoadingProperty = new Property<bool>();
        public Property<bool> IsLoadingProperty { get { return _privateIsLoadingProperty; } }
        public bool IsLoading {
            get { return IsLoadingProperty.GetValue(); }
            set { IsLoadingProperty.SetValue(value); }
        }
        #endregion

        private CommonDialog waitWindow;

        public MainScreen(LobbyController lobbyController) : base("UI/MainScreen") {
            this.lobbyController = lobbyController;
            lobbyController.OnGameStateChanged += OnGameStateChanged;
        }

        public void Dispose() {
            lobbyController.OnGameStateChanged -= OnGameStateChanged;
        }

        public void OnHotseatButtonClick() {
            lobbyController.SetMode(GameMode.Hotseat);
            lobbyController.StartGame();
        }

        public void OnMultiplayerButtonClick() {
            lobbyController.SetMode(GameMode.Multiplayer);
            lobbyController.StartGame();
        }

        private void OnGameStateChanged() {
            if (lobbyController.GameMode == GameMode.Hotseat) return;

            if (lobbyController.GameState == GameState.Starting) {
                waitWindow = new CommonDialog("WAITING FOR PLAYERS", "PLEASE WAIT OTHER PLAYERS");
                waitWindow.AddButton("CANCEL", () => {
                    lobbyController.LeaveGame();
                });
                waitWindow.SetActive(true);
            } else {
                if (waitWindow != null) {
                    waitWindow.Destroy();
                    waitWindow = null;
                }
            }
        }
    }
}