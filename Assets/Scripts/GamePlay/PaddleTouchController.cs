using TrueSync;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TruePong.GamePlay {
    public class PaddleTouchController : TrueSyncBehaviour, IDragHandler {
        private Paddle paddle;
        private FP lastInput;
        private byte inputIndex;

        protected void Awake() {
            lastInput = 0;
        }

        public void Initialize(Paddle paddle, byte inputIndex) {
            this.paddle = paddle;
            this.inputIndex = inputIndex;
            owner = paddle.owner;
        }

        public void OnDrag(PointerEventData eventData) {
            var currentPosition = eventData.position;
            var previousPosition = eventData.position - eventData.delta;

            var currentWorldPosition = Camera.main.ScreenToWorldPoint(currentPosition);
            var previousWorldPosition = Camera.main.ScreenToWorldPoint(previousPosition);

            var worldDelta = currentWorldPosition - previousWorldPosition;
            lastInput = worldDelta.x;

        }

        public override void OnSyncedInput() {
            TrueSyncInput.SetFP(inputIndex, lastInput);
            lastInput = 0;
        }

        public override void OnSyncedUpdate() {
            var paddleDeltaOffset = TrueSyncInput.GetFP(inputIndex);
            paddle.SetOffset(paddle.Offset + paddleDeltaOffset);
        }
    }
}
