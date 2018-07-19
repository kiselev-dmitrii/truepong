using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class GameLauncher : TrueSyncBehaviour {
        [SerializeField]
        private Game game;
        [SerializeField]
        private Paddle paddlePrefab;

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

            game.Restart();
        }
    }
}
