using System.Linq;
using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class GameLauncher : TrueSyncBehaviour {
        [SerializeField]
        private Game game;
        [SerializeField]
        private Camera camera;

        [SerializeField]
        private Paddle paddlePrefab;
        [SerializeField]
        private Ball ballPrefab;

        public override void OnSyncedStart() {
            game.Initialize();

            if (PhotonNetwork.offlineMode) {
                var paddle1 = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                paddle1.owner = TrueSyncManager.LocalPlayer;
                game.AddPlayer(paddle1, 0);

                var paddle2 = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                paddle2.owner = TrueSyncManager.LocalPlayer;
                game.AddPlayer(paddle2, 1);

            } else {
                foreach (var player in TrueSyncManager.Players) {
                    var paddle = TrueSyncManager.SyncedInstantiate(paddlePrefab.gameObject).GetComponent<Paddle>();
                    paddle.owner = player;
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
        }
    }
}
