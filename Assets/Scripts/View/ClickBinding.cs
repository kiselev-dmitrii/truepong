﻿using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TruePong.View {
    public class ClickBinding : CommandBinding, IPointerClickHandler {
        private Button button;
        
        public override void Awake() {
            button = GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (button != null && !button.interactable) return;
            if (_command == null) return;

            _command.DynamicInvoke();
        }
    }
}
