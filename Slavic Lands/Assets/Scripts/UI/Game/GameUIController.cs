using System;
using Gameplay.Player;
using UnityEngine;

namespace UI.Game
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private PlayerInputSystem _playerInputSystem;

        [SerializeField] private GameObject _playerProfileUI;

        private GameObject _activePanel;

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
            OpenPanel(_playerProfileUI, isActive);
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
    }
}