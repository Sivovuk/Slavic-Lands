using System;
using Data.Items;
using Data.Shop;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Shop
{
    /// <summary>
    /// Core business logic for buying and selling items at vendors (Market, Smith, etc.).
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        public ShopSO CurrentShop { get; private set; }
        public bool IsShopOpen => CurrentShop != null;

        public event Action<ShopSO> OnShopOpened;
        public event Action OnShopClosed;
        public event Action<ItemSO, int, bool> OnTransactionCompleted; // Item, Amount, IsBuy

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public void OpenShop(ShopSO shop)
        {
            if (shop == null)
            {
                Debug.LogWarning("[ShopManager] Attempted to open null shop.");
                return;
            }

            CurrentShop = shop;
            OnShopOpened?.Invoke(CurrentShop);
        }

        public void CloseShop()
        {
            if (CurrentShop == null) return;

            CurrentShop = null;
            OnShopClosed?.Invoke();
        }

        public bool TryBuyItem(ShopItemEntry entry, int quantity, out string message)
        {
            message = string.Empty;

            if (entry == null || entry.Item == null)
            {
                message = "Nevažeći artikl.";
                return false;
            }

            if (!entry.CanBuy)
            {
                message = "Ovaj artikl se ne može kupiti.";
                return false;
            }

            if (quantity <= 0)
            {
                message = "Količina mora biti veća od 0.";
                return false;
            }

            if (!entry.IsUnlimited && entry.Stock < quantity)
            {
                message = "Nema dovoljno zaliha u trgovini.";
                return false;
            }

            var player = PlayerController.Instance;
            if (player == null)
            {
                message = "Igrač nije pronađen.";
                return false;
            }

            int unitPrice = entry.GetEffectiveBuyPrice();
            int totalPrice = unitPrice * quantity;

            int currentGold = player.PlayerResources.GetGold();
            if (currentGold < totalPrice)
            {
                message = $"Nedovoljno zlata! Potrebno: {totalPrice}, imaš: {currentGold}.";
                return false;
            }

            // Deduct Gold
            if (!player.TrySpendGold(totalPrice))
            {
                message = "Greška pri plaćanju zlata.";
                return false;
            }

            // Grant Item
            if (player.Inventory != null)
            {
                player.Inventory.AddItem(entry.Item, quantity);
            }
            else if (entry.Item.IsRawResource)
            {
                player.AddResource(quantity, entry.Item.ResourceType);
            }

            if (!entry.IsUnlimited)
            {
                entry.Stock -= quantity;
            }

            message = $"Uspješno kupljeno {quantity}x {entry.Item.ItemName} za {totalPrice} zlata.";
            OnTransactionCompleted?.Invoke(entry.Item, quantity, true);
            return true;
        }

        public bool TrySellItem(ShopItemEntry entry, int quantity, out string message)
        {
            message = string.Empty;

            if (entry == null || entry.Item == null)
            {
                message = "Nevažeći artikl.";
                return false;
            }

            if (!entry.CanSell)
            {
                message = "Trgovac ne otkupljuje ovaj artikl.";
                return false;
            }

            if (quantity <= 0)
            {
                message = "Količina mora biti veća od 0.";
                return false;
            }

            var player = PlayerController.Instance;
            if (player == null)
            {
                message = "Igrač nije pronađen.";
                return false;
            }

            // Check player inventory/resources
            int playerOwned = 0;
            if (entry.Item.IsRawResource)
            {
                playerOwned = player.PlayerResources.GetResource(entry.Item.ResourceType);
            }
            else if (player.Inventory != null)
            {
                playerOwned = player.Inventory.GetItemCount(entry.Item);
            }

            if (playerOwned < quantity)
            {
                message = $"Nemaš dovoljno {entry.Item.ItemName} za prodaju (posjeduješ: {playerOwned}).";
                return false;
            }

            // Remove Item
            bool removed = false;
            if (entry.Item.IsRawResource)
            {
                removed = player.TrySpendResource(entry.Item.ResourceType, quantity);
            }
            else if (player.Inventory != null)
            {
                removed = player.Inventory.RemoveItem(entry.Item, quantity);
            }

            if (!removed)
            {
                message = "Greška pri uklanjanju predmeta iz inventara.";
                return false;
            }

            int unitPrice = entry.GetEffectiveSellPrice();
            int totalGain = unitPrice * quantity;

            // Add Gold
            player.PlayerResources.AddGold(totalGain);
            player.SavePlayer();

            if (!entry.IsUnlimited)
            {
                entry.Stock += quantity;
            }

            message = $"Uspješno prodano {quantity}x {entry.Item.ItemName} za {totalGain} zlata.";
            OnTransactionCompleted?.Invoke(entry.Item, quantity, false);
            return true;
        }
    }
}
