using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class Ball : TrueSyncBehaviour {
        [SerializeField]
        private TSRigidBody2D rigidBody;

        [AddTracking]
        private TSVector2 direction;

        public void Reset(TSVector2 position) {
            tsTransform2D.position = position;
            rigidBody.position = position;

            direction = GenerateRandomDirection();
        }

        public void OnSyncedCollisionEnter(TSCollision2D other) {
            var contact = other.contacts[0];
            direction = TSVector2.Reflect(direction, contact.normal);
        }

        public override void OnSyncedUpdate() {
            rigidBody.position += direction * TrueSyncManager.DeltaTime;
        }

        private static TSVector2 GenerateRandomDirection() {
            var angle = TSRandom.Range(0.0f, 360.0f);

            TSVector2 result;
            result.x = TSMath.Cos(angle);
            result.y = TSMath.Sin(angle);
            return result;
        }
    }
}
