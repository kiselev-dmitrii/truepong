using System;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TruePong {
    public class PaddleTouchController : TrueSyncBehaviour, IDragHandler {
        private Paddle paddle;
        private FP _lastInput;

        public void Awake() {
            _lastInput = 0;
        }

        public void SetPaddle(Paddle paddle) {
            this.paddle = paddle;
            owner = paddle.owner;
        }

        public void OnDrag(PointerEventData eventData) {
            var currentPosition = eventData.position;
            var previousPosition = eventData.position - eventData.delta;

            var currentWorldPosition = Camera.main.ScreenToWorldPoint(currentPosition);
            var previousWorldPosition = Camera.main.ScreenToWorldPoint(previousPosition);

            var worldDelta = currentWorldPosition - previousWorldPosition;
            _lastInput = worldDelta.x;

        }

        public override void OnSyncedInput() {
            TrueSyncInput.SetFP((byte)InputType.Paddle, _lastInput);
            _lastInput = 0;
        }

        public override void OnSyncedUpdate() {
            var paddleDeltaOffset = TrueSyncInput.GetFP((byte)InputType.Paddle);
            paddle.SetOffset(paddle.Offset + paddleDeltaOffset);
        }
    }
}
