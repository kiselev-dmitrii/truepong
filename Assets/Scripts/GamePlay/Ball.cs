using TruePong.Defs;
using TruePong.Defs.DirectionGenerator;
using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class Ball : TrueSyncBehaviour {
        [SerializeField]
        private TSRigidBody2D rigidBody;
        [SerializeField]
        private TSBoxCollider2D boxCollider;
        [SerializeField]
        private BallRenderer renderer;

        [AddTracking]
        private TSVector2 direction;
        [AddTracking]
        private FP speed = 1;

        private BaseDirectionGenerator directionGenerator;

        public void Reset(TSVector2 position) {
            tsTransform2D.position = position;
            rigidBody.position = position;
            direction = directionGenerator.GenerateDirection();
        }

        public void Configure(BallDef ballDef) {
            speed = ballDef.Speed;
            boxCollider.size = TSVector2.one * ballDef.Size;
            renderer.SetColor(ballDef.Color);
            renderer.SetSize(ballDef.Size.AsFloat());
            directionGenerator = ballDef.DirectionGenerator;
        }

        public void OnSyncedCollisionEnter(TSCollision2D other) {
            var contact = other.contacts[0];
            direction = TSVector2.Reflect(direction, contact.normal);
        }

        public override void OnSyncedUpdate() {
            rigidBody.position += direction * TrueSyncManager.DeltaTime * speed;
        }
    }
}
