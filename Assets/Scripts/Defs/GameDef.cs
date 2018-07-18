using UnityEngine;

namespace TruePong.Defs {
    [CreateAssetMenu(menuName = "Game/GameDef")]
    public class GameDef : ScriptableObject {
        public BallDef[] Balls;
    }
}
