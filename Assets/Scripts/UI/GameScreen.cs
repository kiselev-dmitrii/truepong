using System.Linq;
using EZData;
using TruePong.Controllers.Lobby;
using TruePong.GamePlay;
using TruePong.Utils;
using TrueSync;

namespace TruePong.UI {
    public class GameScreen : Window {
        #region Property PlayerScore
        private readonly Property<int> _privatePlayerScoreProperty = new Property<int>();
        public Property<int> PlayerScoreProperty { get { return _privatePlayerScoreProperty; } }
        public int PlayerScore {
            get { return PlayerScoreProperty.GetValue(); }
            set { PlayerScoreProperty.SetValue(value); }
        }
        #endregion

        #region Property EnemyScore
        private readonly Property<int> _privateEnemyScoreProperty = new Property<int>();
        public Property<int> EnemyScoreProperty { get { return _privateEnemyScoreProperty; } }
        public int EnemyScore {
            get { return EnemyScoreProperty.GetValue(); }
            set { EnemyScoreProperty.SetValue(value); }
        }
        #endregion

        private readonly LobbyController lobbyController;
        private readonly Gate playerGate;
        private readonly Gate enemyGate;

        public GameScreen(LobbyController lobbyController, Game game) : base("UI/GameScreen") {
            this.lobbyController = lobbyController;

            playerGate = game.Gates.First(x => x.Paddle.owner == TrueSyncManager.LocalPlayer);
            enemyGate = game.Gates.First(x => x.Paddle.owner != TrueSyncManager.LocalPlayer);
        }

        public void Update() {
            PlayerScore = playerGate.Score;
            EnemyScore = enemyGate.Score;
        }

        public void OnLeaveButtonClick() {
            lobbyController.LeaveGame();
        }
    }
}