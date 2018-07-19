using System;
using System.Collections.Generic;
using TruePong.Defs;
using TruePong.GamePlay;
using TruePong.Utils;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public enum GameMode {
        Hotseat,
        Multiplayer
    }

    public interface IGameLauncher {
        void StartGame(Action<Scene> onSuccess, Action onFail);
        void LeaveGame();
    }

    public class LobbyController {
        public GameMode GameMode { get; private set; }

        private readonly GameDef gameDef;
        private readonly Dictionary<GameMode, IGameLauncher> launchers;
        private IGameLauncher currentLauncher;

        public LobbyController(GameDef gameDef) {
            this.gameDef = gameDef;
            launchers = new Dictionary<GameMode, IGameLauncher>() {
                {GameMode.Hotseat, new HotseatLauncher()},
                {GameMode.Multiplayer, MultiplayerLauncher.Create()}
            };
        }

        public void SetMode(GameMode mode) {
            currentLauncher = launchers[mode];
            GameMode = mode;
        }

        public void StartGame(Action onSuccess, Action onFailed) {
            currentLauncher.StartGame(
                (scene) => {
                    var initializator = scene.GetComponent<GameInitializator>();
                    initializator.Initialize(gameDef, this);
                },
                onFailed
            );
        }

        public void LeaveGame() {
            currentLauncher.LeaveGame();
        }
        
    }
}