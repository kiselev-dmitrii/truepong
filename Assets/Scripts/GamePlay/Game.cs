using System;
using System.Linq;
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

        public void Initialize() {
            GameObject go = TrueSyncManager.SyncedInstantiate(ballPrefab.gameObject, ballSpawnPoint.position, TSQuaternion.identity);
            ball = go.GetComponent<Ball>();

            foreach (var gate in gates) {
                gate.OnScoreChanged += OnScoreChanged;
            }
        }

        public void AddPlayer(Paddle paddle, byte inputIdx) {
            var firstFreeGate = gates.FirstOrDefault(x => x.Paddle == null);
            if (firstFreeGate == null) {
                throw new InvalidOperationException("There is no free gates");
            }

            var paddleController = paddle.Hadle.AddComponent<PaddleTouchController>();
            TrueSyncManager.RegisterITrueSyncBehaviour(paddleController);
            paddleController.Initialize(paddle, inputIdx);

            firstFreeGate.SetPaddle(paddle);
        }

        public void Restart() {
            ball.Reset(ballSpawnPoint.position);
        }

        private void OnScoreChanged() {
            Restart();
        }
    }
}
