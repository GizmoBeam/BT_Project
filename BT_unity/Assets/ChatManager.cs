using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class ChatManager : MonoBehaviour
{
    #region PublicFields

    public Text ChatText;
    public InputField ChatInput;
    public ScrollRect scroll_rect;

    #endregion

    #region 채팅

    public PhotonView PV;

    private bool is_chat;

    public void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text + "\n";
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        ChatText.text += msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        ChatInput.text = "";
        ChatText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (ChatInput.text == "")
                ChatInput.Select();
            else if (ChatInput.text != "")
                Send();
        }
    }
}
