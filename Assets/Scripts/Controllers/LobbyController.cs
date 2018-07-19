using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TruePong.Controllers {
    public enum LobbyState {
        Disconnected,
        Connected,
        InGame
    }

    public class LobbyController : Photon.PunBehaviour {
        public const int MaxPlayers = 2;
        public LobbyState State { get; private set; }

        public static LobbyController Create() {
            var result = new GameObject(typeof(LobbyController).Name)
                .AddComponent<LobbyController>();
            result.Initialize();
            return result;
        }

        private void Initialize() {
            DontDestroyOnLoad(gameObject);
            State = LobbyState.Disconnected;
        }

        public void StartGame() {
            PhotonNetwork.offlineMode = false;

            if (State != LobbyState.Disconnected) {
                throw new InvalidOperationException("Game must be disconnected");
            }
            PhotonNetwork.ConnectUsingSettings("v1.0");
            PhotonNetwork.automaticallySyncScene = true;
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
            PhotonNetwork.offlineMode = true;
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
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
            if (PhotonNetwork.playerList.Length >= MaxPlayers && PhotonNetwork.isMasterClient) {
                PhotonNetwork.LoadLevel("Scenes/Game");
                State = LobbyState.InGame;
            }
        }
    }
}