using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        string gameVersion = "1";
        [SerializeField]
        private byte maxPlayerPerRoom = 8;
        #endregion

        #region Private Methods

        bool isConnecting;

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.JoinLobby();
        }
        void Start()
        {

        }
        void Update()
        {

        }

        #endregion

        #region Public Methods

        public void CreateRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("CreateRoom");
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
            }
        }

        public void JoinRandomRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("JoinRandomRoom");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("JoinedRoom");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; ++i)
                Debug.LogFormat("{0}, {1}", roomList[i].Name, roomList[i].PlayerCount);
        }

        #endregion
    }
}
