using System;
using Gameplay.Player;
using UnityEngine;

namespace UI.Game
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private PlayerInputSystem _playerInputSystem;

        [SerializeField] private GameObject _playerProfileUI;

        [SerializeField] private GameObject _activePanel;
        [SerializeField] private GameObject _activeTab;

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
            OpenPanel(_activePanel != null ? _activePanel : _playerProfileUI, isActive);
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
                _activePanel.SetActive(false);
            }
            
            _activePanel = panel;
            
            _activePanel.SetActive(isActive);
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
        }
    }
}