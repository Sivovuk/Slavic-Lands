using System;
using System.Collections.Generic;
using Gameplay.Player;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.PlayerProfile
{
    public class PlayerProfileUI : MonoBehaviour
    {
        [SerializeField] private GameObject _playerProfileUI;
        
        [Header("Profile")]
        [SerializeField] private TMP_Text _playerLevel;
        [SerializeField] private TMP_Text _xpPlayer;
        [SerializeField] private Image _levelBarPlayer;
        [SerializeField] private TMP_Text _levelPoints;
        [SerializeField] private Button _savePoints;
        
        [Header("Abilities")]
        [SerializeField] private PlayerProfileAbilityUIElement _abilitySlash;
        [SerializeField] private PlayerProfileAbilityUIElement _abilityShieldBash;
        [SerializeField] private PlayerProfileAbilityUIElement _abilityPiercingArrow;
        
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

        public Action<GameObject> OnTabChange;
        
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
            
            Player.Instance.PlayerProfile.PlayerLevelData.OnPointsChanged += UpdateLevelPoints;

            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged += UpdateAbilitiesUI;
            UpdateAbilitiesUI();
            
            _savePoints.onClick.AddListener(delegate { SavePoints(); });
            _levelPoints.text = "Points : " + Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable;
        }

        private void OnDisable()
        {
            Player.Instance.PlayerProfile.AttackLevelData.OnXpChanged -= UpdateAttackBar;
            Player.Instance.PlayerProfile.ShootLevelData.OnXpChanged -= UpdateShootBar;
            Player.Instance.PlayerProfile.CutLevelData.OnXpChanged -= UpdateCutBar;
            Player.Instance.PlayerProfile.MineLevelData.OnXpChanged -= UpdateMineBar;
            Player.Instance.PlayerProfile.PlayerLevelData.OnXpChanged -= UpdatePlayerBar;
            GameManager.Instance.OnPlayerInit -= OnPlayerInit;
            
            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged -= UpdateAbilitiesUI;
            
            _savePoints.onClick.RemoveListener(delegate { SavePoints(); });
            
            ResetCurrentAbility();
        }

        private void UpdateSkillsUI(TMP_Text level, TMP_Text xp, Image levelBar, LevelData levelData)
        {
            level.text = levelData.CurrentLevel.ToString();
            xp.text = levelData.CurrentXp + " / " + levelData.XpToNextLevel;
            levelBar.fillAmount = (float)levelData.CurrentXp / (float)levelData.XpToNextLevel;
        }

        public void UpdateAttackBar(LevelData levelData)
        {
            UpdateSkillsUI(_attackLevel, _xpAttack, _levelBarAttack, levelData);
        }
        
        public void UpdateShootBar(LevelData levelData)
        {
            UpdateSkillsUI(_shootLevel, _xpShoot, _levelBarShoot, levelData);
        }
        
        public void UpdateCutBar(LevelData levelData)
        {
            UpdateSkillsUI(_cutLevel, _xpCut, _levelBarCut, levelData);
        }
        
        public void UpdateMineBar(LevelData levelData)
        {
            UpdateSkillsUI(_mineLevel, _xpMine, _levelBarMine, levelData);
        }
        
        public void UpdatePlayerBar(LevelData levelData)
        {
            UpdateSkillsUI(_playerLevel, _xpPlayer, _levelBarPlayer, levelData);
        }

        public void UpdateAbilitiesUI()
        {
            _abilitySlash.OnInit(Player.Instance.PlayerProfile.AbilitySlashData, this);
            _abilitySlash.OnInit(Player.Instance.PlayerProfile.AbilityShieldBashData, this);
            _abilitySlash.OnInit(Player.Instance.PlayerProfile.AbilityPiercingArrowData, this);
        }
        
        [SerializeField] private int _pointsUsed;
        [SerializeField] private List<PlayerAbilityLevelData> _currentAbilities = new List<PlayerAbilityLevelData>();

        private void UpdateLevelPoints()
        {
            _levelPoints.text = "Points : " + (Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable - _pointsUsed);
        }

        private void SavePoints()
        {
            foreach (var ability in _currentAbilities)
            {
                Player.Instance.PlayerProfile.SaveNewAbilityLevelData(ability);
                Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable -= _pointsUsed;
                _pointsUsed = 0;
                UpdateLevelPoints();
            }
        }

        public PlayerAbilityLevelData AddPoint(PlayerAbilityLevelData playerAbilityLevelData)
        {
            if (Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable - _pointsUsed <= 0) return playerAbilityLevelData;

            if (_currentAbilities == null)
                _currentAbilities = new List<PlayerAbilityLevelData>();

            if (_currentAbilities.Count > 0)
            {
                foreach (var ability in _currentAbilities)
                {
                    if (ability.Name == playerAbilityLevelData.Name)
                    {
                        ability.CurrentLevel++;
                        _pointsUsed++;
                        UpdateLevelPoints();
                        Debug.Log("existing ability update");
                        return ability;
                    }
                }
            }
            
            _currentAbilities.Add(new PlayerAbilityLevelData(playerAbilityLevelData, playerAbilityLevelData.LevelMultiplayer));
            _currentAbilities[_currentAbilities.Count - 1].CurrentLevel++;
            _pointsUsed++;
            UpdateLevelPoints();
            Debug.Log("new ability update");
            return _currentAbilities[_currentAbilities.Count - 1];
        }

        public PlayerAbilityLevelData RemovePoint(PlayerAbilityLevelData playerAbilityLevelData)
        {
            if (_currentAbilities == null || _currentAbilities.Count <= 0)
                return playerAbilityLevelData;

            if (_pointsUsed <= 0) return playerAbilityLevelData;
            
            foreach (var ability in _currentAbilities)
            {
                if (ability.AbilityID == playerAbilityLevelData.AbilityID)
                {
                    ability.CurrentLevel--;
                    ability.CurrentLevel = Math.Clamp(ability.CurrentLevel, 0, 1000);
                    _pointsUsed--;
                    UpdateLevelPoints();
                    Debug.Log("Remove Point 1");
                    //UpdateAbilityUI(ability);
                    return ability;
                }
            }
            return playerAbilityLevelData;
        }

        public void ResetCurrentAbility()
        {
            Debug.Log("Reset Current Ability");
            _pointsUsed = 0;
            UpdateLevelPoints();
            _currentAbilities = new List<PlayerAbilityLevelData>();
            _abilitySlash.OnClose();
            _abilityShieldBash.OnClose();
            _abilityPiercingArrow.OnClose();
        }
        
        
        private void TabChanged(GameObject panel)
        {
            ResetCurrentAbility();
            UpdateAbilitiesUI();
            
        }
    }
}