using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public float Health = 1f;

        #endregion

        #region Private Fields

        [SerializeField]
        private GameObject beams;
        bool IsFiring;

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }
        }

        void Update()
        {
            ProcessInputs();
            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            
            if (!other.name.Contains("Beam"))
            {
                return;
            }
            
            Health -= 0.1f * Time.deltaTime;
        }

        #endregion

        #region Custom

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

        #endregion
    }
}
