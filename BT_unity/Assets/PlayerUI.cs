using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        [SerializeField]
        private Text playerNameText;
        [SerializeField]
        private Image playerHp;

        PlayerManager target;
        [SerializeField]
        float characterControllerHeight = 10f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        #endregion

        #region Public Fields

        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region MonoBehaviour CallBacks
        void Update()
        {
            if (playerHp != null)
            {
                float fillAmount = (float)target.curr_Hp / (float)target.MaxHp;
                playerHp.fillAmount = fillAmount;
            }

            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            target = _target;
            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }

            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }

        public void LateUpdate()
        {
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }

        #endregion
    }
}