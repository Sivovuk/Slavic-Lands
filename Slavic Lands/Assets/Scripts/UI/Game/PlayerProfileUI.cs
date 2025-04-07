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

            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged += UpdateAbilityUI;
            UpdateAbilityUI();
            
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
            
            Player.Instance.PlayerProfile.AbilitySlashData.OnLevelChanged -= UpdateAbilityUI;
            
            _savePoints.onClick.RemoveListener(delegate { SavePoints(); });
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

        private int _pointsUsed;
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
        
        private void UpdateLevelPoints()
        {
            //Debug.LogError("UpdateLevelPoints");
            _levelPoints.text = "Points : " + Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable;
        }

        private void SavePoints()
        {
            _currentAbilityLevel.CurrentLevel = _pointsUsed;
            Player.Instance.PlayerProfile.SaveNewAbilityLevelData(_currentAbilityLevel);
            ResetCurrentAbility();
        }

        private void AddPoint(PlayerAbilityLevelData playerAbilityLevelData, TMP_Text pointText)
        {
            if ( _currentAbilityLevel == null || playerAbilityLevelData != _currentAbilityLevel)
            {
                _pointsUsed = 0;
                _currentAbilityLevel = playerAbilityLevelData;
            }

            if (Player.Instance.PlayerProfile.PlayerLevelData.CanUsePoints())
            {
                if (_pointsUsed < Player.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable)
                {
                    _pointsUsed++;
                }
            }
            
            pointText.text = playerAbilityLevelData.Name + " : " + _pointsUsed + playerAbilityLevelData.CurrentLevel;
        }

        private void RemovePoint(PlayerAbilityLevelData playerAbilityLevelData, TMP_Text pointText)
        {
            
            if ( _currentAbilityLevel == null || playerAbilityLevelData != _currentAbilityLevel)
            {
                _pointsUsed = 0;
                _currentAbilityLevel = playerAbilityLevelData;
            }

            if (_pointsUsed <= 0) return;
            
            _pointsUsed--;
            pointText.text = playerAbilityLevelData.Name + " : " + _pointsUsed;
        }

        public void ResetCurrentAbility()
        {
            _pointsUsed = 0;
            _currentAbilityLevel = null;
        }
        
        
        private void TabChanged(GameObject panel)
        {
            if (panel != gameObject)
                ResetCurrentAbility();
        }
    }
}