using System;
using TruePong.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers.Lobby {
    public enum LobbyState {
        Disconnected,
        Connected,
        InGame
    }

    public class MultiplayerLauncher : Photon.PunBehaviour, IGameLauncher {
        public const int MaxPlayers = 2;
        public LobbyState State { get; private set; }

        private Action<Scene> onSuccess;
        private Action onFail;

        public static MultiplayerLauncher Create() {
            var result = new GameObject(typeof(MultiplayerLauncher).Name)
                .AddComponent<MultiplayerLauncher>();
            result.Initialize();
            return result;
        }

        private void Initialize() {
            DontDestroyOnLoad(gameObject);
            State = LobbyState.Disconnected;
            var view = gameObject.AddComponent<PhotonView>();
            view.viewID = 1;
        }

        public void StartGame(Action<Scene> onSuccess, Action onFail) {
            PhotonNetwork.offlineMode = false;

            if (State != LobbyState.Disconnected) {
                throw new InvalidOperationException("Game must be disconnected");
            }

            this.onSuccess = onSuccess;
            this.onFail = onFail;

            PhotonNetwork.ConnectUsingSettings("v1.0");
        }

        public void LeaveGame() {
            if (State == LobbyState.Disconnected) {
                throw new InvalidOperationException("You are not in game");
            }

            if (State == LobbyState.Connected) {
                PhotonNetwork.Disconnect();
            }

            if (State == LobbyState.InGame) {
                SceneManager.LoadScene("Scenes/Lobby");
            }

            State = LobbyState.Disconnected;
        }

        public override void OnJoinedLobby() {
            State = LobbyState.Connected;
            var roomOptions = new RoomOptions() {
                MaxPlayers = MaxPlayers
            };

            PhotonNetwork.JoinOrCreateRoom("room1", roomOptions, null);
        }

        public override void OnConnectionFail(DisconnectCause cause) {
            State = LobbyState.Disconnected;
            onFail.TryCall();
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
            if (PhotonNetwork.playerList.Length >= MaxPlayers && PhotonNetwork.isMasterClient) {
                photonView.RPC("StartMatch", PhotonTargets.All);
            }
        }

        [PunRPC]
        public void StartMatch() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            PhotonNetwork.LoadLevel("Scenes/Game");
            State = LobbyState.InGame;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            onSuccess.TryCall(SceneManager.GetActiveScene());
        }

    }
}