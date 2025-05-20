using System;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.PlayerProfile
{
    public class PlayerProfileAbilityUIElement : MonoBehaviour
    {
        [SerializeField] private Button _plusBtn;
        [SerializeField] private Button _minusBtn;
        [SerializeField] private TMP_Text _abilityLevel;
        
        private AbilityID _abilityID;
        private PlayerAbilityLevelData _playerAbilityLevelData;
        private PlayerProfileUI _playerProfileUI;

        public void OnInit(PlayerAbilityLevelData playerAbilityLevelData, PlayerProfileUI playerProfileUI)
        {
            _playerAbilityLevelData = playerAbilityLevelData;
            _playerProfileUI = playerProfileUI;
            _plusBtn.onClick.RemoveAllListeners();
            _minusBtn.onClick.RemoveAllListeners();
            
            _abilityLevel.text = playerAbilityLevelData.Name + " : " + playerAbilityLevelData.CurrentLevel;
            _plusBtn.onClick.AddListener(delegate { PlusBtnOnClick(); });
            _minusBtn.onClick.AddListener(delegate { MinusBtnOnClick(); });
        }

        public void OnClose()
        {
            _playerAbilityLevelData = null;
            _playerProfileUI = null;
        }

        private void PlusBtnOnClick()
        {
            _playerAbilityLevelData = _playerProfileUI.AddPoint(_playerAbilityLevelData);
            
            _abilityLevel.text = _playerAbilityLevelData.Name + " : " + _playerAbilityLevelData.CurrentLevel;
        }

        private void MinusBtnOnClick()
        {
            _playerAbilityLevelData = _playerProfileUI.RemovePoint(_playerAbilityLevelData);
            
            _abilityLevel.text = _playerAbilityLevelData.Name + " : " + _playerAbilityLevelData.CurrentLevel;
        }
    }
}