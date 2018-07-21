using System;
using System.Collections.Generic;
using TruePong.Defs;
using TruePong.GamePlay;
using TruePong.Utils;


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
        GameState State { get; }
        event Action OnStateChanged;

        void StartGame();
        void LeaveGame();
    }

    public class LobbyController {
        public GameMode GameMode { get; private set; }
        public GameState GameState { get { return currentLauncher.State; } }
        public event Action OnGameStateChanged;

        private const String lobbyScene = "Scenes/Lobby";
        private const String gameScene = "Scenes/Game";

        private readonly GameDef gameDef;
        private readonly Dictionary<GameMode, IGameLauncher> launchers;
        private IGameLauncher currentLauncher;

        public LobbyController(GameDef gameDef) {
            this.gameDef = gameDef;
            launchers = new Dictionary<GameMode, IGameLauncher>() {
                {GameMode.Hotseat, new HotseatLauncher(lobbyScene, gameScene)},
                {GameMode.Multiplayer, MultiplayerLauncher.Create(lobbyScene, gameScene, 2)}
            };

            SetMode(GameMode.Hotseat);
        }

        public void SetMode(GameMode mode) {
            if (currentLauncher != null) {
                if (currentLauncher.State != GameState.Menu) {
                    throw new InvalidOperationException("You cannot change GameMode in state:" + currentLauncher.State);
                }
                currentLauncher.OnStateChanged -= OnStateChanged;
            }

            GameMode = mode;
            currentLauncher = launchers[GameMode];

            if (currentLauncher != null) {
                currentLauncher.OnStateChanged += OnStateChanged;
            }
        }

        public void StartGame() {
            currentLauncher.StartGame();
        }

        public void LeaveGame() {
            currentLauncher.LeaveGame();
        }

        private void OnStateChanged() {
            if (GameState == GameState.InGame) {
                var scene = SceneLoader.GetActiveScene();
                var initializtor = scene.GetComponent<GameInitializator>();
                initializtor.Initialize(gameDef, this);
            }

            OnGameStateChanged.TryCall();
        }
    }
}