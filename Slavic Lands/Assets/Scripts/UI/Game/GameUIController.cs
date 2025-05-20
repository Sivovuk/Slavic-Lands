using System;
using Gameplay.Player;
using UI.Game.PlayerProfile;
using UnityEngine;

namespace UI.Game
{
    public class GameUIController : MonoBehaviour
    {
        private PlayerInputSystem _playerInputSystem;
        private PlayerProfileUI _playerProfileUI;

        [SerializeField] private GameObject _playerProfilePanel;

        [SerializeField] private GameObject _activePanel;
        [SerializeField] private GameObject _activeTab;

        private void Awake()
        {
            _playerProfileUI = GetComponent<PlayerProfileUI>();
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnTabClicked += OpenMenu;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnTabClicked -= OpenMenu;
        }

        public void OpenMenu(bool isActive)
        {
            OpenPanel(_activePanel != null ? _activePanel : _playerProfilePanel, isActive);
            //OpenTab();
        }


        public void OpenPanel(GameObject panel, bool isActive)
        {
            if (panel == null)
            {
                return;
            }

            if (_activePanel != null)
            {
                _playerProfileUI.OnTabChange?.Invoke(_activePanel);
                _activePanel.SetActive(false);
            }
            
            _activePanel = panel;
            
            _activePanel.SetActive(isActive);
            
            _playerProfileUI.OnTabChange?.Invoke(_activePanel);
        }

        public void OpenTab(GameObject tab, bool isActive)
        {
            if (tab == null)
            {
                return;
            }

            if (_activeTab != null)
            {
                _activeTab.SetActive(false);
            }
            
            _activeTab = tab;
            
            _activeTab.SetActive(isActive);
            
            _playerProfileUI.OnTabChange?.Invoke(_activeTab);
        }
    }
}