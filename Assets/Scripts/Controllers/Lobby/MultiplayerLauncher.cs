using System;
using TruePong.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public class MultiplayerLauncher : Photon.PunBehaviour, IGameLauncher {
        private String lobbyScene;
        private String gameScene;
        private byte maxPlayers;
        private GameState state;

        private const String roomName = "room1";
        private const String version = "v1.0";

        public event Action OnStateChanged;
        public GameState State {
            get { return state; }
            set {
                state = value;
                OnStateChanged.TryCall();
            }
        }

        public static MultiplayerLauncher Create(String lobbyScene, String gameScene, byte maxPlayers) {
            var result = new GameObject(typeof(MultiplayerLauncher).Name)
                .AddComponent<MultiplayerLauncher>();
            result.Initialize(lobbyScene, gameScene, maxPlayers);
            return result;
        }

        private void Initialize(String lobbyScene, String gameScene, byte maxPlayers) {
            this.lobbyScene = lobbyScene;
            this.gameScene = gameScene;
            this.maxPlayers = maxPlayers;

            DontDestroyOnLoad(gameObject);
            state = GameState.Menu;

            var view = gameObject.AddComponent<PhotonView>();
            view.viewID = 1;
        }

        public void StartGame() {
            if (state != GameState.Menu) {
                throw new InvalidOperationException();
            }

            State = GameState.Starting;
            PhotonNetwork.offlineMode = false;
            PhotonNetwork.ConnectUsingSettings(version);
        }

        public void LeaveGame() {
            switch (state) {
                case GameState.Menu:
                    throw new InvalidOperationException("You are not in game");

                case GameState.Starting:
                    PhotonNetwork.Disconnect();
                    State = GameState.Menu;
                    break;

                case GameState.InGame:
                    PhotonNetwork.Disconnect();
                    SceneLoader.LoadScene(lobbyScene, (scene) => {
                        State = GameState.Menu;
                    });
                    break;
            }
        }

        public override void OnJoinedLobby() {
            var roomOptions = new RoomOptions() {
                MaxPlayers = maxPlayers
            };

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
        }

        public override void OnConnectionFail(DisconnectCause cause) {
            State = GameState.Menu;
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
            if (PhotonNetwork.playerList.Length >= maxPlayers && PhotonNetwork.isMasterClient) {
                photonView.RPC("StartMatch", PhotonTargets.All);
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer) {
            if (PhotonNetwork.playerList.Length == 1) {
                LeaveGame();
            }
        }

        [PunRPC]
        public void StartMatch() {
            PhotonNetwork.LoadLevel(gameScene);
            SceneLoader.WaitLoading((scene) => {
                State = GameState.InGame;
            });
        }
    }
}