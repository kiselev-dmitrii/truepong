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

    public enum GameState {
        Menu,
        Starting,
        InGame,
        Leaving

    }

    public interface IGameLauncher {
        void StartGame(Action<Scene> onSuccess, Action onFail);
        void LeaveGame(Action<Scene> onSuccess);
    }

    public class LobbyController {
        public GameMode GameMode { get; private set; }

        public GameState GameState { get; private set; }
        public event Action OnGameStateChanged;

        private readonly GameDef gameDef;
        private readonly Dictionary<GameMode, IGameLauncher> launchers;
        private IGameLauncher currentLauncher;

        public LobbyController(GameDef gameDef) {
            this.gameDef = gameDef;
            launchers = new Dictionary<GameMode, IGameLauncher>() {
                {GameMode.Hotseat, new HotseatLauncher()},
                {GameMode.Multiplayer, MultiplayerLauncher.Create()}
            };

            SetGameState(GameState.Menu, false);
            SetMode(GameMode.Hotseat);
        }

        public void SetMode(GameMode mode) {
            currentLauncher = launchers[mode];
            GameMode = mode;
        }

        public void StartGame(Action onSuccess, Action onFailed) {
            SetGameState(GameState.Starting);
            currentLauncher.StartGame(
                (scene) => {
                    var initializator = scene.GetComponent<GameInitializator>();
                    initializator.Initialize(gameDef, this);
                    SetGameState(GameState.InGame);
                    onSuccess.TryCall();
                },
                () => {
                    SetGameState(GameState.Menu);
                    onFailed.TryCall();
                }
            );
        }

        public void LeaveGame(Action onSuccess) {
            SetGameState(GameState.Leaving);
            currentLauncher.LeaveGame((scene) => {
                SetGameState(GameState.Menu);
                onSuccess.TryCall();
            });
        }

        private void SetGameState(GameState gameState, bool raiseEvent = true) {
            GameState = gameState;
            if (raiseEvent) OnGameStateChanged.TryCall();
        }
    }
}