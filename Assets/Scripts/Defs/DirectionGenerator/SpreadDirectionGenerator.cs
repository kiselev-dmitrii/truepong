using TrueSync;
using UnityEngine;

namespace TruePong.Defs.DirectionGenerator {
    [CreateAssetMenu(menuName = "Game/DirectionGenerator/Spread")]
    public class SpreadDirectionGenerator : BaseDirectionGenerator {
        public float SpreadAngle = 45;

        public override TSVector2 GenerateDirection() {
            var angle = TSRandom.Range(-SpreadAngle, SpreadAngle);
            float offset = TSRandom.Range(0, 2) == 0 ? 90 : 270;
            angle += offset;

            TSVector2 result;
            result.x = TSMath.Cos(angle*TSMath.Deg2Rad);
            result.y = TSMath.Sin(angle * TSMath.Deg2Rad);
            return result;
        }
    }
}