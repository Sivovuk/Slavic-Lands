using Gameplay.Resources;
using UnityEngine;

namespace Data.Items
{
    public enum ItemCategory
    {
        RawResource,
        Material,
        Weapon,
        Tool,
        Armor,
        Consumable,
        Misc
    }

    /// <summary>
    /// Base ScriptableObject for all game items, raw resources, refined ingots, tools, weapons and armor.
    /// Highly extensible for future items, crafting, and shops.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Item Data")]
    public class ItemSO : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string _itemId;
        [SerializeField] private string _itemName;
        [TextArea(2, 4)]
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private ItemCategory _category = ItemCategory.Misc;

        [Header("Economy")]
        [Tooltip("Base purchase price in gold when buying from a vendor.")]
        [SerializeField] private int _buyPrice = 10;
        [Tooltip("Base sell price in gold when selling to a vendor.")]
        [SerializeField] private int _sellPrice = 5;

        [Header("Stacking")]
        [SerializeField] private bool _isStackable = true;
        [SerializeField] private int _maxStackSize = 99;

        [Header("Raw Resource Mapping (Optional)")]
        [Tooltip("Check this if this item corresponds to an existing ResourceType in the game.")]
        [SerializeField] private bool _isRawResource;
        [SerializeField] private ResourceType _resourceType;

        public string ItemId => string.IsNullOrEmpty(_itemId) ? name : _itemId;
        public string ItemName => string.IsNullOrEmpty(_itemName) ? name : _itemName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public ItemCategory Category => _category;
        public int BuyPrice => _buyPrice;
        public int SellPrice => _sellPrice;
        public bool IsStackable => _isStackable;
        public int MaxStackSize => _maxStackSize;
        public bool IsRawResource => _isRawResource;
        public ResourceType ResourceType => _resourceType;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_itemId))
            {
                _itemId = name;
            }
        }
    }
}
