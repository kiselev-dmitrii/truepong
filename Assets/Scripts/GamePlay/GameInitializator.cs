using System.Linq;
using TruePong.Controllers.Lobby;
using TruePong.Defs;
using TruePong.UI;
using TrueSync;
using UnityEngine;
using Object = System.Object;

namespace TruePong.GamePlay {
    public class GameInitializator : TrueSyncBehaviour {
        [SerializeField]
        private Game game;
        [SerializeField]
        private Camera camera;

        [SerializeField]
        private Paddle paddlePrefab;
        [SerializeField]
        private Ball ballPrefab;

        private GameDef gameDef;
        private LobbyController lobbyController;
        private GameScreen gameScreen;

        public void Initialize(GameDef gameDef, LobbyController lobbyController) {
            this.gameDef = gameDef;
            this.lobbyController = lobbyController;
        }

        public override void OnSyncedStart() {
            game.Initialize(gameDef);

            if (PhotonNetwork.offlineMode) {
                var paddle1 = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                paddle1.owner = TrueSyncManager.LocalPlayer;
                paddle1.SetColor(gameDef.PlayerColor);
                game.AddPlayer(paddle1, 0);

                var paddle2 = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                paddle2.owner = TrueSyncManager.LocalPlayer;
                paddle2.SetColor(gameDef.EnemyColor);
                game.AddPlayer(paddle2, 1);

            } else {
                foreach (var player in TrueSyncManager.Players) {
                    var paddle = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                    paddle.owner = player;
                    var color = player == TrueSyncManager.LocalPlayer ? gameDef.PlayerColor : gameDef.EnemyColor;
                    paddle.SetColor(color);
                    game.AddPlayer(paddle, 0);
                }
            }

            var playerGate = game.Gates.First(x => x.Paddle.owner == TrueSyncManager.LocalPlayer);
            if (playerGate.Side == GateSide.Top) {
                camera.transform.localEulerAngles = new Vector3(0, 0, 180);
            }

            var ball = TrueSyncManager.SyncedInstantiate(ballPrefab.gameObject, TSVector.zero, TSQuaternion.identity).GetComponent<Ball>();
            game.AddBall(ball);

            game.Restart();

            gameScreen = new GameScreen(lobbyController, game);
            gameScreen.SetActive(true);
        }

        public override void OnSyncedUpdate() {
            gameScreen.Update();
        }

        public void OnDestroy() {
            game.Dispose();
        }
    }
}
