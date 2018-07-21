using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

namespace TruePong.GamePlay {
    [ExecuteInEditMode]
    public class SpriteColor : MonoBehaviour {
        public SpriteRenderer[] ColorGlows;

#if UNITY_EDITOR
        [SerializeField]
        private Color color;
        public void Update() {
            if (UnityEngine.Application.isPlaying) return;
            SetColor(color);
        }
#endif

        public void SetColor(Color color) {
            foreach (var glow in ColorGlows) {
                if (glow == null) continue;

                var c = color;
                c.a = glow.color.a;
                glow.color = c;
            }
        }
    }
}
