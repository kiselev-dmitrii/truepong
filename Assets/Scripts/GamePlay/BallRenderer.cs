using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

namespace TruePong.GamePlay {
    public class BallRenderer : MonoBehaviour {
        [SerializeField]
        private Transform spritesTransform;
        [SerializeField]
        private SpriteRenderer[] spriteRenderers;
        [SerializeField]
        private TrailRenderer trailRenderer;

        public void SetColor(Color color) {
            foreach (var spriteRenderer in spriteRenderers) {
                var c = color;
                c.a = spriteRenderer.color.a;
                spriteRenderer.color = c;
            }

            trailRenderer.colorGradient = new Gradient() {
                alphaKeys = new[] {
                    new GradientAlphaKey(0.5f, 0),
                    new GradientAlphaKey(0, 1),
                },
                colorKeys = new[] {
                    new GradientColorKey(color, 0),
                    new GradientColorKey(color, 1),
                }
            };
        }

        public void SetSize(float size) {
            spritesTransform.localScale = Vector3.one * size;
            trailRenderer.widthMultiplier = size;
        }
    }
}