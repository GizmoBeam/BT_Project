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
                stream.SendNext(Health);
            }
            else
            {
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region Public Fields

        public float Health = 1f;
        public static GameObject LocalPlayerInstance;

        [SerializeField]
        public float maxSpeed = 20f;
        [SerializeField]
        public float aclrt = 5f;

        #endregion

        #region Private Fields

        [SerializeField]
        private GameObject beams;

        private Camera mainCamera;
        private CharacterController characterController;
        private Animator animator;

        bool IsMoving;
        Vector3 targetPos;
        Vector3 dir;
        float distance;
        float currSpeed;

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
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            if (_cameraWork != null)
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

            mainCamera = Camera.main;

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

            transform.rotation = Quaternion.LookRotation(dir);
            characterController.Move(dir * currSpeed * Time.deltaTime);
            animator.SetFloat("Speed", currSpeed);

            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
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

        const int MouseLeft = 0;
        const int MouseRight = 1;
        const int MouseWheel = 2;

        void ProcessInputs()
        {
            if (Input.GetMouseButton(MouseRight))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    targetPos = hit.point;
                    dir = Vector3.Normalize(targetPos - transform.position);
                    dir.y = 0f;
                    currSpeed = Mathf.Clamp(currSpeed += aclrt * Time.deltaTime, 0f, maxSpeed);
                }
                IsMoving = true;
            }
            else
            {
                currSpeed = Mathf.Clamp(currSpeed -= aclrt * Time.deltaTime, 0f, maxSpeed);
            }
        }

        #endregion
    }
}
