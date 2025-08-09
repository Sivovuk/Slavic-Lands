using System;
using System.Collections.Generic;
using Core.Interfaces;
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

        [Header("Skill UI Blocks")]
        [SerializeField] private SkillUIBlock _playerLevelBlock;
        [SerializeField] private SkillUIBlock _attackBlock;
        [SerializeField] private SkillUIBlock _shootBlock;
        [SerializeField] private SkillUIBlock _cutBlock;
        [SerializeField] private SkillUIBlock _mineBlock;

        [Header("Abilities")]
        [SerializeField] private PlayerProfileAbilityUIElement _abilitySlash;
        [SerializeField] private PlayerProfileAbilityUIElement _abilityShieldBash;
        [SerializeField] private PlayerProfileAbilityUIElement _abilityPiercingArrow;

        [Header("Points UI")]
        [SerializeField] private TMP_Text _levelPoints;
        [SerializeField] private Button _savePoints;

        private List<PlayerAbilityLevelData> _currentAbilities = new();
        private int _pointsUsed = 0;

        public Action<GameObject> OnTabChange;

        private void OnEnable()
        {
            GameManager.Instance.OnPlayerInit += OnPlayerInit;
            OnTabChange += TabChanged;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnPlayerInit -= OnPlayerInit;
            OnTabChange -= TabChanged;

            PlayerController.Instance.PlayerProfile.PlayerLevelData.OnXpChanged -= _ => UpdateLevelPoints();
            PlayerController.Instance.PlayerProfile.PlayerLevelData.OnPointsChanged -= UpdateLevelPoints;

            _savePoints.onClick.RemoveListener(SavePoints);
            ResetCurrentAbility();
        }

        private void OnPlayerInit()
        {
            var profile = PlayerController.Instance.PlayerProfile;

            _playerLevelBlock.Init(profile.PlayerLevelData);
            profile.PlayerLevelData.OnPointsChanged += UpdateLevelPoints;

            _attackBlock.Init(profile.GetLevelData(ToolType.BattleAxe));
            _shootBlock.Init(profile.GetLevelData(ToolType.Bow));
            _cutBlock.Init(profile.GetLevelData(ToolType.Axe));
            _mineBlock.Init(profile.GetLevelData(ToolType.Pickaxe));

            UpdateAbilitiesUI();

            _savePoints.onClick.AddListener(SavePoints);
            UpdateLevelPoints();
        }

        public void UpdateAbilitiesUI()
        {
            var profile = PlayerController.Instance.PlayerProfile;

            _abilitySlash.OnInit(profile.AbilityMap[ToolType.Slashed], this);
            _abilityShieldBash.OnInit(profile.AbilityMap[ToolType.ShieldBash], this);
            _abilityPiercingArrow.OnInit(profile.AbilityMap[ToolType.PiercingArrow], this);
        }

        private void UpdateLevelPoints()
        {
            int available = PlayerController.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable - _pointsUsed;
            _levelPoints.text = $"Points: {available}";
        }

        private void SavePoints()
        {
            var profile = PlayerController.Instance.PlayerProfile;

            foreach (var ability in _currentAbilities)
                profile.SaveNewAbilityLevelData(ability);

            _pointsUsed = 0;
            UpdateLevelPoints();
        }

        public PlayerAbilityLevelData AddPoint(PlayerAbilityLevelData ability)
        {
            if (PlayerController.Instance.PlayerProfile.PlayerLevelData.LevelPointsAvailable - _pointsUsed <= 0 ||
                ability.CurrentLevel >= 10)
                return ability;

            var existing = _currentAbilities.Find(a => a.ToolType == ability.ToolType);
            if (existing != null)
            {
                existing.CurrentLevel++;
                _pointsUsed++;
                UpdateLevelPoints();
                return existing;
            }

            var newAbility = new PlayerAbilityLevelData(ability.ToolType, ability.CurrentLevel, ability.LevelMultiplier)
            {
                CurrentLevel = ability.CurrentLevel + 1
            };
            _currentAbilities.Add(newAbility);
            _pointsUsed++;
            UpdateLevelPoints();
            return newAbility;
        }

        public PlayerAbilityLevelData RemovePoint(PlayerAbilityLevelData ability)
        {
            if (_pointsUsed <= 0) return ability;

            var existing = _currentAbilities.Find(a => a.ToolType == ability.ToolType);
            if (existing != null)
            {
                existing.CurrentLevel = Math.Max(0, existing.CurrentLevel - 1);
                _pointsUsed--;
                UpdateLevelPoints();
                return existing;
            }

            return ability;
        }

        public void ResetCurrentAbility()
        {
            _pointsUsed = 0;
            UpdateLevelPoints();
            _currentAbilities.Clear();

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
