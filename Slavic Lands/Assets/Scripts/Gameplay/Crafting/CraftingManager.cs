using System;
using Data.Crafting;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Crafting
{
    /// <summary>
    /// Evaluates and handles crafting / smelting recipes (e.g. 2 Iron Ore + 4 Coal = 1 Iron Ingot).
    /// </summary>
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

        public event Action<CraftingRecipeSO, int> OnCraftCompleted;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public bool CanCraft(CraftingRecipeSO recipe, int quantity, out string message)
        {
            message = string.Empty;

            if (recipe == null || recipe.ResultItem == null)
            {
                message = "Nevažeći recept.";
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

            // Check Gold cost if any
            int totalGoldCost = recipe.GoldCost * quantity;
            if (totalGoldCost > 0 && player.PlayerResources.GetGold() < totalGoldCost)
            {
                message = $"Nedovoljno zlata za izradu! Potrebno: {totalGoldCost} zlata.";
                return false;
            }

            // Check ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                int needed = ingredient.Amount * quantity;
                int owned = 0;

                if (ingredient.UseRawResourceType)
                {
                    owned = player.PlayerResources.GetResource(ingredient.RawResource);
                }
                else if (ingredient.Item != null)
                {
                    if (ingredient.Item.IsRawResource)
                    {
                        owned = player.PlayerResources.GetResource(ingredient.Item.ResourceType);
                    }
                    else if (player.Inventory != null)
                    {
                        owned = player.Inventory.GetItemCount(ingredient.Item);
                    }
                }

                if (owned < needed)
                {
                    string ingName = ingredient.GetDisplayName();
                    message = $"Nedostaje materijala: {ingName} (Imaš: {owned}/{needed}).";
                    return false;
                }
            }

            return true;
        }

        public bool TryCraftRecipe(CraftingRecipeSO recipe, int quantity, out string message)
        {
            if (!CanCraft(recipe, quantity, out message))
            {
                return false;
            }

            var player = PlayerController.Instance;
            int totalGoldCost = recipe.GoldCost * quantity;

            // Spend gold fee if any
            if (totalGoldCost > 0)
            {
                player.TrySpendGold(totalGoldCost);
            }

            // Consume all ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                int needed = ingredient.Amount * quantity;

                if (ingredient.UseRawResourceType)
                {
                    player.TrySpendResource(ingredient.RawResource, needed);
                }
                else if (ingredient.Item != null)
                {
                    if (ingredient.Item.IsRawResource)
                    {
                        player.TrySpendResource(ingredient.Item.ResourceType, needed);
                    }
                    else if (player.Inventory != null)
                    {
                        player.Inventory.RemoveItem(ingredient.Item, needed);
                    }
                }
            }

            // Grant result item
            int outputCount = recipe.ResultAmount * quantity;
            if (player.Inventory != null)
            {
                player.Inventory.AddItem(recipe.ResultItem, outputCount);
            }
            else if (recipe.ResultItem.IsRawResource)
            {
                player.AddResource(outputCount, recipe.ResultItem.ResourceType);
            }

            player.SavePlayer();

            message = $"Uspješno izrađeno {outputCount}x {recipe.ResultItem.ItemName}!";
            OnCraftCompleted?.Invoke(recipe, quantity);
            return true;
        }
    }
}
