using TruePong.Defs.DirectionGenerator;
using TrueSync;
using UnityEngine;

namespace TruePong.Defs {
    [CreateAssetMenu(menuName = "Game/BallDef")]
    public class BallDef : ScriptableObject {
        public FP Speed;
        public FP Size;
        public Color Color;
        public BaseDirectionGenerator DirectionGenerator;
    }
}