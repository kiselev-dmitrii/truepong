using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class Game : TrueSyncBehaviour {
        [SerializeField]
        private Gate[] gates;
        [SerializeField]
        private TSTransform2D ballSpawnPoint;
        [SerializeField]
        private Ball ballPrefab;

        private Ball ball;

        public override void OnSyncedStart() {
            var paddles = FindObjectsOfType<Paddle>();
            foreach (var paddle in paddles) {
                var gateIdx = paddle.owner.Id % gates.Length;
                var gate = gates[gateIdx];
                paddle.SetAnchor(gate.Anchor);

                var paddleController = paddle.Hadle.AddComponent<PaddleTouchController>();
                TrueSyncManager.RegisterITrueSyncBehaviour(paddleController);
                paddleController.SetPaddle(paddle);
            }

            GameObject go = TrueSyncManager.SyncedInstantiate(ballPrefab.gameObject, ballSpawnPoint.position, TSQuaternion.identity);
            ball = go.GetComponent<Ball>();
            ball.Reset(ballSpawnPoint.position);

            foreach (var gate in gates) {
                gate.OnScoreChanged += OnScoreChanged;
            }
        }

        private void OnScoreChanged() {
            ball.Reset(ballSpawnPoint.position);
        }
    }
}
