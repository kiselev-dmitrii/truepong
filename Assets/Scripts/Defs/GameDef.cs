using UnityEngine;

namespace TruePong.Defs {
    [CreateAssetMenu(menuName = "Game/GameDef")]
    public class GameDef : ScriptableObject {
        public Color EnemyColor;
        public Color PlayerColor;

        public BallDef[] Balls;
    }
}
