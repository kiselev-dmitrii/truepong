using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class PaddleKeyboardController : TrueSyncBehaviour{
        private Paddle paddle;

        public void SetPaddle(Paddle paddle) {
            this.paddle = paddle;
            owner = paddle.owner;
        }

        public override void OnSyncedInput() {
            FP input = 0;

            if (Input.GetKey(KeyCode.LeftArrow)) {
                input = -0.1;
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                input = 0.1;
            }

            TrueSyncInput.SetFP((byte)InputType.Paddle, input);
        }

        public override void OnSyncedUpdate() {
            var paddleDeltaOffset = TrueSyncInput.GetFP((byte)InputType.Paddle);
            paddle.SetOffset(paddle.Offset + paddleDeltaOffset);
        }
    }
}
