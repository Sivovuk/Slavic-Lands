using Core.Interfaces;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.PlayerProfile
{
    public class SkillUIBlock : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _xpText;
        [SerializeField] private Image _progressBar;

        private LevelDataBase _levelData;
        
        private bool _isInitialized = false;

        public void Init(LevelDataBase levelData)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _levelData = levelData;

            _levelData.OnXpChanged += UpdateUI;

            UpdateUI(_levelData);

        }
        
        private void UpdateUI(LevelDataBase data)
        {
            _levelText.text = $"Lv. {data.CurrentLevel}";
            _xpText.text = $"{data.CurrentXp} / {data.XpToNextLevel}";
            _progressBar.fillAmount = (float)data.CurrentXp / data.XpToNextLevel;
        }
    }
}