using TrueSync;
using UnityEngine;

namespace TruePong.Defs.DirectionGenerator {
    public abstract class BaseDirectionGenerator : ScriptableObject {
        public abstract TSVector2 GenerateDirection();
    }
}