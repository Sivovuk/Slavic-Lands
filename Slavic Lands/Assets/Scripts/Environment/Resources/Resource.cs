using System;
using System.Collections.Generic;
using Core.Interfaces;
using Data;
using Gameplay.Dungeon;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Resources
{
    /// <summary>
    /// Enum representing different types of collectible or craftable resources in the game.
    /// </summary>
    public enum ResourceType
    {
        Wood,
        Stone,
        Hide,
        Food,
        Coal,
        Iron,
        Gold,
        Crystal
    }

    /// <summary>
    /// Handles damage and interaction logic for a resource node (e.g., tree, rock, ore).
    /// Connects with ResourceSO for stat and drop configuration.
    /// </summary>
    public class Resource : MonoBehaviour, IDamageable, ILoadStatsResource
    {
        private float _maxHealth;
        private float _currentHealth;

        [field: SerializeField] public ResourceSO ResourceSO { get; private set; }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Initializes the resource with values from its ScriptableObject.
        /// </summary>
        private void Init()
        {
            LoadResourceData(ResourceSO);
        }

        /// <summary>
        /// Sets health values based on the resource’s data.
        /// </summary>
        public void LoadResourceData(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;

            // TODO: Add loading from saved data if needed
        }

        /// <summary>
        /// Returns the list of items this resource will drop upon destruction.
        /// </summary>
        public IReadOnlyList<ResourceData> GetResourceType()
        {
            return ResourceSO.ResourceData;
        }

        /// <summary>
        /// Applies damage to the resource if the correct tool is used.
        /// </summary>
        public void TakeDamage(float damage, ToolType toolType)
        {
            if (toolType != ResourceSO.ToolType) return;

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                HandleBreak();
            }
        }

        /// <summary>
        /// Handles what happens when the resource is destroyed (drops, XP reward, etc.).
        /// </summary>
        private void HandleBreak(Action callback = null)
        {
            var player = PlayerController.Instance;

            foreach (var resource in ResourceSO.ResourceData)
            {
                player.AddResource(resource.DropAmount, resource.ResourceType);
            }

            player.PlayerProfile.PlayerLevelData.AddXp(ResourceSO.XPReward);

            Destroy(gameObject);
        }

        /// <summary>
        /// Placeholder for knockback logic if resource needs to respond to force.
        /// </summary>
        public virtual void ApplyKnockback(Vector2 direction, float force)
        {
            // Optional: implement physics reaction if needed
        }

        public bool IsDead { get; }
    }

    /// <summary>
    /// Configuration for dungeon-based resource spawning rules.
    /// </summary>
    [System.Serializable]
    public class DungeonData
    {
        public DungeonLevels DungeonLevels;
        public int MaxNodeAmount;
    }
}
