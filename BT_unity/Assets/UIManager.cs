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
        private Text nameLvText;
        [SerializeField]
        private Text hpText;
        [SerializeField]
        private Text mpText;
        [SerializeField]
        private Text expText;
        [SerializeField]
        private Image hpImage;
        [SerializeField]
        private Image mpImage;
        [SerializeField]
        private Image expImage;
        [SerializeField]
        private Image portraitImage;
        private GameObject player;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            player = GameManager.Instance.playerCharacter;
            nameLvText.text = PhotonNetwork.LocalPlayer.NickName + " / Lv: " + player.GetComponent<PlayerManager>().Level;
            
        }

        // Update is called once per frame
        void Update()
        {
            nameLvText.text = PhotonNetwork.LocalPlayer.NickName + " / Lv: " + player.GetComponent<PlayerManager>().Level;
            hpText.text = player.GetComponent<PlayerManager>().curr_Hp.ToString() 
                + "/" + player.GetComponent<PlayerManager>().MaxHp.ToString();
            mpText.text = player.GetComponent<PlayerManager>().curr_Mp.ToString()
                + "/" + player.GetComponent<PlayerManager>().MaxMp.ToString();
            expText.text = player.GetComponent<PlayerManager>().curr_Exp.ToString()
                + "/" + player.GetComponent<PlayerManager>().MaxExp.ToString();
            hpImage.fillAmount = (float)player.GetComponent<PlayerManager>().curr_Hp 
                / (float)player.GetComponent<PlayerManager>().MaxHp;
            mpImage.fillAmount = (float)player.GetComponent<PlayerManager>().curr_Mp
                / (float)player.GetComponent<PlayerManager>().MaxMp;
            expImage.fillAmount = (float)player.GetComponent<PlayerManager>().curr_Exp
                / (float)player.GetComponent<PlayerManager>().MaxExp;
        }
    }
}