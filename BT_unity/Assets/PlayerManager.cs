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
                stream.SendNext(curr_Hp);
                stream.SendNext(currSpeed);
            }
            else
            {
                this.curr_Hp = (float)stream.ReceiveNext();
                this.currSpeed = (float)stream.ReceiveNext();
            }
        }

        #endregion

        #region Public Fields
        public static GameObject LocalPlayerInstance;

        public float curr_Hp;
        public float curr_Mp;

        [SerializeField]
        public float maxSpeed = 5f;
        [SerializeField]
        public float aclrt = 3f;

        #endregion

        #region SerializeField

        [SerializeField]
        int Lv = 1;
        [SerializeField]
        float max_Hp = 500f;
        [SerializeField]
        float max_Mp = 250f;
        [SerializeField]
        int max_Exp = 100;
        [SerializeField]
        float Atk = 10f;
        [SerializeField]
        float Def = 2f;
        [SerializeField]
        float Str = 5f;
        [SerializeField]
        float Dex = 5f;
        [SerializeField]
        float Int = 5f;
        [SerializeField]
        int Main_Stat;
        [SerializeField]
        float Str_per_LV = 1.5f;
        [SerializeField]
        float Dex_per_LV = 1.5f;
        [SerializeField]
        float Int_per_LV = 1.5f;
        [SerializeField]
        float Attack_Speed = 1.5f;
        #endregion

        #region Private Method
        int curr_Exp = 0;

        float additional_Atk;
        float additional_Def;
        float additional_Attack_Speed;
        float additional_Move_Speed;
        float additional_Str;
        float additional_Dex;
        float additional_Int;

        private CharacterController characterController;
        private Animator animator;

        [SerializeField]
        private GameObject playerUiPrefab;

        Vector3 targetPos;
        Vector3 dir;
        float currSpeed;

        void Init()
        {
            max_Hp += Str * 25f;
            max_Mp += Int * 20f;

            curr_Hp = max_Hp;
            curr_Mp = max_Mp;

            float mainstat;
            if (Main_Stat == 0)
                mainstat = Str;
            else if (Main_Stat == 1)
                mainstat = Dex;
            else
                mainstat = Int;

            Atk += mainstat * 2;
            Def += Dex * 0.01f;
        }

        void LevelUp()
        {
            Lv++;
            Str += Str_per_LV;
            Dex += Dex_per_LV;
            Int += Int_per_LV;
            curr_Exp -= max_Exp;
            max_Exp += Lv * 100;
        }

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
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        void Awake()
        {
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

            Init();

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

            if (!photonView.IsMine)
            {
                if (playerUiPrefab != null)
                {
                    GameObject _uiGo = Instantiate(playerUiPrefab);
                    _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                }
                else
                {
                    Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
                }
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

                transform.rotation = Quaternion.LookRotation(dir);
                characterController.Move(dir * currSpeed * Time.deltaTime);
                animator.SetFloat("Speed", currSpeed);
            }

            if (curr_Hp <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }

            if (curr_Exp >= max_Exp)
                LevelUp();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            curr_Hp -= 0.1f;
        }

        void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            
            curr_Hp -= 0.1f * Time.deltaTime;
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    targetPos = hit.point;
                    dir = Vector3.Normalize(targetPos - transform.position);
                    dir.y = 0f;
                    currSpeed = Mathf.Clamp(currSpeed += aclrt * Time.deltaTime, 0f, maxSpeed);
                }
            }
            else
            {
                currSpeed = Mathf.Clamp(currSpeed -= aclrt * Time.deltaTime, 0f, maxSpeed);
            }
        }

        #endregion
    }
}
