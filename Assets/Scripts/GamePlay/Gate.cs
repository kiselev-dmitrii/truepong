using System;
using TruePong.Utils;
using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public enum GateSide {
        Top,
        Bottom
    }

    public class Gate : TrueSyncBehaviour {
        [SerializeField]
        private TSTransform2D paddleAnchor;
        [SerializeField]
        private TriggerZone goalTrigger;
        [SerializeField]
        private GateSide side;

        [AddTracking]
        private int score;

        public TSTransform2D Anchor { get { return paddleAnchor; } }
        public int Score { get { return score; } }
        public event Action OnScoreChanged;

        public override void OnSyncedStart() {
            goalTrigger.OnEnter += OnGoal;
        }

        protected void OnDestroy() {
            goalTrigger.OnEnter -= OnGoal;
        }

        private void OnGoal(TSCollision2D obj) {
            ++score;
            OnScoreChanged.TryCall();
        }
    }
}