using System;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Interaction
{
    /// <summary>
    /// Displays a popup choice menu when interacting with an NPC (e.g. Talk, Shop, Leave).
    /// </summary>
    public class NPCInteractionChoiceUI : MonoBehaviour
    {
        public static NPCInteractionChoiceUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private TMP_Text _npcNameText;
        [SerializeField] private Button _talkButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _leaveButton;

        private Action _onTalkAction;
        private Action _onShopAction;
        private Action _onLeaveAction;

        public bool IsOpen => _panelRoot != null && _panelRoot.activeSelf;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            if (_panelRoot != null)
                _panelRoot.SetActive(false);

            if (_talkButton != null)
                _talkButton.onClick.AddListener(HandleTalkClicked);
            if (_shopButton != null)
                _shopButton.onClick.AddListener(HandleShopClicked);
            if (_leaveButton != null)
                _leaveButton.onClick.AddListener(HandleLeaveClicked);
        }

        public void OpenChoices(string npcName, Action onTalk, Action onShop, Action onLeave = null, bool hasShop = true)
        {
            _onTalkAction = onTalk;
            _onShopAction = onShop;
            _onLeaveAction = onLeave;

            if (_npcNameText != null)
            {
                _npcNameText.text = string.IsNullOrEmpty(npcName) ? "Mještanin" : npcName;
            }

            if (_shopButton != null)
            {
                _shopButton.gameObject.SetActive(hasShop);
            }

            if (_panelRoot != null)
            {
                _panelRoot.SetActive(true);
            }

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(true);
            }
        }

        public void CloseChoices()
        {
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }

            _onTalkAction = null;
            _onShopAction = null;
            _onLeaveAction = null;

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(false);
            }
        }

        private void HandleTalkClicked()
        {
            Action callback = _onTalkAction;
            CloseChoices();
            callback?.Invoke();
        }

        private void HandleShopClicked()
        {
            Action callback = _onShopAction;
            CloseChoices();
            callback?.Invoke();
        }

        private void HandleLeaveClicked()
        {
            Action callback = _onLeaveAction;
            CloseChoices();
            callback?.Invoke();
        }
    }
}
