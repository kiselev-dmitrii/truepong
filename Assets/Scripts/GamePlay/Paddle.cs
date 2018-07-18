using TrueSync;
using UnityEngine;

namespace TruePong.GamePlay {
    public class Paddle : TrueSyncBehaviour {
        public FP Offset { get { return offset; } }
        public GameObject Hadle { get { return handle; } }

        [SerializeField]
        private TSRigidBody2D rigidBody;
        [SerializeField]
        private FP maxOffset;
        [SerializeField]
        private GameObject handle;

        private TSTransform2D anchor;

        [AddTracking]
        private FP offset;

        public void SetAnchor(TSTransform2D anchor) {
            this.anchor = anchor;

            tsTransform2D.tsParent = anchor;
            tsTransform2D.position = anchor.position;
            tsTransform2D.rotation = anchor.rotation;
            tsTransform2D.scale = TSVector.one;

            SetOffset(0);
        }

        public void SetOffset(FP value) {
            offset = TSMath.Clamp(value, -maxOffset, maxOffset);
            rigidBody.position = anchor.position + new TSVector2(offset, 0);
        }
    }
}
