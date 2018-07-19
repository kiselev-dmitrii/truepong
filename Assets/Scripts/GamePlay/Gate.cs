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

        public int Score { get { return score; } }
        public Paddle Paddle { get; private set; }
        public GateSide Side { get { return side; } }

        public event Action OnScoreChanged;

        public void Awake() {
            goalTrigger.OnEnter += OnGoal;
        }

        protected void OnDestroy() {
            goalTrigger.OnEnter -= OnGoal;
        }

        public void SetPaddle(Paddle paddle) {
            Paddle = paddle;
            paddle.SetAnchor(paddleAnchor);
        }

        private void OnGoal(TSCollision2D obj) {
            ++score;
            OnScoreChanged.TryCall();
        }
    }
}