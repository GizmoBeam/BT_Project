using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields
        
        [SerializeField]
        private float directionDampTime = 0.25f;

        #endregion

        #region MonoBehaviour Callbakcs

        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            if(!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }

        void Update()
        {
            if(!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                return;
            }

            if(!animator)
            {
                return;
            }

            AnimatorStateInfo stateinfo = animator.GetCurrentAnimatorStateInfo(0);

            if(stateinfo.IsName("Base Layer.Locomotion"))
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            if(v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }
        #endregion
    }
}