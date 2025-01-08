using System;
using Gameplay.Player;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class PlayerProfileUI : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private TMP_Text _attackLevel;
        [SerializeField] private TMP_Text _xpAttack;
        [SerializeField] private Image _levelBarAttack;
        [Header("Shoot")]
        [SerializeField] private TMP_Text _shootLevel;
        [SerializeField] private TMP_Text _xpShoot;
        [SerializeField] private Image _levelBarShoot;
        [Header("Cut")]
        [SerializeField] private TMP_Text _cutLevel;
        [SerializeField] private TMP_Text _xpCut;
        [SerializeField] private Image _levelBarCut;
        [Header("Mine")]
        [SerializeField] private TMP_Text _mineLevel;
        [SerializeField] private TMP_Text _xpMine;
        [SerializeField] private Image _levelBarMine;

        private void OnEnable()
        {
            GameManager.Instance.OnPlayerInit += OnPlayerInit;
        }

        private void OnPlayerInit()
        {
            Player.Instance.PlayerProfile.AttackLevelData.OnXpChanged += UpdateAttackBar;
            Player.Instance.PlayerProfile.ShootLevelData.OnXpChanged += UpdateShootBar;
            Player.Instance.PlayerProfile.CutLevelData.OnXpChanged += UpdateCutBar;
            Player.Instance.PlayerProfile.MineLevelData.OnXpChanged += UpdateMineBar;
        }

        private void OnDisable()
        {
            Player.Instance.PlayerProfile.AttackLevelData.OnXpChanged -= UpdateAttackBar;
            Player.Instance.PlayerProfile.ShootLevelData.OnXpChanged -= UpdateShootBar;
            Player.Instance.PlayerProfile.CutLevelData.OnXpChanged -= UpdateCutBar;
            Player.Instance.PlayerProfile.MineLevelData.OnXpChanged -= UpdateMineBar;
            GameManager.Instance.OnPlayerInit -= OnPlayerInit;
        }

        private void UpdateUI(TMP_Text level, TMP_Text xp, Image levelBar, PlayerLevelData playerLevelData)
        {
            level.text = playerLevelData.CurrentLevel.ToString();
            xp.text = playerLevelData.CurrentXp + " / " + playerLevelData.XpToNextLevel;
            levelBar.fillAmount = playerLevelData.CurrentXp / playerLevelData.XpToNextLevel;
            Debug.LogError(playerLevelData.CurrentXp / playerLevelData.XpToNextLevel);
        }

        public void UpdateAttackBar(PlayerLevelData playerLevelData)
        {
            UpdateUI(_attackLevel, _xpAttack, _levelBarAttack, playerLevelData);
        }
        
        public void UpdateShootBar(PlayerLevelData playerLevelData)
        {
            UpdateUI(_shootLevel, _xpShoot, _levelBarShoot, playerLevelData);
        }
        
        public void UpdateCutBar(PlayerLevelData playerLevelData)
        {
            UpdateUI(_cutLevel, _xpCut, _levelBarCut, playerLevelData);
        }
        
        public void UpdateMineBar(PlayerLevelData playerLevelData)
        {
            UpdateUI(_mineLevel, _xpMine, _levelBarMine, playerLevelData);
        }
    }
}