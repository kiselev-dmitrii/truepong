using TruePong.Controllers;
using TruePong.Utils;

namespace TruePong.UI {
    public class MainScreen : Window {
        private readonly LobbyController lobbyController;

        public MainScreen(LobbyController lobbyController) : base("UI/MainScreen") {
            this.lobbyController = lobbyController;
        }

        public void OnHotseatButtonClick() {

        }

        public void OnMultiplayerButtonClick() {
            lobbyController.StartGame();
        }
    }
}