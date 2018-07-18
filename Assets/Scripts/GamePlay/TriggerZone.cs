using System;
using TruePong.Utils;
using TrueSync;

namespace TruePong.GamePlay {
    public class TriggerZone : TrueSyncBehaviour {
        public event Action<TSCollision2D> OnEnter;
        public event Action<TSCollision2D> OnExit;

        public void OnSyncedTriggerEnter(TSCollision2D other) {
            OnEnter.TryCall(other);
        }

        public void OnSyncedTriggerExit(TSCollision2D other) {
            OnExit.TryCall(other);
        }
    }
}
