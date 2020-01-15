using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region Public Fields

        public float Health = 1f;
        public static GameObject LocalPlayerInstance;

        #endregion

        #region Private Fields

        [SerializeField]
        private GameObject beams;
        bool IsFiring;

        #endregion

        #region MonoBehaviour CallBacks

        #if !UNITY_5_4_OR_NEWER
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }   
        #endif

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

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
            if(photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
            if(_cameraWork != null)
            {
                if(photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            #if Unity_5_4_OR_NEWER
            UnityEngine.SceneManagerment.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                this.CalledOnLevelWasLoaded(scene.buildIndex);
            };
            #endif
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInputs();
            }

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
