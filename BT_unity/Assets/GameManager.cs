using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public static GameManager Instance;

        void Start()
        {
            Instance = this;
        }

        #endregion

        #region Private Methods
        void LoadArena()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion

        #region Photon Callbacks
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() { 0}", otherPlayer.NickName);
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
                LoadArena();
            }
        }
        #endregion

        #region Public Methods
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion
    }
}