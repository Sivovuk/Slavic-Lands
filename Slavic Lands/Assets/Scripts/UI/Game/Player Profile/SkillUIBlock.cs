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

        public void Init(LevelDataBase levelData)
        {
            _levelData = levelData;
            UpdateUI(levelData);

            _levelData.OnXpChanged += UpdateUI;
        }

        private void OnDisable()
        {
            if (_levelData != null)
                _levelData.OnXpChanged -= UpdateUI;
        }

        private void UpdateUI(LevelDataBase data)
        {
            _levelText.text = $"Lv. {data.CurrentLevel}";
            _xpText.text = $"{data.CurrentXp} / {data.XpToNextLevel}";
            _progressBar.fillAmount = (float)data.CurrentXp / data.XpToNextLevel;
        }
    }
}