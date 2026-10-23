# Project Overview
- **Game Title**: Slavic Lands
- **High-Level Concept**: 2D side-scrolling platformer with action, exploration, resource gathering, village interactions, and dungeon crawling.
- **Players**: Single player
- **Inspiration / Reference Games**: Terraria, Kingdom: Two Crowns, Castlevania, Stardew Valley
- **Tone / Art Direction**: Slavic mythological fantasy, hand-crafted 2D pixel art/sprites
- **Target Platform**: PC (StandaloneWindows64)
- **Screen Orientation / Resolution**: Landscape 1920x1080 (16:9)
- **Render Pipeline**: Universal Render Pipeline (URP 2D)

---

# Game Mechanics

## Core Gameplay Loop
1. **Gather & Hunt**: Player explores world and dungeons, gathers raw resources (Wood, Stone, Iron, Coal, Gold, Crystals, Hide, Food) and earns XP.
2. **Interact & Trade**: Player returns to the village, interacts with NPCs (press `E`), and opens interaction choice panels.
3. **Market (Tržnica)**: Player sells excess raw resources for Gold and buys scarce resources needed for progression.
4. **Smith Shop (Kovačnica)**:
   - **Smelting & Crafting**: Player uses raw resources (e.g., 2 Iron ore + 4 Coal) to smelt refined materials (e.g., Iron Ingot).
   - **Forging & Equipment**: Player purchases or crafts advanced tools, weapons, and armor with specific stats and variable attributes.
5. **Progress & Conquer**: Upgraded equipment allows harvesting higher-tier nodes, surviving deeper dungeons, and defeating tougher enemies.

## Controls and Input Methods
- **`E` (Interaction Key)**: When near an NPC with `NPCDialogueTrigger` or `NPCInteractable`, pressing `E` brings up the NPC Choice Panel.
- **Mouse Left-Click**: Select choices in dialogue/menu, click tabs (Buy, Sell, Craft), select shop items, adjust quantity, and confirm transaction/crafting.
- **`Esc` or Close Button `(X)`**: Closes the Shop or NPC Choice menu and returns control to player movement.
- **Input Action System**: Fully integrated with existing `PlayerInputSystem.OnInteractionClick` and `OnActionLockChanged` to prevent player moving/attacking while browsing shop or choice UI.

---

# UI

## NPC Interaction Choice Panel (Wireframe)
```
+-------------------------------------------------------+
|                       [NPC Name]                      |
|           "Welcome traveler! How can I help you?"     |
+-------------------------------------------------------+
|  [ 1. Razgovor (Talk) ]                               |
|  [ 2. Trgovina (Shop) ]                               |
|  [ 3. Odlazak (Leave) ]                               |
+-------------------------------------------------------+
```

## Shop UI Panel (Unified for Market & Smith Shop)
```
+---------------------------------------------------------------------------------+
| [NPC / Shop Title: Blacksmith Shop]                         [Gold: 1,450] [ X ] |
+---------------------------------------------------------------------------------+
|  [ TAB: Kupi (Buy) ]   [ TAB: Prodaj (Sell) ]   [ TAB: Izrada (Crafting) ]      |
+------------------------------------+--------------------------------------------+
| ITEM LIST / RECIPE GRID            | ITEM DETAILS / TRANSACTION PREVIEW         |
| +--------+--------+--------+       |                                            |
| | Icon 1 | Icon 2 | Icon 3 |       |  [Large Icon]   Iron Ingot                 |
| | Iron   | Coal   | Sword  |       |  "A refined bar of iron forged with coal." |
| +--------+--------+--------+       |                                            |
| | Icon 4 | Icon 5 | Icon 6 |       |  Cost / Recipe Requirements:               |
| | Armor  | Pickaxe| Ingot  |       |   - Iron Ore: 2 / 2   [OK]                 |
| +--------+--------+--------+       |   - Coal:     4 / 4   [OK]                 |
|                                    |                                            |
|                                    |  Količina (Quantity): [ - ] [ 1 ] [ + ]    |
|                                    |  Ukupna cijena: 0 Zlata (Crafting)         |
|                                    |                                            |
|                                    |  [ POTVRDI / CRAFT / KUPI (ACTION BTN) ]   |
+------------------------------------+--------------------------------------------+
| Status poruka: "Uspješno izrađen 1x Iron Ingot!"                                |
+---------------------------------------------------------------------------------+
```

---

# Key Asset & Context

### Existing Scripts & Systems to Extend
- `Assets/Scripts/Player/Profile/PlayerResource.cs`: Add `HasResource(ResourceType, int)`, `ConsumeResource(ResourceType, int)` / `TrySpendResource(ResourceType, int)`, and event triggers.
- `Assets/Scripts/Entities/NPCs/NPCDialogueTrigger.cs`: Connect interaction button `E` to show the NPC choice dialog before/after dialogue text.
- `Assets/Scripts/Player/Controller/PlayerController.cs`: Provide clean API for resource and inventory manipulation with automatic `SavePlayer()`.

### New Data Structures (ScriptableObjects)
1. **`Assets/Scripts/Data/Items/ItemSO.cs`**:
   - Base ScriptableObject for all game items: `Id`, `ItemName`, `Description`, `Icon`, `ItemType` (Resource, Material, Weapon, Tool, Armor), `BaseValue` (in Gold), `MaxStackSize`.
2. **`Assets/Scripts/Data/Items/EquipmentItemSO.cs`** (Inherits `ItemSO`):
   - Adds combat and tool stats: `ToolType`, `DamageBonus`, `DefenseBonus`, `Durability`, `MiningPower`.
3. **`Assets/Scripts/Data/Shop/ShopItemData.cs`** & **`ShopSO.cs`**:
   - Defines a shop inventory: `ShopTitle`, `ShopType` (Market vs. Smith), `List<ShopItemEntry>` (ItemSO, BuyPrice, SellPrice, IsUnlimited, Stock).
4. **`Assets/Scripts/Data/Crafting/CraftingRecipeSO.cs`**:
   - Defines recipes: `RecipeName`, `ResultItem` (ItemSO), `ResultAmount`, `Ingredients` (List of ItemSO/ResourceType and required count), `CraftingCostGold`, `RequiredTool/Station`.

### New Runtime Scripts & Managers
1. **`Assets/Scripts/Gameplay/Inventory/PlayerInventory.cs`**:
   - Lightweight inventory storage for items, weapons, armor, and refined materials (with serialization support in `PlayerSaveData`).
2. **`Assets/Scripts/Gameplay/Shop/ShopManager.cs`**:
   - Core business logic for transactions: checks player gold/resources, transfers items, updates player profile.
3. **`Assets/Scripts/Gameplay/Crafting/CraftingManager.cs`**:
   - Evaluates player ingredients against `CraftingRecipeSO`, deducts ingredients and adds output item to inventory.
4. **`Assets/Scripts/UI/Shop/ShopUIController.cs`**:
   - Manages tabs (Buy, Sell, Craft), dynamically populates item slots, updates selection panel, and handles Buy/Sell/Craft button clicks.
5. **`Assets/Scripts/UI/Interaction/NPCInteractionChoiceUI.cs`**:
   - Manages the prompt popup with buttons `[Razgovor]`, `[Trgovina]`, `[Izlaz]`.

---

# Implementation Steps

### Step 1: Extend Player Resource & Currency Handling
- **Files**: `Assets/Scripts/Player/Profile/PlayerResource.cs`, `Assets/Scripts/Player/Controller/PlayerController.cs`
- **Changes**:
  - Add `bool HasResource(ResourceType type, int amount)`
  - Add `bool TrySpendResource(ResourceType type, int amount)`
  - Add `bool TrySpendGold(int amount)` (using `ResourceType.Gold`)
- **Dependencies**: None.

### Step 2: Create Modular Item, Shop, and Crafting ScriptableObjects
- **Files**:
  - `Assets/Scripts/Data/Items/ItemSO.cs`
  - `Assets/Scripts/Data/Items/EquipmentItemSO.cs`
  - `Assets/Scripts/Data/Shop/ShopSO.cs`
  - `Assets/Scripts/Data/Crafting/CraftingRecipeSO.cs`
- **Changes**:
  - Implement full SO data architecture supporting extensible attributes (stats, rarity, price multipliers).
- **Dependencies**: Step 1.

### Step 3: Create Player Inventory Component for Items & Refined Materials
- **Files**:
  - `Assets/Scripts/Gameplay/Inventory/PlayerInventory.cs`
  - `Assets/Scripts/SaveSystem/PlayerSaveData.cs`
- **Changes**:
  - Track owned items (Ingots, Equipment, Tools).
  - Sync with `SaveManager` so player does not lose bought/crafted items on reload.
- **Dependencies**: Step 2.

### Step 4: Implement Shop and Crafting Logic Controllers
- **Files**:
  - `Assets/Scripts/Gameplay/Shop/ShopManager.cs`
  - `Assets/Scripts/Gameplay/Crafting/CraftingManager.cs`
- **Changes**:
  - `BuyItem(ShopItemEntry, int quantity)` -> validates gold/inventory, deducts gold, grants item.
  - `SellItem(ItemSO, int quantity)` -> validates player ownership, removes item, gives gold.
  - `CraftRecipe(CraftingRecipeSO, int quantity)` -> validates all ingredients, consumes them (e.g. 2 Iron + 4 Coal), grants output (e.g. 1 Iron Ingot).
- **Dependencies**: Step 2, Step 3.

### Step 5: NPC Interaction Choice System (On Press 'E')
- **Files**:
  - `Assets/Scripts/UI/Interaction/NPCInteractionChoiceUI.cs`
  - `Assets/Scripts/Entities/NPCs/NPCDialogueTrigger.cs`
- **Changes**:
  - When player approaches NPC and presses `E`, display choice menu:
    - Button 1: "Razgovor (Talk)" -> opens `DialogueManager` as before.
    - Button 2: "Trgovina (Shop)" -> opens `ShopUIController` with this NPC's `ShopSO`.
    - Button 3: "Odlazak (Leave)" -> closes menu.
  - Lock player movement/actions (`OnActionLockChanged`) while menu is open.
- **Dependencies**: Step 4.

### Step 6: Shop & Crafting UI Interface
- **Files**:
  - `Assets/Scripts/UI/Shop/ShopUIController.cs`
  - `Assets/Scripts/UI/Shop/ShopItemSlotUI.cs`
  - Prefabs/UI in `Assets/Prefabs/UI/Shop/`
- **Changes**:
  - Tab 1: Buy (Market or Equipment)
  - Tab 2: Sell (Sell player resources/items for Gold)
  - Tab 3: Crafting (Blacksmith smelting recipes like 2 Metal + 4 Coal = Iron Ingot)
  - Quantity picker, price calculator, inventory balance live update.
- **Dependencies**: Step 4, Step 5.

### Step 7: Village Scene Setup & NPC Configuration
- **Files**: `Assets/Scenes/Village.unity`
- **Changes**:
  - Configure Market NPC (Market Merchant) with Market `ShopSO` (raw resources buy/sell).
  - Configure Blacksmith NPC with Blacksmith `ShopSO` (weapons/armor buy + smelting/crafting recipes).
  - Add Shop Canvas panel to UI hierarchy.
- **Dependencies**: Step 5, Step 6.

---

# Verification & Testing

1. **NPC Proximity & Input Verification**:
   - Approach NPC -> verify interaction prompt appears.
   - Press `E` -> verify NPC Choice Panel appears with options [Razgovor], [Trgovina], [Odlazak].
   - Verify player cannot move or attack while menu is open.
2. **Market Transactions (Raw Resources)**:
   - Click [Trgovina] on Market NPC -> verify Market inventory opens.
   - Buy Wood/Stone -> verify Gold decreases, resource count in `PlayerResource` increases.
   - Sell Wood/Stone -> verify resource count decreases, Gold increases.
   - Attempt buy with insufficient Gold -> verify transaction is rejected with error feedback.
3. **Smith Crafting (Smelting)**:
   - Click [Trgovina] on Blacksmith NPC -> switch to [Izrada / Crafting] tab.
   - Test recipe: Select `Iron Ingot` (needs 2 Iron + 4 Coal).
   - If player has < 2 Iron or < 4 Coal -> verify Craft button is disabled or gives insufficient resource warning.
   - If player has sufficient materials -> click Craft -> verify 2 Iron and 4 Coal are deducted and 1 Iron Ingot is added to inventory.
4. **Smith Equipment Purchasing**:
   - Browse weapons/armor/tools -> purchase Sword or Armor -> verify gold deducted and item placed in player inventory.
5. **Persistence & Save System**:
   - Perform buy, sell, and craft operations.
   - Reload scene or restart game -> verify gold, resources, and inventory items persist accurately from save data.
