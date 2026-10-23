using System.IO;
using Data.Shop;
using Entities;
using Gameplay.Crafting;
using Gameplay.Shop;
using TMPro;
using UI.Interaction;
using UI.Shop;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Editor
{
    public static class ShopUISceneSetup
    {
        private static TMP_FontAsset _fontAsset;
        private static Sprite _squareSprite;

        // Setup can be re-run anytime via MenuItem Tools -> Setup Shop Scene & UI
        [MenuItem("Tools/Setup Shop Scene & UI")]
        public static void SetupSceneAndUI()
        {
            if (!Directory.Exists("Assets/Prefabs/UI/Shop"))
            {
                Directory.CreateDirectory("Assets/Prefabs/UI/Shop");
            }

            _fontAsset = TMPro.TMP_Settings.defaultFontAsset;
            _squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");

            // 1. Create Prefabs for Slots
            ShopItemSlotUI itemSlotPrefab = CreateItemSlotPrefab("Assets/Prefabs/UI/Shop/ShopItemSlot.prefab");
            ShopRecipeSlotUI recipeSlotPrefab = CreateRecipeSlotPrefab("Assets/Prefabs/UI/Shop/ShopRecipeSlot.prefab");

            // 2. Open Village Scene if not open
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "Village")
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Village.unity");
            }

            // 3. Ensure Managers exist
            EnsureManagersInScene();

            // 4. Ensure Canvas and UI Elements exist
            EnsureCanvasUI(itemSlotPrefab, recipeSlotPrefab);

            // 5. Configure NPCs in Scene (Market & Blacksmith)
            ConfigureNPCsInScene();

            // 6. Save Scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();

            Debug.Log("[ShopUISceneSetup] Shop & Crafting UI and Scene setup completed successfully!");
        }

        private static void EnsureManagersInScene()
        {
            GameObject managersObj = GameObject.Find("ShopSystem");
            if (managersObj == null)
            {
                managersObj = new GameObject("ShopSystem");
                Undo.RegisterCreatedObjectUndo(managersObj, "Create ShopSystem");
            }

            if (managersObj.GetComponent<ShopManager>() == null)
            {
                managersObj.AddComponent<ShopManager>();
            }

            if (managersObj.GetComponent<CraftingManager>() == null)
            {
                managersObj.AddComponent<CraftingManager>();
            }
        }

        private static void EnsureCanvasUI(ShopItemSlotUI itemSlotPrefab, ShopRecipeSlotUI recipeSlotPrefab)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[ShopUISceneSetup] Canvas not found in scene!");
                return;
            }

            Transform uiRoot = canvas.transform.Find("UI");
            if (uiRoot == null)
            {
                uiRoot = canvas.transform;
            }

            // --- A. Create NPC Interaction Choice UI ---
            NPCInteractionChoiceUI choiceUI = Object.FindAnyObjectByType<NPCInteractionChoiceUI>();
            if (choiceUI == null)
            {
                GameObject choiceObj = new GameObject("NPCInteractionChoicePanel", typeof(RectTransform));
                choiceObj.transform.SetParent(uiRoot, false);
                choiceUI = choiceObj.AddComponent<NPCInteractionChoiceUI>();

                RectTransform choiceRect = choiceObj.GetComponent<RectTransform>();
                choiceRect.anchorMin = new Vector2(0.5f, 0.5f);
                choiceRect.anchorMax = new Vector2(0.5f, 0.5f);
                choiceRect.pivot = new Vector2(0.5f, 0.5f);
                choiceRect.sizeDelta = new Vector2(480, 280);

                Image bg = choiceObj.AddComponent<Image>();
                bg.color = new Color(0.12f, 0.12f, 0.15f, 0.95f);
                if (_squareSprite != null) bg.sprite = _squareSprite;

                // Name Text
                TMP_Text nameText = CreateText(choiceObj.transform, "NPCNameText", "Mještanin", 28, TextAlignmentOptions.Center, new Vector2(0, 85), new Vector2(440, 50));
                nameText.color = new Color(1f, 0.85f, 0.3f, 1f);

                // Prompt Text
                CreateText(choiceObj.transform, "PromptText", "Kako vam mogu pomoći?", 20, TextAlignmentOptions.Center, new Vector2(0, 45), new Vector2(440, 35));

                // Buttons
                Button talkBtn = CreateButton(choiceObj.transform, "BtnTalk", "1. Razgovor (Talk)", new Vector2(0, 0), new Vector2(380, 45), new Color(0.2f, 0.45f, 0.25f, 1f));
                Button shopBtn = CreateButton(choiceObj.transform, "BtnShop", "2. Trgovina (Shop)", new Vector2(0, -55), new Vector2(380, 45), new Color(0.25f, 0.35f, 0.6f, 1f));
                Button leaveBtn = CreateButton(choiceObj.transform, "BtnLeave", "3. Odlazak (Leave)", new Vector2(0, -110), new Vector2(380, 40), new Color(0.4f, 0.2f, 0.2f, 1f));

                var so = new SerializedObject(choiceUI);
                so.FindProperty("_panelRoot").objectReferenceValue = choiceObj;
                so.FindProperty("_npcNameText").objectReferenceValue = nameText;
                so.FindProperty("_talkButton").objectReferenceValue = talkBtn;
                so.FindProperty("_shopButton").objectReferenceValue = shopBtn;
                so.FindProperty("_leaveButton").objectReferenceValue = leaveBtn;
                so.ApplyModifiedPropertiesWithoutUndo();

                choiceObj.SetActive(false);
            }

            // --- B. Create Shop & Crafting Master UI ---
            ShopUIController shopUI = Object.FindAnyObjectByType<ShopUIController>();
            if (shopUI == null)
            {
                GameObject shopObj = new GameObject("ShopUIPanel", typeof(RectTransform));
                shopObj.transform.SetParent(uiRoot, false);
                shopUI = shopObj.AddComponent<ShopUIController>();

                RectTransform shopRect = shopObj.GetComponent<RectTransform>();
                shopRect.anchorMin = new Vector2(0.5f, 0.5f);
                shopRect.anchorMax = new Vector2(0.5f, 0.5f);
                shopRect.pivot = new Vector2(0.5f, 0.5f);
                shopRect.sizeDelta = new Vector2(1050, 680);

                Image shopBg = shopObj.AddComponent<Image>();
                shopBg.color = new Color(0.08f, 0.08f, 0.11f, 0.98f);
                if (_squareSprite != null) shopBg.sprite = _squareSprite;

                // 1. Header
                GameObject header = new GameObject("Header", typeof(RectTransform));
                header.transform.SetParent(shopObj.transform, false);
                RectTransform headerRect = header.GetComponent<RectTransform>();
                headerRect.anchorMin = new Vector2(0, 1);
                headerRect.anchorMax = new Vector2(1, 1);
                headerRect.pivot = new Vector2(0.5f, 1);
                headerRect.sizeDelta = new Vector2(0, 70);
                headerRect.anchoredPosition = new Vector2(0, 0);

                TMP_Text titleText = CreateText(header.transform, "TitleText", "Trgovina", 30, TextAlignmentOptions.Left, new Vector2(30, -35), new Vector2(400, 50));
                titleText.color = new Color(1f, 0.88f, 0.4f, 1f);

                TMP_Text goldText = CreateText(header.transform, "GoldText", "Zlato: 0g", 24, TextAlignmentOptions.Right, new Vector2(-120, -35), new Vector2(300, 50));
                goldText.color = new Color(1f, 0.85f, 0.2f, 1f);

                Button closeBtn = CreateButton(header.transform, "BtnClose", "X", new Vector2(-35, -35), new Vector2(50, 45), new Color(0.6f, 0.2f, 0.2f, 1f));

                // 2. Navigation Tabs Bar
                GameObject tabs = new GameObject("TabsBar", typeof(RectTransform));
                tabs.transform.SetParent(shopObj.transform, false);
                RectTransform tabsRect = tabs.GetComponent<RectTransform>();
                tabsRect.anchorMin = new Vector2(0, 1);
                tabsRect.anchorMax = new Vector2(0.6f, 1);
                tabsRect.pivot = new Vector2(0, 1);
                tabsRect.sizeDelta = new Vector2(0, 50);
                tabsRect.anchoredPosition = new Vector2(25, -75);

                Button tabBuy = CreateButton(tabs.transform, "TabBuy", "Kupi (Buy)", new Vector2(80, -25), new Vector2(150, 42), new Color(0.2f, 0.35f, 0.55f, 1f));
                Button tabSell = CreateButton(tabs.transform, "TabSell", "Prodaj (Sell)", new Vector2(245, -25), new Vector2(150, 42), new Color(0.2f, 0.45f, 0.35f, 1f));
                Button tabCraft = CreateButton(tabs.transform, "TabCraft", "Izrada / Taljenje", new Vector2(420, -25), new Vector2(180, 42), new Color(0.55f, 0.35f, 0.2f, 1f));

                // 3. Scroll View / Item Grid Container
                GameObject scrollObj = new GameObject("ItemsScrollView", typeof(RectTransform), typeof(ScrollRect));
                scrollObj.transform.SetParent(shopObj.transform, false);
                RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
                scrollRect.anchorMin = new Vector2(0, 0);
                scrollRect.anchorMax = new Vector2(0.62f, 1);
                scrollRect.pivot = new Vector2(0, 0.5f);
                scrollRect.anchoredPosition = new Vector2(25, -25);
                scrollRect.sizeDelta = new Vector2(-30, -170);

                Image scrollBg = scrollObj.AddComponent<Image>();
                scrollBg.color = new Color(0.12f, 0.12f, 0.16f, 0.8f);

                GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
                viewport.transform.SetParent(scrollObj.transform, false);
                RectTransform vpRect = viewport.GetComponent<RectTransform>();
                vpRect.anchorMin = Vector2.zero;
                vpRect.anchorMax = Vector2.one;
                vpRect.sizeDelta = Vector2.zero;
                viewport.GetComponent<Image>().color = Color.white;
                viewport.GetComponent<Mask>().showMaskGraphic = false;

                GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
                content.transform.SetParent(viewport.transform, false);
                RectTransform contentRect = content.GetComponent<RectTransform>();
                contentRect.anchorMin = new Vector2(0, 1);
                contentRect.anchorMax = new Vector2(1, 1);
                contentRect.pivot = new Vector2(0.5f, 1);
                contentRect.sizeDelta = new Vector2(0, 0);

                VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
                vlg.spacing = 6;
                vlg.padding = new RectOffset(8, 8, 8, 8);
                vlg.childControlWidth = true;
                vlg.childControlHeight = false;
                vlg.childForceExpandWidth = true;
                vlg.childForceExpandHeight = false;

                ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                ScrollRect sr = scrollObj.GetComponent<ScrollRect>();
                sr.viewport = vpRect;
                sr.content = contentRect;
                sr.horizontal = false;
                sr.vertical = true;

                // 4. Details / Selection Panel (Right Side)
                GameObject detailPanel = new GameObject("DetailsPanel", typeof(RectTransform));
                detailPanel.transform.SetParent(shopObj.transform, false);
                RectTransform dpRect = detailPanel.GetComponent<RectTransform>();
                dpRect.anchorMin = new Vector2(0.64f, 0);
                dpRect.anchorMax = new Vector2(1f, 1);
                dpRect.pivot = new Vector2(0.5f, 0.5f);
                dpRect.anchoredPosition = new Vector2(-15, -25);
                dpRect.sizeDelta = new Vector2(-30, -120);

                Image dpBg = detailPanel.AddComponent<Image>();
                dpBg.color = new Color(0.14f, 0.14f, 0.19f, 0.95f);
                if (_squareSprite != null) dpBg.sprite = _squareSprite;

                // Icon
                GameObject iconObj = new GameObject("DetailIcon", typeof(RectTransform), typeof(Image));
                iconObj.transform.SetParent(detailPanel.transform, false);
                RectTransform iconRt = iconObj.GetComponent<RectTransform>();
                iconRt.anchorMin = new Vector2(0.5f, 1);
                iconRt.anchorMax = new Vector2(0.5f, 1);
                iconRt.pivot = new Vector2(0.5f, 1);
                iconRt.anchoredPosition = new Vector2(0, -20);
                iconRt.sizeDelta = new Vector2(70, 70);
                Image detailIcon = iconObj.GetComponent<Image>();
                if (_squareSprite != null) detailIcon.sprite = _squareSprite;

                // Title
                TMP_Text detailTitle = CreateText(detailPanel.transform, "DetailTitle", "Naziv Artikla", 22, TextAlignmentOptions.Center, new Vector2(0, -105), new Vector2(320, 35));
                detailTitle.color = new Color(1f, 0.9f, 0.4f, 1f);

                // Description
                TMP_Text detailDesc = CreateText(detailPanel.transform, "DetailDesc", "Opis artikla...", 15, TextAlignmentOptions.TopLeft, new Vector2(0, -145), new Vector2(310, 60));
                detailDesc.color = new Color(0.85f, 0.85f, 0.85f, 1f);

                // Requirements / Info
                TMP_Text detailReqs = CreateText(detailPanel.transform, "DetailRequirements", "Zahtjevi...", 15, TextAlignmentOptions.TopLeft, new Vector2(0, -220), new Vector2(310, 90));
                detailReqs.color = new Color(0.9f, 0.9f, 0.9f, 1f);

                // Quantity Controls
                GameObject qtyControls = new GameObject("QuantityControls", typeof(RectTransform));
                qtyControls.transform.SetParent(detailPanel.transform, false);
                RectTransform qtyRect = qtyControls.GetComponent<RectTransform>();
                qtyRect.anchorMin = new Vector2(0.5f, 0);
                qtyRect.anchorMax = new Vector2(0.5f, 0);
                qtyRect.pivot = new Vector2(0.5f, 0);
                qtyRect.anchoredPosition = new Vector2(0, 110);
                qtyRect.sizeDelta = new Vector2(240, 40);

                Button btnDec = CreateButton(qtyControls.transform, "BtnDecrease", "-", new Vector2(-65, 0), new Vector2(40, 36), new Color(0.3f, 0.3f, 0.35f, 1f));
                TMP_Text qtyText = CreateText(qtyControls.transform, "QtyText", "1", 20, TextAlignmentOptions.Center, Vector2.zero, new Vector2(70, 36));
                Button btnInc = CreateButton(qtyControls.transform, "BtnIncrease", "+", new Vector2(65, 0), new Vector2(40, 36), new Color(0.3f, 0.3f, 0.35f, 1f));

                // Total Cost Text
                TMP_Text totalCostText = CreateText(detailPanel.transform, "TotalCostText", "Ukupno: 0g", 18, TextAlignmentOptions.Center, new Vector2(0, 75), new Vector2(310, 30));
                totalCostText.rectTransform.anchorMin = new Vector2(0.5f, 0);
                totalCostText.rectTransform.anchorMax = new Vector2(0.5f, 0);
                totalCostText.rectTransform.pivot = new Vector2(0.5f, 0);
                totalCostText.color = new Color(1f, 0.85f, 0.2f, 1f);

                // Action Button
                Button actionBtn = CreateButton(detailPanel.transform, "BtnAction", "Potvrdi", new Vector2(0, 25), new Vector2(260, 42), new Color(0.25f, 0.5f, 0.25f, 1f));
                actionBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
                actionBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
                actionBtn.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                TMP_Text actionBtnText = actionBtn.GetComponentInChildren<TMP_Text>();

                // Status Message Text (Bottom)
                TMP_Text statusText = CreateText(shopObj.transform, "StatusText", "", 18, TextAlignmentOptions.Center, new Vector2(0, 25), new Vector2(700, 35));
                statusText.rectTransform.anchorMin = new Vector2(0.5f, 0);
                statusText.rectTransform.anchorMax = new Vector2(0.5f, 0);
                statusText.rectTransform.pivot = new Vector2(0.5f, 0);

                // Wire Serialized Properties
                var so = new SerializedObject(shopUI);
                so.FindProperty("_panelRoot").objectReferenceValue = shopObj;
                so.FindProperty("_shopTitleText").objectReferenceValue = titleText;
                so.FindProperty("_playerGoldText").objectReferenceValue = goldText;
                so.FindProperty("_closeButton").objectReferenceValue = closeBtn;

                so.FindProperty("_tabBuyButton").objectReferenceValue = tabBuy;
                so.FindProperty("_tabSellButton").objectReferenceValue = tabSell;
                so.FindProperty("_tabCraftButton").objectReferenceValue = tabCraft;

                so.FindProperty("_itemsContainer").objectReferenceValue = content.transform;
                so.FindProperty("_itemSlotPrefab").objectReferenceValue = itemSlotPrefab;
                so.FindProperty("_recipeSlotPrefab").objectReferenceValue = recipeSlotPrefab;

                so.FindProperty("_detailPanelRoot").objectReferenceValue = detailPanel;
                so.FindProperty("_detailIconImage").objectReferenceValue = detailIcon;
                so.FindProperty("_detailTitleText").objectReferenceValue = detailTitle;
                so.FindProperty("_detailDescriptionText").objectReferenceValue = detailDesc;
                so.FindProperty("_detailRequirementsText").objectReferenceValue = detailReqs;
                so.FindProperty("_detailTotalCostText").objectReferenceValue = totalCostText;

                so.FindProperty("_quantityControlsRoot").objectReferenceValue = qtyControls;
                so.FindProperty("_decreaseQtyButton").objectReferenceValue = btnDec;
                so.FindProperty("_increaseQtyButton").objectReferenceValue = btnInc;
                so.FindProperty("_quantityText").objectReferenceValue = qtyText;

                so.FindProperty("_actionButton").objectReferenceValue = actionBtn;
                so.FindProperty("_actionButtonText").objectReferenceValue = actionBtnText;
                so.FindProperty("_statusMessageText").objectReferenceValue = statusText;
                so.ApplyModifiedPropertiesWithoutUndo();

                shopObj.SetActive(false);
            }
        }

        private static ShopItemSlotUI CreateItemSlotPrefab(string path)
        {
            ShopItemSlotUI existing = AssetDatabase.LoadAssetAtPath<ShopItemSlotUI>(path);
            if (existing != null) return existing;

            GameObject slotObj = new GameObject("ShopItemSlot", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(580, 52);

            Image bg = slotObj.GetComponent<Image>();
            bg.color = new Color(0.18f, 0.18f, 0.23f, 0.95f);
            if (_squareSprite != null) bg.sprite = _squareSprite;

            Button btn = slotObj.GetComponent<Button>();

            // Icon
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(slotObj.transform, false);
            RectTransform iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0.5f);
            iconRt.anchorMax = new Vector2(0, 0.5f);
            iconRt.pivot = new Vector2(0, 0.5f);
            iconRt.anchoredPosition = new Vector2(10, 0);
            iconRt.sizeDelta = new Vector2(38, 38);
            Image iconImg = iconObj.GetComponent<Image>();
            if (_squareSprite != null) iconImg.sprite = _squareSprite;

            // Name
            TMP_Text nameTxt = CreateText(slotObj.transform, "ItemName", "Naziv Predmeta", 17, TextAlignmentOptions.Left, new Vector2(60, 0), new Vector2(260, 36));
            nameTxt.rectTransform.anchorMin = new Vector2(0, 0.5f);
            nameTxt.rectTransform.anchorMax = new Vector2(0, 0.5f);
            nameTxt.rectTransform.pivot = new Vector2(0, 0.5f);

            // Price
            TMP_Text priceTxt = CreateText(slotObj.transform, "Price", "10g", 17, TextAlignmentOptions.Right, new Vector2(-120, 0), new Vector2(90, 36));
            priceTxt.rectTransform.anchorMin = new Vector2(1, 0.5f);
            priceTxt.rectTransform.anchorMax = new Vector2(1, 0.5f);
            priceTxt.rectTransform.pivot = new Vector2(1, 0.5f);
            priceTxt.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Count / Stock
            TMP_Text countTxt = CreateText(slotObj.transform, "Count", "Besk.", 14, TextAlignmentOptions.Right, new Vector2(-15, 0), new Vector2(95, 36));
            countTxt.rectTransform.anchorMin = new Vector2(1, 0.5f);
            countTxt.rectTransform.anchorMax = new Vector2(1, 0.5f);
            countTxt.rectTransform.pivot = new Vector2(1, 0.5f);
            countTxt.color = new Color(0.75f, 0.75f, 0.75f, 1f);

            // Selection Border
            GameObject borderObj = new GameObject("SelectedBorder", typeof(RectTransform), typeof(Image));
            borderObj.transform.SetParent(slotObj.transform, false);
            RectTransform borderRt = borderObj.GetComponent<RectTransform>();
            borderRt.anchorMin = Vector2.zero;
            borderRt.anchorMax = Vector2.one;
            borderRt.sizeDelta = Vector2.zero;
            Image borderImg = borderObj.GetComponent<Image>();
            borderImg.color = new Color(1f, 0.85f, 0.25f, 0.35f);
            borderObj.SetActive(false);

            ShopItemSlotUI slotComp = slotObj.AddComponent<ShopItemSlotUI>();
            var so = new SerializedObject(slotComp);
            so.FindProperty("_iconImage").objectReferenceValue = iconImg;
            so.FindProperty("_nameText").objectReferenceValue = nameTxt;
            so.FindProperty("_priceText").objectReferenceValue = priceTxt;
            so.FindProperty("_countText").objectReferenceValue = countTxt;
            so.FindProperty("_selectButton").objectReferenceValue = btn;
            so.FindProperty("_selectedBorder").objectReferenceValue = borderObj;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slotObj, path);
            Object.DestroyImmediate(slotObj);
            return prefab.GetComponent<ShopItemSlotUI>();
        }

        private static ShopRecipeSlotUI CreateRecipeSlotPrefab(string path)
        {
            ShopRecipeSlotUI existing = AssetDatabase.LoadAssetAtPath<ShopRecipeSlotUI>(path);
            if (existing != null) return existing;

            GameObject slotObj = new GameObject("ShopRecipeSlot", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(580, 56);

            Image bg = slotObj.GetComponent<Image>();
            bg.color = new Color(0.18f, 0.18f, 0.23f, 0.95f);
            if (_squareSprite != null) bg.sprite = _squareSprite;

            Button btn = slotObj.GetComponent<Button>();

            // Result Icon
            GameObject iconObj = new GameObject("ResultIcon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(slotObj.transform, false);
            RectTransform iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0.5f);
            iconRt.anchorMax = new Vector2(0, 0.5f);
            iconRt.pivot = new Vector2(0, 0.5f);
            iconRt.anchoredPosition = new Vector2(10, 0);
            iconRt.sizeDelta = new Vector2(40, 40);
            Image iconImg = iconObj.GetComponent<Image>();
            if (_squareSprite != null) iconImg.sprite = _squareSprite;

            // Recipe Name
            TMP_Text nameTxt = CreateText(slotObj.transform, "RecipeName", "Naziv Recepta", 16, TextAlignmentOptions.Left, new Vector2(60, 10), new Vector2(340, 26));
            nameTxt.rectTransform.anchorMin = new Vector2(0, 0.5f);
            nameTxt.rectTransform.anchorMax = new Vector2(0, 0.5f);
            nameTxt.rectTransform.pivot = new Vector2(0, 0.5f);
            nameTxt.color = new Color(1f, 0.9f, 0.4f, 1f);

            // Ingredients Summary
            TMP_Text ingTxt = CreateText(slotObj.transform, "IngredientsSummary", "2x Iron Ore + 4x Coal", 13, TextAlignmentOptions.Left, new Vector2(60, -12), new Vector2(500, 22));
            ingTxt.rectTransform.anchorMin = new Vector2(0, 0.5f);
            ingTxt.rectTransform.anchorMax = new Vector2(0, 0.5f);
            ingTxt.rectTransform.pivot = new Vector2(0, 0.5f);
            ingTxt.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // Selection Border
            GameObject borderObj = new GameObject("SelectedBorder", typeof(RectTransform), typeof(Image));
            borderObj.transform.SetParent(slotObj.transform, false);
            RectTransform borderRt = borderObj.GetComponent<RectTransform>();
            borderRt.anchorMin = Vector2.zero;
            borderRt.anchorMax = Vector2.one;
            borderRt.sizeDelta = Vector2.zero;
            Image borderImg = borderObj.GetComponent<Image>();
            borderImg.color = new Color(1f, 0.85f, 0.25f, 0.35f);
            borderObj.SetActive(false);

            ShopRecipeSlotUI slotComp = slotObj.AddComponent<ShopRecipeSlotUI>();
            var so = new SerializedObject(slotComp);
            so.FindProperty("_resultIconImage").objectReferenceValue = iconImg;
            so.FindProperty("_recipeNameText").objectReferenceValue = nameTxt;
            so.FindProperty("_ingredientsSummaryText").objectReferenceValue = ingTxt;
            so.FindProperty("_selectButton").objectReferenceValue = btn;
            so.FindProperty("_selectedBorder").objectReferenceValue = borderObj;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slotObj, path);
            Object.DestroyImmediate(slotObj);
            return prefab.GetComponent<ShopRecipeSlotUI>();
        }

        private static void ConfigureNPCsInScene()
        {
            ShopSO marketShop = AssetDatabase.LoadAssetAtPath<ShopSO>("Assets/Data/Shop/Shop_Market.asset");
            ShopSO smithShop = AssetDatabase.LoadAssetAtPath<ShopSO>("Assets/Data/Shop/Shop_Blacksmith.asset");

            // Look for existing villagers or find by name
            NPCDialogueTrigger[] allTriggers = Object.FindObjectsByType<NPCDialogueTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (allTriggers.Length >= 2)
            {
                // Assign Market to first, Blacksmith to second
                AssignShopToNPC(allTriggers[0], "Trgovac Milan (Tržnica)", marketShop);
                AssignShopToNPC(allTriggers[1], "Kovač Vuk (Kovačnica)", smithShop);
            }
            else
            {
                // Find villagers by name
                GameObject v1 = GameObject.Find("Villager (1)");
                GameObject v2 = GameObject.Find("Villager (2)");

                if (v1 != null)
                {
                    NPCDialogueTrigger t1 = v1.GetComponent<NPCDialogueTrigger>();
                    if (t1 != null) AssignShopToNPC(t1, "Trgovac Milan (Tržnica)", marketShop);
                }

                if (v2 != null)
                {
                    NPCDialogueTrigger t2 = v2.GetComponent<NPCDialogueTrigger>();
                    if (t2 != null) AssignShopToNPC(t2, "Kovač Vuk (Kovačnica)", smithShop);
                }
            }
        }

        private static void AssignShopToNPC(NPCDialogueTrigger trigger, string name, ShopSO shop)
        {
            if (trigger == null) return;
            var so = new SerializedObject(trigger);
            so.FindProperty("_npcName").stringValue = name;
            so.FindProperty("_shopData").objectReferenceValue = shop;
            so.ApplyModifiedProperties();
        }

        private static TMP_Text CreateText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions align, Vector2 pos, Vector2 size)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            if (_fontAsset != null) tmp.font = _fontAsset;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            return tmp;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color bgColor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.GetComponent<Image>();
            img.color = bgColor;
            if (_squareSprite != null) img.sprite = _squareSprite;

            Button btn = go.GetComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = bgColor;
            cb.highlightedColor = bgColor * 1.25f;
            cb.pressedColor = bgColor * 0.8f;
            btn.colors = cb;

            TMP_Text lbl = CreateText(go.transform, "Label", label, 18, TextAlignmentOptions.Center, Vector2.zero, size);
            lbl.rectTransform.anchorMin = Vector2.zero;
            lbl.rectTransform.anchorMax = Vector2.one;
            lbl.rectTransform.sizeDelta = Vector2.zero;

            return btn;
        }
    }
}
