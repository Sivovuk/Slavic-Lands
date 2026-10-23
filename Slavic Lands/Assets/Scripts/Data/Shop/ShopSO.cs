using System;
using System.Collections.Generic;
using Data.Crafting;
using Data.Items;
using UnityEngine;

namespace Data.Shop
{
    public enum ShopType
    {
        Market,
        Blacksmith,
        General
    }

    [Serializable]
    public class ShopItemEntry
    {
        public ItemSO Item;
        public bool OverridePrices;
        [Min(0)]
        public int BuyPrice;
        [Min(0)]
        public int SellPrice;
        public bool CanBuy = true;
        public bool CanSell = true;
        public bool IsUnlimited = true;
        [Min(0)]
        public int Stock = 10;

        public int GetEffectiveBuyPrice()
        {
            if (OverridePrices) return BuyPrice;
            return Item != null ? Item.BuyPrice : BuyPrice;
        }

        public int GetEffectiveSellPrice()
        {
            if (OverridePrices) return SellPrice;
            return Item != null ? Item.SellPrice : SellPrice;
        }
    }

    /// <summary>
    /// ScriptableObject defining shop inventory, pricing, and available crafting recipes for a vendor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Shop", menuName = "Shop/Shop Data")]
    public class ShopSO : ScriptableObject
    {
        [Header("Shop Profile")]
        [SerializeField] private string _shopTitle = "Shop";
        [SerializeField] private ShopType _shopType = ShopType.General;
        [TextArea(2, 3)]
        [SerializeField] private string _greetingMessage = "Welcome! Take a look at my wares.";

        [Header("Goods Available For Buy/Sell")]
        [SerializeField] private List<ShopItemEntry> _items = new List<ShopItemEntry>();

        [Header("Crafting / Smelting (e.g. for Blacksmith)")]
        [SerializeField] private bool _supportsCrafting;
        [SerializeField] private List<CraftingRecipeSO> _craftingRecipes = new List<CraftingRecipeSO>();

        public string ShopTitle => _shopTitle;
        public ShopType ShopType => _shopType;
        public string GreetingMessage => _greetingMessage;
        public IReadOnlyList<ShopItemEntry> Items => _items;
        public bool SupportsCrafting => _supportsCrafting;
        public IReadOnlyList<CraftingRecipeSO> CraftingRecipes => _craftingRecipes;
    }
}
