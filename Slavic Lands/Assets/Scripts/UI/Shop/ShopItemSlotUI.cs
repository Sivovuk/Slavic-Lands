using System;
using Data.Items;
using Data.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    /// <summary>
    /// UI slot representing a single buyable or sellable item in the shop grid.
    /// </summary>
    public class ShopItemSlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Button _selectButton;
        [SerializeField] private GameObject _selectedBorder;

        public ShopItemEntry Entry { get; private set; }
        public bool IsBuyMode { get; private set; }

        private Action<ShopItemSlotUI> _onClickedCallback;

        private void Awake()
        {
            if (_selectButton != null)
            {
                _selectButton.onClick.AddListener(HandleClicked);
            }
        }

        public void Setup(ShopItemEntry entry, bool isBuyMode, int playerOwnedCount, Action<ShopItemSlotUI> onClicked)
        {
            Entry = entry;
            IsBuyMode = isBuyMode;
            _onClickedCallback = onClicked;

            if (entry == null || entry.Item == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_iconImage != null)
            {
                _iconImage.sprite = entry.Item.Icon;
                _iconImage.enabled = entry.Item.Icon != null;
            }

            if (_nameText != null)
            {
                _nameText.text = entry.Item.ItemName;
            }

            int price = isBuyMode ? entry.GetEffectiveBuyPrice() : entry.GetEffectiveSellPrice();
            if (_priceText != null)
            {
                _priceText.text = $"{price}g";
            }

            if (_countText != null)
            {
                if (isBuyMode)
                {
                    _countText.text = entry.IsUnlimited ? "Besk." : $"Zalihe: {entry.Stock}";
                }
                else
                {
                    _countText.text = $"Imaš: {playerOwnedCount}";
                }
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
