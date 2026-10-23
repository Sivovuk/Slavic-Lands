using System;
using System.Collections.Generic;
using Data.Items;
using Gameplay.Player;
using Gameplay.Resources;
using SaveSystem;
using UnityEngine;

namespace Gameplay.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemSO Item;
        public int Amount;

        public InventorySlot(ItemSO item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }

    /// <summary>
    /// Manages items, equipment, ingots, and crafted goods owned by the player.
    /// Supports automatic saving and synchronization with PlayerController.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Database Reference")]
        [SerializeField] private ItemDatabaseSO _itemDatabase;

        [Header("Runtime Inventory Items")]
        [SerializeField] private List<InventorySlot> _slots = new List<InventorySlot>();

        public event Action OnInventoryChanged;

        public IReadOnlyList<InventorySlot> Slots => _slots;
        public ItemDatabaseSO ItemDatabase => _itemDatabase;

        public void SetItemDatabase(ItemDatabaseSO database)
        {
            _itemDatabase = database;
        }

        public void AddItem(ItemSO item, int amount = 1)
        {
            if (item == null || amount <= 0) return;

            // If this item is a raw resource, also add to PlayerResource
            if (item.IsRawResource && PlayerController.Instance != null && PlayerController.Instance.PlayerResources != null)
            {
                PlayerController.Instance.AddResource(amount, item.ResourceType);
            }

            InventorySlot existing = _slots.Find(s => s.Item != null && s.Item.ItemId == item.ItemId);
            if (existing != null && item.IsStackable)
            {
                existing.Amount += amount;
            }
            else
            {
                _slots.Add(new InventorySlot(item, amount));
            }

            OnInventoryChanged?.Invoke();
            AutoSave();
        }

        public bool HasItem(ItemSO item, int amount = 1)
        {
            if (item == null) return false;
            if (amount <= 0) return true;

            // If raw resource, check PlayerResource directly
            if (item.IsRawResource && PlayerController.Instance != null && PlayerController.Instance.PlayerResources != null)
            {
                return PlayerController.Instance.PlayerResources.HasResource(item.ResourceType, amount);
            }

            InventorySlot existing = _slots.Find(s => s.Item != null && s.Item.ItemId == item.ItemId);
            return existing != null && existing.Amount >= amount;
        }

        public int GetItemCount(ItemSO item)
        {
            if (item == null) return 0;

            if (item.IsRawResource && PlayerController.Instance != null && PlayerController.Instance.PlayerResources != null)
            {
                return PlayerController.Instance.PlayerResources.GetResource(item.ResourceType);
            }

            InventorySlot existing = _slots.Find(s => s.Item != null && s.Item.ItemId == item.ItemId);
            return existing != null ? existing.Amount : 0;
        }

        public bool RemoveItem(ItemSO item, int amount = 1)
        {
            if (item == null || amount <= 0) return false;

            if (item.IsRawResource && PlayerController.Instance != null && PlayerController.Instance.PlayerResources != null)
            {
                return PlayerController.Instance.TrySpendResource(item.ResourceType, amount);
            }

            InventorySlot existing = _slots.Find(s => s.Item != null && s.Item.ItemId == item.ItemId);
            if (existing == null || existing.Amount < amount) return false;

            existing.Amount -= amount;
            if (existing.Amount <= 0)
            {
                _slots.Remove(existing);
            }

            OnInventoryChanged?.Invoke();
            AutoSave();
            return true;
        }

        private void AutoSave()
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SavePlayer();
            }
        }

        public List<InventoryItemSaveData> ToSaveData()
        {
            var list = new List<InventoryItemSaveData>();
            foreach (var slot in _slots)
            {
                if (slot.Item != null && slot.Amount > 0)
                {
                    list.Add(new InventoryItemSaveData
                    {
                        ItemId = slot.Item.ItemId,
                        Amount = slot.Amount
                    });
                }
            }
            return list;
        }

        public void LoadFromSaveData(IEnumerable<InventoryItemSaveData> data)
        {
            LoadFromSaveData(data, id => _itemDatabase != null ? _itemDatabase.GetItemById(id) : null);
        }

        public void LoadFromSaveData(IEnumerable<InventoryItemSaveData> data, Func<string, ItemSO> itemResolver)
        {
            _slots.Clear();
            if (data == null || itemResolver == null) return;

            foreach (var entry in data)
            {
                ItemSO resolved = itemResolver(entry.ItemId);
                if (resolved != null && entry.Amount > 0)
                {
                    _slots.Add(new InventorySlot(resolved, entry.Amount));
                }
            }

            OnInventoryChanged?.Invoke();
        }
    }
}
