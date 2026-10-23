using System;
using System.Collections.Generic;
using Data.Items;
using Gameplay.Resources;
using UnityEngine;

namespace Data.Crafting
{
    [Serializable]
    public class RecipeIngredient
    {
        [Tooltip("If checked, uses ResourceType instead of ItemSO.")]
        public bool UseRawResourceType;
        public ResourceType RawResource;
        public ItemSO Item;
        [Min(1)]
        public int Amount = 1;

        public string GetDisplayName()
        {
            if (UseRawResourceType)
                return RawResource.ToString();
            return Item != null ? Item.ItemName : "None";
        }

        public Sprite GetIcon()
        {
            if (!UseRawResourceType && Item != null)
                return Item.Icon;
            return null;
        }
    }

    /// <summary>
    /// Defines a crafting or smelting recipe (e.g. 2 Iron Ore + 4 Coal = 1 Iron Ingot).
    /// </summary>
    [CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/Recipe")]
    public class CraftingRecipeSO : ScriptableObject
    {
        [Header("Recipe Information")]
        [SerializeField] private string _recipeName;
        [TextArea(2, 3)]
        [SerializeField] private string _description;

        [Header("Ingredients (Required)")]
        [SerializeField] private List<RecipeIngredient> _ingredients = new List<RecipeIngredient>();

        [Header("Result (Output)")]
        [SerializeField] private ItemSO _resultItem;
        [Min(1)]
        [SerializeField] private int _resultAmount = 1;

        [Header("Additional Costs")]
        [Tooltip("Optional gold fee for blacksmith to craft this.")]
        [SerializeField] private int _goldCost = 0;

        public string RecipeName => string.IsNullOrEmpty(_recipeName) && _resultItem != null ? _resultItem.ItemName : _recipeName;
        public string Description => _description;
        public IReadOnlyList<RecipeIngredient> Ingredients => _ingredients;
        public ItemSO ResultItem => _resultItem;
        public int ResultAmount => _resultAmount;
        public int GoldCost => _goldCost;
    }
}
