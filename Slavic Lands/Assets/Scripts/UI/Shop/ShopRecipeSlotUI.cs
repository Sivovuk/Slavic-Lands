using System;
using Data.Crafting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    /// <summary>
    /// UI slot representing a craftable recipe (e.g. Iron Ingot from 2 Iron + 4 Coal) in the blacksmith crafting tab.
    /// </summary>
    public class ShopRecipeSlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _resultIconImage;
        [SerializeField] private TMP_Text _recipeNameText;
        [SerializeField] private TMP_Text _ingredientsSummaryText;
        [SerializeField] private Button _selectButton;
        [SerializeField] private GameObject _selectedBorder;

        public CraftingRecipeSO Recipe { get; private set; }

        private Action<ShopRecipeSlotUI> _onClickedCallback;

        private void Awake()
        {
            if (_selectButton != null)
            {
                _selectButton.onClick.AddListener(HandleClicked);
            }
        }

        public void Setup(CraftingRecipeSO recipe, Action<ShopRecipeSlotUI> onClicked)
        {
            Recipe = recipe;
            _onClickedCallback = onClicked;

            if (recipe == null || recipe.ResultItem == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_resultIconImage != null)
            {
                _resultIconImage.sprite = recipe.ResultItem.Icon;
                _resultIconImage.enabled = recipe.ResultItem.Icon != null;
            }

            if (_recipeNameText != null)
            {
                _recipeNameText.text = recipe.RecipeName;
            }

            if (_ingredientsSummaryText != null)
            {
                var parts = new System.Collections.Generic.List<string>();
                foreach (var ing in recipe.Ingredients)
                {
                    parts.Add($"{ing.Amount}x {ing.GetDisplayName()}");
                }
                _ingredientsSummaryText.text = string.Join(" + ", parts);
            }

            SetSelected(false);
        }

        public void SetSelected(bool isSelected)
        {
            if (_selectedBorder != null)
            {
                _selectedBorder.SetActive(isSelected);
            }
        }

        private void HandleClicked()
        {
            _onClickedCallback?.Invoke(this);
        }
    }
}
