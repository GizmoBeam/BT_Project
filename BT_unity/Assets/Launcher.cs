using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Header("DisconnectPanel")]
        public InputField NickNameInput;

        [Header("LobbyPanel")]
        public GameObject LobbyPanel;
        public InputField RoomInput;
        public Text WelcomeText;
        public Text LobbyInfoText;
        public Button[] CellBtn;
        public Button PreviousBtn;
        public Button NextBtn;

        [Header("RoomPanel")]
        public GameObject RoomPanel;
        public Text ListText;
        public Text RoomInfoText;
        public Button StartButton;
        public ChatManager chatmanager;

        [Header("ETC")]
        public Text StatusText;
        public PhotonView PV;

        List<RoomInfo> myList = new List<RoomInfo>();
        int currentPage = 1, maxPage, multiple;

        #region 방리스트 갱신
        // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
        public void MyListClick(int num)
        {
            if (num == -2) --currentPage;
            else if (num == -1) ++currentPage;
            else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            MyListRenewal();
        }

        void MyListRenewal()
        {
            // 최대페이지
            maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

            // 이전, 다음버튼
            PreviousBtn.interactable = (currentPage <= 1) ? false : true;
            NextBtn.interactable = (currentPage >= maxPage) ? false : true;

            // 페이지에 맞는 리스트 대입
            multiple = (currentPage - 1) * CellBtn.Length;
            for (int i = 0; i < CellBtn.Length; i++)
            {
                CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
                CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
                CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            int roomCount = roomList.Count;
            for (int i = 0; i < roomCount; i++)
            {
                if (!roomList[i].RemovedFromList)
                {
                    if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                    else myList[myList.IndexOf(roomList[i])] = roomList[i];
                }
                else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
            }
            MyListRenewal();
        }
        #endregion


        #region 서버연결
        void Awake()
        {
            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.AllocateSceneViewID(photonView);
        }

        void Update()
        {
            if(PhotonNetwork.InRoom)
            {
                ;
            }
            else
            {
                StatusText.text = PhotonNetwork.NetworkClientState.ToString();
                LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
            }
        }

        public void Connect()
        {
            if(!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            LobbyPanel.SetActive(true);
            RoomPanel.SetActive(false);
            if (NickNameInput.text != "")
                PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            else
                PhotonNetwork.LocalPlayer.NickName = "DPM";
            WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
            myList.Clear();
        }

        public void Disconnect() => PhotonNetwork.Disconnect();

        public override void OnDisconnected(DisconnectCause cause)
        {
            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(false);
        }
        #endregion


        #region 방

        public void GameStart()
        {
            PV.RPC("GameStartRPC", RpcTarget.All, "Room for 1");
        }

        [PunRPC]
        void GameStartRPC(string sceneName)
        {
            PhotonNetwork.LoadLevel(sceneName);
        }

        public void CreateRoom()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });
                RoomInput.text = "";
                StartButton.gameObject.SetActive(true);
            }
        }

        public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

        public void LeaveRoom() => PhotonNetwork.LeaveRoom();

        public override void OnJoinedRoom()
        {
            RoomPanel.SetActive(true);
            RoomRenewal();
        }

        public override void OnCreateRoomFailed(short returnCode, string message) 
        {
            RoomInput.text = ""; 
            if(!PhotonNetwork.InRoom)
                CreateRoom(); 
        }

        public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            RoomRenewal();
            chatmanager.PV.RPC("ChatRPC", RpcTarget.All, "<color=red>" + newPlayer.NickName + "님이 참가하셨습니다</color>\n");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            RoomRenewal();
            chatmanager.PV.RPC("ChatRPC", RpcTarget.All, "<color=red>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>\n");
        }

        void RoomRenewal()
        {
            ListText.text = "";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
            RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
        }

        #endregion
    }
}