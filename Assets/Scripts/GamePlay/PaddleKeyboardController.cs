using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class PaddleKeyboardController : TrueSyncBehaviour{
        private Paddle paddle;
        private byte inputIndex;

        public void Initialize(Paddle paddle, byte inputIndex) {
            this.paddle = paddle;
            this.inputIndex = inputIndex;
            owner = paddle.owner;
        }

        public override void OnSyncedInput() {
            FP input = 0;

            if (Input.GetKey(KeyCode.LeftArrow)) {
                input = -0.1;
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                input = 0.1;
            }

            TrueSyncInput.SetFP(inputIndex, input);
        }

        public override void OnSyncedUpdate() {
            var paddleDeltaOffset = TrueSyncInput.GetFP(inputIndex);
            paddle.SetOffset(paddle.Offset + paddleDeltaOffset);
        }
    }
}
