using System;
using TruePong.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public enum NetworkState {
        Disconnected,
        Connected,
        InGame
    }

    public class MultiplayerLauncher : Photon.PunBehaviour, IGameLauncher {
        public const int MaxPlayers = 2;
        public NetworkState state { get; private set; }

        private Action<Scene> onConnectionSucced;
        private Action onConnectionFailed;

        public static MultiplayerLauncher Create() {
            var result = new GameObject(typeof(MultiplayerLauncher).Name)
                .AddComponent<MultiplayerLauncher>();
            result.Initialize();
            return result;
        }

        private void Initialize() {
            DontDestroyOnLoad(gameObject);
            state = NetworkState.Disconnected;
            var view = gameObject.AddComponent<PhotonView>();
            view.viewID = 1;
        }

        public void StartGame(Action<Scene> onSuccess, Action onFail) {
            if (state != NetworkState.Disconnected) {
                throw new InvalidOperationException("Game must be disconnected");
            }

            onConnectionSucced = onSuccess;
            onConnectionFailed = onFail;

            PhotonNetwork.offlineMode = false;
            PhotonNetwork.ConnectUsingSettings("v1.0");
        }

        public void LeaveGame(Action<Scene> onSuccess) {
            switch (state) {
                case NetworkState.Disconnected:
                    throw new InvalidOperationException("You are not in game");

                case NetworkState.Connected:
                    PhotonNetwork.Disconnect();
                    state = NetworkState.Disconnected;
                    onSuccess.TryCall(SceneLoader.GetActiveScene());
                    break;

                case NetworkState.InGame:
                    PhotonNetwork.Disconnect();
                    SceneLoader.LoadScene("Scenes/Lobby", (scene) => {
                        state = NetworkState.Disconnected;
                        onSuccess.TryCall(scene);
                    });
                    break;
            }
        }

        public override void OnJoinedLobby() {
            state = NetworkState.Connected;
            var roomOptions = new RoomOptions() {
                MaxPlayers = MaxPlayers
            };

            PhotonNetwork.JoinOrCreateRoom("room1", roomOptions, null);
        }

        public override void OnConnectionFail(DisconnectCause cause) {
            state = NetworkState.Disconnected;
            onConnectionFailed.TryCall();
            onConnectionFailed = null;
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
            if (PhotonNetwork.playerList.Length >= MaxPlayers && PhotonNetwork.isMasterClient) {
                photonView.RPC("StartMatch", PhotonTargets.All);
            }
        }

        [PunRPC]
        public void StartMatch() {
            PhotonNetwork.LoadLevel("Scenes/Game");
            SceneLoader.WaitLoading((scene) => {
                state = NetworkState.InGame;
                onConnectionSucced.TryCall(scene);
            });
        }
    }
}