using UnityEngine;
using UnityEngine.UI;

namespace TruePong.View {
    [RequireComponent(typeof(Button))]
    public class InteractableBinding : BooleanBinding {
        private Button button;

        public override void Awake() {
            button = GetComponent<Button>();
        }

        protected override void ApplyNewValue(bool newValue) {
            button.interactable = newValue;
        }
    }
}
