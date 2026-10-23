using System.Collections.Generic;
using System.Text;
using Data.Crafting;
using Data.Items;
using Data.Shop;
using Gameplay.Crafting;
using Gameplay.Player;
using Gameplay.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    public enum ShopTabMode
    {
        Buy,
        Sell,
        Craft
    }

    /// <summary>
    /// Master UI controller managing the unified Shop and Crafting interface for Market and Blacksmith.
    /// </summary>
    public class ShopUIController : MonoBehaviour
    {
        public static ShopUIController Instance { get; private set; }

        [Header("Main Panel")]
        [SerializeField] private GameObject _panelRoot;

        [Header("Header")]
        [SerializeField] private TMP_Text _shopTitleText;
        [SerializeField] private TMP_Text _playerGoldText;
        [SerializeField] private Button _closeButton;

        [Header("Navigation Tabs")]
        [SerializeField] private Button _tabBuyButton;
        [SerializeField] private Button _tabSellButton;
        [SerializeField] private Button _tabCraftButton;

        [Header("Grid List")]
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private ShopItemSlotUI _itemSlotPrefab;
        [SerializeField] private ShopRecipeSlotUI _recipeSlotPrefab;

        [Header("Selection & Details Panel")]
        [SerializeField] private GameObject _detailPanelRoot;
        [SerializeField] private Image _detailIconImage;
        [SerializeField] private TMP_Text _detailTitleText;
        [SerializeField] private TMP_Text _detailDescriptionText;
        [SerializeField] private TMP_Text _detailRequirementsText;
        [SerializeField] private TMP_Text _detailTotalCostText;

        [Header("Quantity Controls")]
        [SerializeField] private GameObject _quantityControlsRoot;
        [SerializeField] private Button _decreaseQtyButton;
        [SerializeField] private Button _increaseQtyButton;
        [SerializeField] private TMP_Text _quantityText;

        [Header("Action Execution")]
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionButtonText;
        [SerializeField] private TMP_Text _statusMessageText;

        private ShopTabMode _currentTab = ShopTabMode.Buy;
        private ShopSO _activeShop;
        private ShopItemEntry _selectedItemEntry;
        private CraftingRecipeSO _selectedRecipe;
        private int _currentQuantity = 1;

        private readonly List<GameObject> _spawnedSlots = new List<GameObject>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            if (_panelRoot != null)
                _panelRoot.SetActive(false);

            if (_closeButton != null)
                _closeButton.onClick.AddListener(CloseShopUI);

            if (_tabBuyButton != null)
                _tabBuyButton.onClick.AddListener(() => SetTab(ShopTabMode.Buy));

            if (_tabSellButton != null)
                _tabSellButton.onClick.AddListener(() => SetTab(ShopTabMode.Sell));

            if (_tabCraftButton != null)
                _tabCraftButton.onClick.AddListener(() => SetTab(ShopTabMode.Craft));

            if (_decreaseQtyButton != null)
                _decreaseQtyButton.onClick.AddListener(DecreaseQuantity);

            if (_increaseQtyButton != null)
                _increaseQtyButton.onClick.AddListener(IncreaseQuantity);

            if (_actionButton != null)
                _actionButton.onClick.AddListener(ExecuteAction);
        }

        private void Start()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopOpened += OpenShopUI;
                ShopManager.Instance.OnShopClosed += CloseShopUI;
                ShopManager.Instance.OnTransactionCompleted += HandleTransactionCompleted;
            }

            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftCompleted += HandleCraftCompleted;
            }
        }

        private void OnDestroy()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopOpened -= OpenShopUI;
                ShopManager.Instance.OnShopClosed -= CloseShopUI;
                ShopManager.Instance.OnTransactionCompleted -= HandleTransactionCompleted;
            }

            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftCompleted -= HandleCraftCompleted;
            }
        }

        private void Update()
        {
            if (_panelRoot != null && _panelRoot.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseShopUI();
                }
            }
        }

        public void OpenShopUI(ShopSO shop)
        {
            _activeShop = shop;
            if (_activeShop == null) return;

            if (_panelRoot != null)
                _panelRoot.SetActive(true);

            if (_shopTitleText != null)
                _shopTitleText.text = _activeShop.ShopTitle;

            if (_tabCraftButton != null)
                _tabCraftButton.gameObject.SetActive(_activeShop.SupportsCrafting);

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(true);
            }

            ClearStatus();
            SetTab(ShopTabMode.Buy);
        }

        public void CloseShopUI()
        {
            if (_panelRoot != null)
                _panelRoot.SetActive(false);

            _activeShop = null;
            _selectedItemEntry = null;
            _selectedRecipe = null;

            if (ShopManager.Instance != null && ShopManager.Instance.IsShopOpen)
            {
                ShopManager.Instance.CloseShop();
            }

            if (PlayerController.Instance != null && PlayerController.Instance.PlayerMovement != null)
            {
                PlayerController.Instance.PlayerMovement.SetMovementLock(false);
            }
        }

        public void SetTab(ShopTabMode mode)
        {
            _currentTab = mode;
            _selectedItemEntry = null;
            _selectedRecipe = null;
            _currentQuantity = 1;

            UpdateGoldDisplay();
            RefreshGrid();
            RefreshDetails();
        }

        private void UpdateGoldDisplay()
        {
            if (_playerGoldText != null)
            {
                int gold = PlayerController.Instance != null && PlayerController.Instance.PlayerResources != null
                    ? PlayerController.Instance.PlayerResources.GetGold()
                    : 0;
                _playerGoldText.text = $"Zlato: {gold}g";
            }
        }

        private void RefreshGrid()
        {
            foreach (var slot in _spawnedSlots)
            {
                Destroy(slot);
            }
            _spawnedSlots.Clear();

            if (_activeShop == null || _itemsContainer == null) return;

            if (_currentTab == ShopTabMode.Craft)
            {
                // Populate recipes
                if (_recipeSlotPrefab == null) return;

                foreach (var recipe in _activeShop.CraftingRecipes)
                {
                    if (recipe == null) continue;

                    ShopRecipeSlotUI slot = Instantiate(_recipeSlotPrefab, _itemsContainer);
                    slot.Setup(recipe, SelectRecipe);
                    _spawnedSlots.Add(slot.gameObject);
                }
            }
            else
            {
                // Populate items (Buy or Sell)
                if (_itemSlotPrefab == null) return;

                bool isBuy = _currentTab == ShopTabMode.Buy;
                var player = PlayerController.Instance;

                foreach (var entry in _activeShop.Items)
                {
                    if (entry == null || entry.Item == null) continue;

                    if (isBuy && !entry.CanBuy) continue;
                    if (!isBuy && !entry.CanSell) continue;

                    int owned = 0;
                    if (player != null)
                    {
                        if (entry.Item.IsRawResource && player.PlayerResources != null)
                            owned = player.PlayerResources.GetResource(entry.Item.ResourceType);
                        else if (player.Inventory != null)
                            owned = player.Inventory.GetItemCount(entry.Item);
                    }

                    ShopItemSlotUI slot = Instantiate(_itemSlotPrefab, _itemsContainer);
                    slot.Setup(entry, isBuy, owned, SelectItem);
                    _spawnedSlots.Add(slot.gameObject);
                }
            }
        }

        private void SelectItem(ShopItemSlotUI slot)
        {
            _selectedItemEntry = slot.Entry;
            _selectedRecipe = null;
            _currentQuantity = 1;

            // Highlight in grid
            foreach (var s in _itemsContainer.GetComponentsInChildren<ShopItemSlotUI>())
            {
                s.SetSelected(s == slot);
            }

            RefreshDetails();
        }

        private void SelectRecipe(ShopRecipeSlotUI slot)
        {
            _selectedRecipe = slot.Recipe;
            _selectedItemEntry = null;
            _currentQuantity = 1;

            // Highlight in grid
            foreach (var s in _itemsContainer.GetComponentsInChildren<ShopRecipeSlotUI>())
            {
                s.SetSelected(s == slot);
            }

            RefreshDetails();
        }

        private void RefreshDetails()
        {
            if (_selectedItemEntry == null && _selectedRecipe == null)
            {
                if (_detailPanelRoot != null)
                    _detailPanelRoot.SetActive(false);
                return;
            }

            if (_detailPanelRoot != null)
                _detailPanelRoot.SetActive(true);

            if (_quantityText != null)
                _quantityText.text = _currentQuantity.ToString();

            if (_quantityControlsRoot != null)
                _quantityControlsRoot.SetActive(true);

            if (_selectedItemEntry != null)
            {
                var item = _selectedItemEntry.Item;
                if (_detailIconImage != null)
                {
                    _detailIconImage.sprite = item.Icon;
                    _detailIconImage.enabled = item.Icon != null;
                }

                if (_detailTitleText != null)
                    _detailTitleText.text = item.ItemName;

                if (_detailDescriptionText != null)
                    _detailDescriptionText.text = item.Description;

                if (_detailRequirementsText != null)
                    _detailRequirementsText.text = $"Kategorija: {item.Category}";

                if (_currentTab == ShopTabMode.Buy)
                {
                    int total = _selectedItemEntry.GetEffectiveBuyPrice() * _currentQuantity;
                    if (_detailTotalCostText != null)
                        _detailTotalCostText.text = $"Ukupna cijena: {total} zlata";

                    if (_actionButtonText != null)
                        _actionButtonText.text = $"Kupi ({total}g)";
                }
                else
                {
                    int total = _selectedItemEntry.GetEffectiveSellPrice() * _currentQuantity;
                    if (_detailTotalCostText != null)
                        _detailTotalCostText.text = $"Ukupna zarada: {total} zlata";

                    if (_actionButtonText != null)
                        _actionButtonText.text = $"Prodaj (+{total}g)";
                }
            }
            else if (_selectedRecipe != null)
            {
                var resultItem = _selectedRecipe.ResultItem;
                if (_detailIconImage != null)
                {
                    _detailIconImage.sprite = resultItem.Icon;
                    _detailIconImage.enabled = resultItem.Icon != null;
                }

                if (_detailTitleText != null)
                    _detailTitleText.text = $"{_selectedRecipe.RecipeName} x({_selectedRecipe.ResultAmount * _currentQuantity})";

                if (_detailDescriptionText != null)
                    _detailDescriptionText.text = _selectedRecipe.Description;

                // Ingredients check
                var sb = new StringBuilder("Potrebni materijali:\n");
                var player = PlayerController.Instance;

                foreach (var ing in _selectedRecipe.Ingredients)
                {
                    int needed = ing.Amount * _currentQuantity;
                    int owned = 0;
                    if (player != null)
                    {
                        if (ing.UseRawResourceType && player.PlayerResources != null)
                            owned = player.PlayerResources.GetResource(ing.RawResource);
                        else if (ing.Item != null)
                        {
                            if (ing.Item.IsRawResource && player.PlayerResources != null)
                                owned = player.PlayerResources.GetResource(ing.Item.ResourceType);
                            else if (player.Inventory != null)
                                owned = player.Inventory.GetItemCount(ing.Item);
                        }
                    }

                    string statusColor = owned >= needed ? "<color=#44FF44>" : "<color=#FF4444>";
                    sb.AppendLine($" - {ing.GetDisplayName()}: {statusColor}{owned}/{needed}</color>");
                }

                if (_detailRequirementsText != null)
                    _detailRequirementsText.text = sb.ToString();

                int totalGoldCost = _selectedRecipe.GoldCost * _currentQuantity;
                if (_detailTotalCostText != null)
                {
                    _detailTotalCostText.text = totalGoldCost > 0 ? $"Cijena kovanja: {totalGoldCost} zlata" : "Besplatno kovanje";
                }

                if (_actionButtonText != null)
                    _actionButtonText.text = "Izradi (Craft)";
            }
        }

        private void DecreaseQuantity()
        {
            if (_currentQuantity > 1)
            {
                _currentQuantity--;
                RefreshDetails();
            }
        }

        private void IncreaseQuantity()
        {
            _currentQuantity++;
            RefreshDetails();
        }

        private void ExecuteAction()
        {
            string message = string.Empty;
            bool success = false;

            if (_currentTab == ShopTabMode.Buy && _selectedItemEntry != null)
            {
                if (ShopManager.Instance != null)
                {
                    success = ShopManager.Instance.TryBuyItem(_selectedItemEntry, _currentQuantity, out message);
                }
            }
            else if (_currentTab == ShopTabMode.Sell && _selectedItemEntry != null)
            {
                if (ShopManager.Instance != null)
                {
                    success = ShopManager.Instance.TrySellItem(_selectedItemEntry, _currentQuantity, out message);
                }
            }
            else if (_currentTab == ShopTabMode.Craft && _selectedRecipe != null)
            {
                if (CraftingManager.Instance != null)
                {
                    success = CraftingManager.Instance.TryCraftRecipe(_selectedRecipe, _currentQuantity, out message);
                }
            }

            SetStatus(message, success);
            UpdateGoldDisplay();
            RefreshGrid();
            RefreshDetails();
        }

        private void HandleTransactionCompleted(ItemSO item, int amount, bool isBuy)
        {
            UpdateGoldDisplay();
            RefreshGrid();
            RefreshDetails();
        }

        private void HandleCraftCompleted(CraftingRecipeSO recipe, int amount)
        {
            UpdateGoldDisplay();
            RefreshGrid();
            RefreshDetails();
        }

        private void SetStatus(string message, bool isSuccess)
        {
            if (_statusMessageText != null)
            {
                string color = isSuccess ? "#44FF44" : "#FF6666";
                _statusMessageText.text = $"<color={color}>{message}</color>";
            }
        }

        private void ClearStatus()
        {
            if (_statusMessageText != null)
                _statusMessageText.text = string.Empty;
        }
    }
}
