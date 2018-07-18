using TruePong.Controllers;
using TruePong.Defs;
using TruePong.UI;
using UnityEngine;

namespace TruePong {
    public class Application : MonoBehaviour {
        [SerializeField]
        private GameDef gameDef;

        protected void Awake() {
            var lobbyController = LobbyController.Create();
            var mainScreen = new MainScreen(lobbyController);
            mainScreen.SetActive(true);
        }
    }
}
