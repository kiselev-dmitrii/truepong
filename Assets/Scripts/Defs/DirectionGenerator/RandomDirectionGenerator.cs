using TrueSync;
using UnityEngine;

namespace TruePong.Defs.DirectionGenerator {
    [CreateAssetMenu(menuName = "Game/DirectionGenerator/Random")]
    public class RandomDirectionGenerator : BaseDirectionGenerator {
        public override TSVector2 GenerateDirection() {
            var angle = TSRandom.Range(0.0f, 360.0f);

            TSVector2 result;
            result.x = TSMath.Cos(angle * TSMath.Deg2Rad);
            result.y = TSMath.Sin(angle * TSMath.Deg2Rad);
            return result;
        }
    }
}