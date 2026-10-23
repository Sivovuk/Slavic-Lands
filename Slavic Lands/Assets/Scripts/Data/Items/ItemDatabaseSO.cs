using System.Collections.Generic;
using UnityEngine;

namespace Data.Items
{
    /// <summary>
    /// Global catalog of all ItemSO assets in the game.
    /// Used by inventory and save systems to reliably resolve item IDs at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
    public class ItemDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<ItemSO> _allItems = new List<ItemSO>();

        private readonly Dictionary<string, ItemSO> _itemLookup = new Dictionary<string, ItemSO>();
        private bool _isInitialized;

        public IReadOnlyList<ItemSO> AllItems => _allItems;

        public void RegisterItem(ItemSO item)
        {
            if (item != null && !_allItems.Contains(item))
            {
                _allItems.Add(item);
            }
        }

        private void InitializeLookup()
        {
            if (_isInitialized) return;
            _itemLookup.Clear();
            foreach (var item in _allItems)
            {
                if (item != null && !string.IsNullOrEmpty(item.ItemId))
                {
                    _itemLookup[item.ItemId] = item;
                }
            }
            _isInitialized = true;
        }

        public ItemSO GetItemById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            InitializeLookup();
            return _itemLookup.TryGetValue(itemId, out var item) ? item : null;
        }
    }
}
