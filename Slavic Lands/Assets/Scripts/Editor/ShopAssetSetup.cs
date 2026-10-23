using System.IO;
using Core.Interfaces;
using Data.Crafting;
using Data.Items;
using Data.Shop;
using Gameplay.Resources;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ShopAssetSetup
    {
        [InitializeOnLoadMethod]
        public static void InitializeAssets()
        {
            if (File.Exists("Assets/Data/Shop/Shop_Market.asset") &&
                File.Exists("Assets/Data/Shop/Shop_Blacksmith.asset") &&
                File.Exists("Assets/Data/Items/ItemDatabase.asset"))
            {
                return;
            }

            CreateAll();
        }

        [MenuItem("Tools/Setup Shop Assets")]
        public static void CreateAll()
        {
            if (!Directory.Exists("Assets/Data/Items")) Directory.CreateDirectory("Assets/Data/Items");
            if (!Directory.Exists("Assets/Data/Crafting")) Directory.CreateDirectory("Assets/Data/Crafting");
            if (!Directory.Exists("Assets/Data/Shop")) Directory.CreateDirectory("Assets/Data/Shop");

            // Raw Resources
            ItemSO wood = CreateItem("Assets/Data/Items/Item_Wood.asset", "Drvo (Wood)", "Sirovo drvo.", ItemCategory.RawResource, 4, 2, true, ResourceType.Wood);
            ItemSO stone = CreateItem("Assets/Data/Items/Item_Stone.asset", "Kamen (Stone)", "Čvrsti kamen.", ItemCategory.RawResource, 5, 2, true, ResourceType.Stone);
            ItemSO coal = CreateItem("Assets/Data/Items/Item_Coal.asset", "Ugalj (Coal)", "Gorući ugljen za kovačnicu.", ItemCategory.RawResource, 8, 4, true, ResourceType.Coal);
            ItemSO iron = CreateItem("Assets/Data/Items/Item_IronOre.asset", "Ruda Željeza (Iron Ore)", "Sirova željezna ruda.", ItemCategory.RawResource, 15, 7, true, ResourceType.Iron);
            ItemSO goldOre = CreateItem("Assets/Data/Items/Item_GoldOre.asset", "Ruda Zlata (Gold Ore)", "Sirova zlatna ruda.", ItemCategory.RawResource, 30, 15, true, ResourceType.Gold);
            ItemSO crystal = CreateItem("Assets/Data/Items/Item_Crystal.asset", "Kristal (Crystal)", "Rijedak kristal.", ItemCategory.RawResource, 50, 25, true, ResourceType.Crystal);
            ItemSO hide = CreateItem("Assets/Data/Items/Item_Hide.asset", "Koža (Hide)", "Životinjska koža.", ItemCategory.RawResource, 10, 5, true, ResourceType.Hide);
            ItemSO food = CreateItem("Assets/Data/Items/Item_Food.asset", "Hrana (Food)", "Namirnice za hranu.", ItemCategory.RawResource, 6, 3, true, ResourceType.Food);

            // Materials (Ingots)
            ItemSO ironIngot = CreateItem("Assets/Data/Items/Item_IronIngot.asset", "Željezna Poluga (Iron Ingot)", "Poluga čistog željeza.", ItemCategory.Material, 50, 25, false, ResourceType.Iron);
            ItemSO goldIngot = CreateItem("Assets/Data/Items/Item_GoldIngot.asset", "Zlatna Poluga (Gold Ingot)", "Poluga čistog zlata.", ItemCategory.Material, 100, 50, false, ResourceType.Gold);

            // Equipment
            EquipmentItemSO ironSword = CreateEquipment("Assets/Data/Items/Item_IronSword.asset", "Željezni Mač (Iron Sword)", "Oštar željezni mač.", EquipmentSlot.Weapon, ToolType.Axe, 25f, 0f, 120, 60);
            EquipmentItemSO ironPickaxe = CreateEquipment("Assets/Data/Items/Item_IronPickaxe.asset", "Željezni Pijučić (Iron Pickaxe)", "Kramp za rudarenje.", EquipmentSlot.Tool, ToolType.Pickaxe, 10f, 0f, 100, 50);
            EquipmentItemSO ironArmor = CreateEquipment("Assets/Data/Items/Item_IronArmor.asset", "Željezni Oklop (Iron Armor)", "Teški željezni oklop.", EquipmentSlot.Chest, ToolType.None, 0f, 15f, 150, 75);
            EquipmentItemSO hunterBow = CreateEquipment("Assets/Data/Items/Item_HunterBow.asset", "Lovački Luk (Hunter Bow)", "Precizan luk.", EquipmentSlot.Weapon, ToolType.Bow, 20f, 0f, 110, 55);
            EquipmentItemSO ironShield = CreateEquipment("Assets/Data/Items/Item_IronShield.asset", "Željezni Štit (Iron Shield)", "Masivni željezni štit.", EquipmentSlot.Shield, ToolType.ShieldBash, 5f, 20f, 90, 45);

            // Crafting Recipes
            CraftingRecipeSO rIronIngot = CreateRecipe("Assets/Data/Crafting/Recipe_IronIngot.asset", "Taljenje Željeza (Smelt Iron Ingot)",
                "Istopi 2 rude željeza i 4 ugljena u 1 željeznu polugu.", ironIngot, 1, 0,
                new (bool, ResourceType, ItemSO, int)[] { (true, ResourceType.Iron, null, 2), (true, ResourceType.Coal, null, 4) });

            CraftingRecipeSO rGoldIngot = CreateRecipe("Assets/Data/Crafting/Recipe_GoldIngot.asset", "Taljenje Zlata (Smelt Gold Ingot)",
                "Istopi 2 rude zlata i 4 ugljena u 1 zlatnu polugu.", goldIngot, 1, 0,
                new (bool, ResourceType, ItemSO, int)[] { (true, ResourceType.Gold, null, 2), (true, ResourceType.Coal, null, 4) });

            CraftingRecipeSO rSword = CreateRecipe("Assets/Data/Crafting/Recipe_IronSword.asset", "Kovanje Željeznog Mača",
                "Iskuj mač od 2 željezne poluge i 1 drva.", ironSword, 1, 10,
                new (bool, ResourceType, ItemSO, int)[] { (false, ResourceType.Iron, ironIngot, 2), (true, ResourceType.Wood, null, 1) });

            CraftingRecipeSO rPickaxe = CreateRecipe("Assets/Data/Crafting/Recipe_IronPickaxe.asset", "Kovanje Željeznog Pijuka",
                "Iskuj pijuk od 3 željezne poluge i 2 drva.", ironPickaxe, 1, 10,
                new (bool, ResourceType, ItemSO, int)[] { (false, ResourceType.Iron, ironIngot, 3), (true, ResourceType.Wood, null, 2) });

            CraftingRecipeSO rArmor = CreateRecipe("Assets/Data/Crafting/Recipe_IronArmor.asset", "Kovanje Željeznog Oklopa",
                "Iskuj oklop od 5 željeznih poluga i 2 kože.", ironArmor, 1, 15,
                new (bool, ResourceType, ItemSO, int)[] { (false, ResourceType.Iron, ironIngot, 5), (true, ResourceType.Hide, null, 2) });

            // Shops
            CreateShop("Assets/Data/Shop/Shop_Market.asset", "Seoska Tržnica (Market)", ShopType.Market,
                "Dobrodošao! Ovdje možeš kupiti i prodati sirovine.", false,
                new ItemSO[] { wood, stone, coal, iron, goldOre, crystal, hide, food },
                new int[] { 4, 5, 8, 15, 30, 50, 10, 6 },
                new int[] { 2, 2, 4, 7, 15, 25, 5, 3 },
                new CraftingRecipeSO[0]);

            CreateShop("Assets/Data/Shop/Shop_Blacksmith.asset", "Kovačnica (Smith Shop)", ShopType.Blacksmith,
                "Dobrodošao u kovačnicu! Ovdje prodajem oružje i oklop te talim rude.", true,
                new ItemSO[] { ironSword, ironPickaxe, ironArmor, hunterBow, ironShield, ironIngot },
                new int[] { 120, 100, 150, 110, 90, 50 },
                new int[] { 60, 50, 75, 55, 45, 25 },
                new CraftingRecipeSO[] { rIronIngot, rGoldIngot, rSword, rPickaxe, rArmor });

            // Create Item Database
            CreateItemDatabase("Assets/Data/Items/ItemDatabase.asset", new ItemSO[] {
                wood, stone, coal, iron, goldOre, crystal, hide, food,
                ironIngot, goldIngot,
                ironSword, ironPickaxe, ironArmor, hunterBow, ironShield
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ShopAssetSetup] Shop assets generated successfully!");
        }

        private static void CreateItemDatabase(string path, ItemSO[] items)
        {
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(path);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<ItemDatabaseSO>();
                AssetDatabase.CreateAsset(db, path);
            }

            var so = new SerializedObject(db);
            var listProp = so.FindProperty("_allItems");
            listProp.ClearArray();
            for (int i = 0; i < items.Length; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static ItemSO CreateItem(string path, string name, string desc, ItemCategory cat, int buy, int sell, bool isRaw, ResourceType res)
        {
            var item = AssetDatabase.LoadAssetAtPath<ItemSO>(path);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemSO>();
                AssetDatabase.CreateAsset(item, path);
            }
            var so = new SerializedObject(item);
            so.FindProperty("_itemId").stringValue = Path.GetFileNameWithoutExtension(path);
            so.FindProperty("_itemName").stringValue = name;
            so.FindProperty("_description").stringValue = desc;
            so.FindProperty("_category").enumValueIndex = (int)cat;
            so.FindProperty("_buyPrice").intValue = buy;
            so.FindProperty("_sellPrice").intValue = sell;
            so.FindProperty("_isRawResource").boolValue = isRaw;
            so.FindProperty("_resourceType").enumValueIndex = (int)res;
            so.ApplyModifiedPropertiesWithoutUndo();
            return item;
        }

        private static EquipmentItemSO CreateEquipment(string path, string name, string desc, EquipmentSlot slot, ToolType tool, float dmg, float def, int buy, int sell)
        {
            var item = AssetDatabase.LoadAssetAtPath<EquipmentItemSO>(path);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<EquipmentItemSO>();
                AssetDatabase.CreateAsset(item, path);
            }
            var so = new SerializedObject(item);
            so.FindProperty("_itemId").stringValue = Path.GetFileNameWithoutExtension(path);
            so.FindProperty("_itemName").stringValue = name;
            so.FindProperty("_description").stringValue = desc;
            so.FindProperty("_category").enumValueIndex = (int)(slot == EquipmentSlot.Chest || slot == EquipmentSlot.Shield ? ItemCategory.Armor : (slot == EquipmentSlot.Tool ? ItemCategory.Tool : ItemCategory.Weapon));
            so.FindProperty("_buyPrice").intValue = buy;
            so.FindProperty("_sellPrice").intValue = sell;
            so.FindProperty("_slot").enumValueIndex = (int)slot;
            so.FindProperty("_toolType").enumValueIndex = (int)tool;
            so.FindProperty("_damageBonus").floatValue = dmg;
            so.FindProperty("_defenseBonus").floatValue = def;
            so.ApplyModifiedPropertiesWithoutUndo();
            return item;
        }

        private static CraftingRecipeSO CreateRecipe(string path, string name, string desc, ItemSO resItem, int resAmt, int fee, (bool isRaw, ResourceType res, ItemSO itm, int amt)[] ings)
        {
            var recipe = AssetDatabase.LoadAssetAtPath<CraftingRecipeSO>(path);
            if (recipe == null)
            {
                recipe = ScriptableObject.CreateInstance<CraftingRecipeSO>();
                AssetDatabase.CreateAsset(recipe, path);
            }
            var so = new SerializedObject(recipe);
            so.FindProperty("_recipeName").stringValue = name;
            so.FindProperty("_description").stringValue = desc;
            so.FindProperty("_resultItem").objectReferenceValue = resItem;
            so.FindProperty("_resultAmount").intValue = resAmt;
            so.FindProperty("_goldCost").intValue = fee;
            var p = so.FindProperty("_ingredients");
            p.ClearArray();
            for (int i = 0; i < ings.Length; i++)
            {
                p.InsertArrayElementAtIndex(i);
                var e = p.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("UseRawResourceType").boolValue = ings[i].isRaw;
                e.FindPropertyRelative("RawResource").enumValueIndex = (int)ings[i].res;
                e.FindPropertyRelative("Item").objectReferenceValue = ings[i].itm;
                e.FindPropertyRelative("Amount").intValue = ings[i].amt;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return recipe;
        }

        private static ShopSO CreateShop(string path, string title, ShopType type, string greet, bool craft, ItemSO[] items, int[] buys, int[] sells, CraftingRecipeSO[] recipes)
        {
            var shop = AssetDatabase.LoadAssetAtPath<ShopSO>(path);
            if (shop == null)
            {
                shop = ScriptableObject.CreateInstance<ShopSO>();
                AssetDatabase.CreateAsset(shop, path);
            }
            var so = new SerializedObject(shop);
            so.FindProperty("_shopTitle").stringValue = title;
            so.FindProperty("_shopType").enumValueIndex = (int)type;
            so.FindProperty("_greetingMessage").stringValue = greet;
            so.FindProperty("_supportsCrafting").boolValue = craft;
            var itemsProp = so.FindProperty("_items");
            itemsProp.ClearArray();
            for (int i = 0; i < items.Length; i++)
            {
                itemsProp.InsertArrayElementAtIndex(i);
                var e = itemsProp.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("Item").objectReferenceValue = items[i];
                e.FindPropertyRelative("BuyPrice").intValue = buys[i];
                e.FindPropertyRelative("SellPrice").intValue = sells[i];
                e.FindPropertyRelative("CanBuy").boolValue = true;
                e.FindPropertyRelative("CanSell").boolValue = true;
                e.FindPropertyRelative("IsUnlimited").boolValue = true;
                e.FindPropertyRelative("Stock").intValue = 99;
            }
            var recProp = so.FindProperty("_craftingRecipes");
            recProp.ClearArray();
            for (int i = 0; i < recipes.Length; i++)
            {
                recProp.InsertArrayElementAtIndex(i);
                recProp.GetArrayElementAtIndex(i).objectReferenceValue = recipes[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return shop;
        }
    }
}
