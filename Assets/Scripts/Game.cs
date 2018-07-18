using TrueSync;
using UnityEngine;

namespace TruePong {
    public class Game : TrueSyncBehaviour {
        [SerializeField]
        private TSTransform2D[] spawnPoints;
        [SerializeField]
        private TSTransform2D ballSpawnPoint;
        [SerializeField]
        private Ball ballPrefab;

        public override void OnSyncedStart() {
            var paddles = FindObjectsOfType<Paddle>();
            foreach (var paddle in paddles) {
                var spawnPointIdx = paddle.owner.Id % spawnPoints.Length;
                var spawnPoint = spawnPoints[spawnPointIdx];
                paddle.SetAnchor(spawnPoint);
            }

            GameObject go = TrueSyncManager.SyncedInstantiate(ballPrefab.gameObject, ballSpawnPoint.position, TSQuaternion.identity);
            var ball = go.GetComponent<Ball>();
            ball.Reset(ballSpawnPoint.position);
        }
    }
}
