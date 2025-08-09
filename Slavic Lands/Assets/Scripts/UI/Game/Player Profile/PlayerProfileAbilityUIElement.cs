using Core.Interfaces;
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

        private ToolType _toolType;
        private PlayerAbilityLevelData _playerAbilityLevelData;
        private PlayerProfileUI _playerProfileUI;

        public void OnInit(PlayerAbilityLevelData data, PlayerProfileUI ui)
        {
            _playerAbilityLevelData = data;
            _playerProfileUI = ui;

            _plusBtn.onClick.RemoveAllListeners();
            _minusBtn.onClick.RemoveAllListeners();

            _plusBtn.onClick.AddListener(PlusBtnOnClick);
            _minusBtn.onClick.AddListener(MinusBtnOnClick);

            UpdateDisplay();
        }

        private void PlusBtnOnClick()
        {
            _playerAbilityLevelData = _playerProfileUI.AddPoint(_playerAbilityLevelData);
            UpdateDisplay();
        }

        private void MinusBtnOnClick()
        {
            _playerAbilityLevelData = _playerProfileUI.RemovePoint(_playerAbilityLevelData);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _abilityLevel.text = $"{_playerAbilityLevelData.ToolType} : {_playerAbilityLevelData.CurrentLevel}";
        }

        public void OnClose()
        {
            _playerAbilityLevelData = null;
            _playerProfileUI = null;
        }
    }
}