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


        public override void OnSyncedStart() {
            var freeGates = gates.ToList();

            var paddles = FindObjectsOfType<Paddle>();
            foreach (var paddle in paddles) {
                var gateIdx = paddle.owner.Id % gates.Length;
                var gate = gates[gateIdx];
                paddle.SetAnchor(gate.Anchor);

                var paddleController = paddle.Hadle.AddComponent<PaddleTouchController>();
                TrueSyncManager.RegisterITrueSyncBehaviour(paddleController);
                paddleController.Initialize(paddle, 0);

                freeGates.Remove(gate);
            }

            //TODO: переписать добавление юзеров
            if (freeGates.Count > 0) {
                var mgr = FindObjectOfType<TrueSyncManager>();
                var paddlePrefab = mgr.playerPrefabs[0];

                var spawnedGo = TrueSyncManager.SyncedInstantiate(paddlePrefab);
                var freeGate = freeGates.First();
                var paddle = spawnedGo.GetComponent<Paddle>();
                paddle.SetAnchor(freeGate.Anchor);

                var paddleController = paddle.Hadle.AddComponent<PaddleTouchController>();
                TrueSyncManager.RegisterITrueSyncBehaviour(paddleController);
                paddleController.Initialize(paddle, 1);
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
