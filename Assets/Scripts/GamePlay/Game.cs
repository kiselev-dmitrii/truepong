using System;
using System.Linq;
using TruePong.Defs;
using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class Game : TrueSyncBehaviour, IDisposable {
        [SerializeField]
        private Gate[] gates;
        [SerializeField]
        private TSTransform2D ballSpawnPoint;

        private GameDef gameDef;
        private Ball ball;
        [AddTracking]
        private int ballIdx;

        public Gate[] Gates { get { return gates; } }


        public void Initialize(GameDef gameDef) {
            this.gameDef = gameDef;
            foreach (var gate in gates) {
                gate.OnScoreChanged += OnScoreChanged;
            }
        }

        public void Dispose() {
            foreach (var gate in gates) {
                gate.OnScoreChanged -= OnScoreChanged;
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

        public void AddBall(Ball ball) {
            if (this.ball != null) {
                throw new InvalidOperationException("Game already has ball");
            }
            this.ball = ball;
        }

        public void Restart() {
            ball.Reset(ballSpawnPoint.position);

            ballIdx = TSRandom.Range(0, gameDef.Balls.Length);
            ball.Configure(gameDef.Balls[ballIdx]);
        }

        private void OnScoreChanged() {
            Restart();
        }
    }
}
