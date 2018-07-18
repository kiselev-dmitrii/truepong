using UnityEngine;
using UnityEngine.UI;

namespace TruePong.View {
    public class BoolColorBinding : BooleanBinding {
        public Color TrueColor;
        public Color FalseColor;
        private Image image;

        public override void Awake() {
            image = GetComponent<Image>();
        }

        protected override void ApplyNewValue(bool newValue) {
            image.color = newValue ? TrueColor : FalseColor;
        }
    }
}
