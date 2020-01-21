using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class UIManager : MonoBehaviour
    {
        #region Private Feilds

        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text hpText;
        [SerializeField]
        private Text mpText;
        [SerializeField]
        private Image hpImage;

        private GameObject player;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            player = GameManager.Instance.playerCharacter;
            nameText.text = PhotonNetwork.LocalPlayer.NickName;
        }

        // Update is called once per frame
        void Update()
        {
            hpText.text = player.GetComponent<PlayerManager>().curr_Hp.ToString() 
                + "/" + player.GetComponent<PlayerManager>().GetMaxHp().ToString();
            mpText.text = player.GetComponent<PlayerManager>().curr_Mp.ToString()
                + "/" + player.GetComponent<PlayerManager>().GetMaxMp().ToString();
            hpImage.fillAmount = player.GetComponent<PlayerManager>().curr_Hp 
                / player.GetComponent<PlayerManager>().GetMaxHp();
        }
    }
}