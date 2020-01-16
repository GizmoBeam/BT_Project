using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
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

        }
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }
        void Update()
        {
            
        }

        #endregion

        #region Public Methods

        [SerializeField]
        private GameObject controlPanel;
        [SerializeField]
        private GameObject progressLabel;

        public void Connect()
        {
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LoadLevel("Lobby");
            }
            else
            { 
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            if(isConnecting)
            {
                PhotonNetwork.LoadLevel("Lobby");
            }
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        #endregion
    }
}