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
        [Header("Profile")]
        [SerializeField] private TMP_Text _playerLevel;
        [SerializeField] private TMP_Text _xpPlayer;
        [SerializeField] private Image _levelBarPlayer;
        [SerializeField] private TMP_Text _levelPoints;
        [SerializeField] private Button _savePoints;
        [SerializeField] private GameObject _abilitySlash;
        [SerializeField] private GameObject _abilityShieldBash;
        [SerializeField] private GameObject _abilityPiercingArrow;
        
        [Header("Skills")]
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

        public Action OnTabChange;
        
        private void OnEnable()
        {
            GameManager.Instance.OnPlayerInit += OnPlayerInit;

            OnTabChange += TabChanged;
        }

        private void OnPlayerInit()
        {
            Player.Instance.PlayerProfile.AttackLevelData.OnXpChanged += UpdateAttackBar;
            Player.Instance.PlayerProfile.ShootLevelData.OnXpChanged += UpdateShootBar;
            Player.Instance.PlayerProfile.CutLevelData.OnXpChanged += UpdateCutBar;
            Player.Instance.PlayerProfile.MineLevelData.OnXpChanged += UpdateMineBar;
            Player.Instance.PlayerProfile.PlayerLevelData.OnXpChanged += UpdatePlayerBar;

            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged += UpdateAbilityUI;
            UpdateAbilityUI();
            
            _savePoints.onClick.AddListener(delegate { SavePoints(); });
        }

        private void OnDisable()
        {
            Player.Instance.PlayerProfile.AttackLevelData.OnXpChanged -= UpdateAttackBar;
            Player.Instance.PlayerProfile.ShootLevelData.OnXpChanged -= UpdateShootBar;
            Player.Instance.PlayerProfile.CutLevelData.OnXpChanged -= UpdateCutBar;
            Player.Instance.PlayerProfile.MineLevelData.OnXpChanged -= UpdateMineBar;
            Player.Instance.PlayerProfile.PlayerLevelData.OnXpChanged -= UpdatePlayerBar;
            GameManager.Instance.OnPlayerInit -= OnPlayerInit;
            
            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged -= UpdateAbilityUI;
            
            _savePoints.onClick.RemoveListener(delegate { SavePoints(); });
        }

        private void UpdateSkillsUI(TMP_Text level, TMP_Text xp, Image levelBar, PlayerLevelData playerLevelData)
        {
            level.text = playerLevelData.CurrentLevel.ToString();
            xp.text = playerLevelData.CurrentXp + " / " + playerLevelData.XpToNextLevel;
            levelBar.fillAmount = (float)playerLevelData.CurrentXp / (float)playerLevelData.XpToNextLevel;
        }

        public void UpdateAttackBar(PlayerLevelData playerLevelData)
        {
            UpdateSkillsUI(_attackLevel, _xpAttack, _levelBarAttack, playerLevelData);
        }
        
        public void UpdateShootBar(PlayerLevelData playerLevelData)
        {
            UpdateSkillsUI(_shootLevel, _xpShoot, _levelBarShoot, playerLevelData);
        }
        
        public void UpdateCutBar(PlayerLevelData playerLevelData)
        {
            UpdateSkillsUI(_cutLevel, _xpCut, _levelBarCut, playerLevelData);
        }
        
        public void UpdateMineBar(PlayerLevelData playerLevelData)
        {
            UpdateSkillsUI(_mineLevel, _xpMine, _levelBarMine, playerLevelData);
        }
        
        public void UpdatePlayerBar(PlayerLevelData playerLevelData)
        {
            UpdateSkillsUI(_playerLevel, _xpPlayer, _levelBarPlayer, playerLevelData);
        }

        private int _points;
        private PlayerAbilityLevelData _currentAbilityLevel;

        public void UpdateAbilityUI()
        {
            UpdateAbilityUI(Player.Instance.PlayerProfile.AbilitySlashData, _abilitySlash );
            UpdateAbilityUI(Player.Instance.PlayerProfile.AbilityShieldBashData, _abilityShieldBash );
            UpdateAbilityUI(Player.Instance.PlayerProfile.AbilityPiercingArrowData, _abilityPiercingArrow );
        }

        private void UpdateAbilityUI(PlayerAbilityLevelData playerAbilityLevelData, GameObject abilityUI)
        {
            Button plusButton = abilityUI.transform.GetChild(1).gameObject.GetComponent<Button>();
            Button minusButton = abilityUI.transform.GetChild(2).gameObject.GetComponent<Button>();
            
            plusButton.onClick.RemoveAllListeners();
            minusButton.onClick.RemoveAllListeners();
            
            abilityUI.GetComponentInChildren<TMP_Text>().text = playerAbilityLevelData.Name + " : " + playerAbilityLevelData.CurrentLevel;
            abilityUI.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate { AddPoint(playerAbilityLevelData, abilityUI.GetComponentInChildren<TMP_Text>()); });
            abilityUI.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(delegate { RemovePoint(playerAbilityLevelData, abilityUI.GetComponentInChildren<TMP_Text>()); });
        }

        private void SavePoints()
        {
            _currentAbilityLevel.CurrentLevel = _points;
            Player.Instance.PlayerProfile.SaveNewAbilityLevelData(_currentAbilityLevel);
            ResetCurrentAbility();
        }

        private void AddPoint(PlayerAbilityLevelData playerAbilityLevelData, TMP_Text pointText)
        {
            if ( _currentAbilityLevel == null || playerAbilityLevelData != _currentAbilityLevel)
            {
                _points = 0;
                _currentAbilityLevel = playerAbilityLevelData;
            }
            
            _points++;
            pointText.text = playerAbilityLevelData.Name + " : " + _points;
        }

        private void RemovePoint(PlayerAbilityLevelData playerAbilityLevelData, TMP_Text pointText)
        {
            
            if ( _currentAbilityLevel == null || playerAbilityLevelData != _currentAbilityLevel)
            {
                _points = 0;
                _currentAbilityLevel = playerAbilityLevelData;
            }

            if (_points <= 0) return;
            
            _points--;
            pointText.text = playerAbilityLevelData.Name + " : " + _points;
        }

        public void ResetCurrentAbility()
        {
            _points = 0;
            _currentAbilityLevel = null;
        }
        
        
        private void TabChanged()
        {
            ResetCurrentAbility();
        }
    }
}