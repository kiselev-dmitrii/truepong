using UnityEngine;

namespace TruePong.Defs {
    [CreateAssetMenu(menuName = "Game/BallDef")]
    public class BallDef : ScriptableObject {
        public float Speed;
        public float Size;
        public Color Color;
    }
}