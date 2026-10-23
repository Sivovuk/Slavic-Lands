using Core.Interfaces;
using UnityEngine;

namespace Data.Items
{
    public enum EquipmentSlot
    {
        Weapon,
        Tool,
        Head,
        Chest,
        Shield,
        Accessory
    }

    /// <summary>
    /// Extended item data for weapons, tools, and armor pieces with combat and utility stats.
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Items/Equipment Data")]
    public class EquipmentItemSO : ItemSO
    {
        [Header("Equipment Details")]
        [SerializeField] private EquipmentSlot _slot = EquipmentSlot.Weapon;
        [SerializeField] private ToolType _toolType = ToolType.None;

        [Header("Combat & Utility Stats")]
        [SerializeField] private float _damageBonus;
        [SerializeField] private float _defenseBonus;
        [SerializeField] private float _movementSpeedModifier;
        [SerializeField] private float _harvestPower = 1f;
        [SerializeField] private float _durability = 100f;

        public EquipmentSlot Slot => _slot;
        public ToolType ToolType => _toolType;
        public float DamageBonus => _damageBonus;
        public float DefenseBonus => _defenseBonus;
        public float MovementSpeedModifier => _movementSpeedModifier;
        public float HarvestPower => _harvestPower;
        public float Durability => _durability;
    }
}
