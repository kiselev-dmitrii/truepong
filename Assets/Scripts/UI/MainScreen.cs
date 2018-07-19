using TruePong.Controllers;
using TruePong.Controllers.Lobby;
using TruePong.Utils;

namespace TruePong.UI {
    public class MainScreen : Window {
        private readonly LobbyController lobbyController;

        public MainScreen(LobbyController lobbyController) : base("UI/MainScreen") {
            this.lobbyController = lobbyController;
        }

        public void OnHotseatButtonClick() {
            lobbyController.SetMode(GameMode.Hotseat);
            lobbyController.StartGame(null, null);
        }

        public void OnMultiplayerButtonClick() {
            var window = new WaitWindow("Wait, please");
            window.SetActive(true);

            lobbyController.SetMode(GameMode.Multiplayer);
            lobbyController.StartGame(
                () => {
                    window.Destroy();
                },
                () => {
                    window.Destroy();
                }
            );
        }
    }
}