using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{   
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        const string playerNamePrefKey = "DPM";

        void Start()
        {
            string defaultname = string.Empty;
            InputField _inputfeild = this.GetComponent<InputField>();
            if(_inputfeild != null)
            {
                if(PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultname = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputfeild.text = defaultname;
                }
            }

            PhotonNetwork.NickName = defaultname;
        }

        public void SetPlayerName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = name;

            PlayerPrefs.SetString(playerNamePrefKey, name);
        }

    }
}