using TruePong.Controllers;
using TruePong.Utils;

namespace TruePong.UI {
    public class MainScreen : Window {
        private readonly LobbyController lobbyController;
        private readonly HotseatController hotseatController;

        public MainScreen(LobbyController lobbyController, HotseatController hotseatController) : base("UI/MainScreen") {
            this.lobbyController = lobbyController;
            this.hotseatController = hotseatController;
        }

        public void OnHotseatButtonClick() {
            hotseatController.StartGame();
        }

        public void OnMultiplayerButtonClick() {
            lobbyController.StartGame();
        }
    }
}