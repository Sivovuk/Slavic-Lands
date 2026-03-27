

Let me explore the full project structure and read through all the scripts to understand the architecture before writing the documentation.Now let me read through all the key script files to understand the architecture:Now let me read the remaining key files to complete my understanding. Based on everything I've gathered, here's a comprehensive README for your GitHub repository:

---

# 🏰 Slavic Lands

> A 2D action-adventure game built in **Unity 6** (6000.3.10f1), set in a world inspired by Slavic mythology. Explore the wilderness, gather resources, fight mythological creatures, dive into dungeons, and interact with villagers — all rendered with the **Universal Render Pipeline (URP)**.

![Unity](https://img.shields.io/badge/Unity-6000.3-blue?logo=unity)
![C#](https://img.shields.io/badge/C%23-9.0-purple?logo=csharp)
![URP](https://img.shields.io/badge/Render%20Pipeline-URP-green)
![Platform](https://img.shields.io/badge/Platform-PC-lightgrey)

---

## 📖 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Core Systems](#core-systems)
- [Data-Driven Design](#data-driven-design)
- [How to Run](#how-to-run)

---

## Overview

**Slavic Lands** is a personal portfolio project demonstrating proficiency in Unity game development, C# programming, and software architecture patterns. The game features a player character who can explore an open world, harvest resources (trees, rocks), engage in melee and ranged combat, use special abilities, level up multiple skills, enter procedural-style dungeons, and interact with NPCs through a dialogue system.

The project emphasizes **clean code architecture**, **data-driven design via ScriptableObjects**, **interface-based abstraction**, and a **modular component system** — all hallmarks of production-ready game development practices.

---

## Features

| Feature | Description |
|---|---|
| ⚔️ **Combat System** | Melee attacks, ranged bow combat, and special abilities (Slash, Shield Bash, Piercing Arrow) |
| 🏃 **Player Movement** | Walk, run, jump, and dash with energy management |
| 🌲 **Resource Gathering** | Harvest trees with axes and mine rocks with pickaxes; tool-specific interactions |
| 📈 **Leveling & Progression** | Per-skill XP and leveling (combat, cutting, mining, archery) with stat scaling |
| 🐺 **Entity AI** | Animals and NPCs with idle, patrol, chase, and attack behaviors |
| 🏚️ **Dungeon System** | Dungeon entrances in the world leading to multi-level encounters with mobs and loot |
| 💬 **Dialogue System** | NPC interaction with a trigger-based dialogue manager |
| 💾 **Save System** | Persistent player data saving and loading |
| 🖥️ **HUD & UI** | Health/energy bars, resource display, player profile panel with skill/ability details, tab-based menus |

---

## Architecture

The project follows a **component-based architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────┐
│                  Managers Layer                  │
│         GameManager · DialogueManager           │
│         DungeonManager · SaveManager            │
├─────────────────────────────────────────────────┤
│                Gameplay Layer                    │
│  Player (Controller, Combat, Health, Energy,    │
│          Movement, Input, Profile)              │
│  Entities (Base → Animals, NPCs)                │
│  Environment (Resources)                        │
│  Combat (Projectiles)                           │
│  World (Dungeon Controllers)                    │
├─────────────────────────────────────────────────┤
│               Core Layer                        │
│  Interfaces: IDamageable, IInteractable,        │
│              ILevelData, ILoadingStatsPlayer,    │
│              ILoadStatsResource, ILoadStatsEntity│
│  Constants                                      │
├─────────────────────────────────────────────────┤
│              Data Layer (ScriptableObjects)      │
│  PlayerSO · EntitySO · ResourceSO · XPDataSO   │
│  PlayerProfileSO · DialogueDataSO               │
└─────────────────────────────────────────────────┘
```


### Key Design Principles

- **Interface Segregation** — Core interfaces (`IDamageable`, `IInteractable`, `ILevelData`) ensure loose coupling between systems. Any object that can be damaged implements `IDamageable`, whether it's a player, animal, NPC, or resource node.
- **Inheritance Hierarchy** — Entities use a base class pattern (`EntityBase` → `Animal` / `NPC`; `EntityMovement` → `AnimalMovement` / `NPCMovement`; `EntityAttack` → `AnimalAttack` / `NPCAttack`), promoting code reuse while allowing specialized behavior.
- **ScriptableObject-Driven Configuration** — All stats, drops, XP values, and dialogue are defined in ScriptableObjects, keeping data out of code and enabling designer-friendly tuning without recompilation.
- **Component Composition** — The player is composed of focused components (`PlayerMovement`, `PlayerCombat`, `PlayerHealth`, `PlayerEnergy`, `PlayerInputSystem`, `PlayerProfile`) rather than a monolithic script.

---

## Project Structure

```
Assets/
├── Animations/          # Animator controllers and animation clips
├── Data/                # ScriptableObject asset instances
│   ├── Entity/Animals/  # Per-animal stat definitions
│   ├── Resource/        # Resource nodes, XP data, player resources
│   └── DialogueVillager # NPC dialogue data
├── Prefabs/
│   ├── Player/          # Player character prefab
│   ├── Animals/         # Bear, Wolf, Stag, Rabbit, Eagle
│   ├── Mob/             # Mythological enemies (Drekavac)
│   ├── Village/         # House, Wall, Villager NPCs
│   ├── Dungeon/         # Entrance, ground, level prefabs
│   ├── Ammo/            # Arrow, Piercing Arrow projectiles
│   ├── Enviroment/      # Trees, rocks, terrain
│   └── UI/              # HUD, panels, profile buttons
├── Scenes/              # Game scenes
├── Scripts/             # All C# source code (see below)
├── Sprites/             # 2D art assets
├── Settings/            # URP and project settings
└── _ThirdParty/         # External plugins and assets
```


### Scripts Breakdown

| Folder | Purpose |
|---|---|
| `Scripts/Core/Interfaces/` | Core abstractions — `IDamageable`, `IInteractable`, `ILevelData`, stat-loading interfaces |
| `Scripts/Core/Constants/` | String constants for save keys and shared values |
| `Scripts/Data/` | ScriptableObject definitions for players, entities, resources, XP, dialogues |
| `Scripts/Player/Controller/` | Player components: `PlayerController`, `PlayerMovement`, `PlayerCombat`, `PlayerHealth`, `PlayerEnergy` |
| `Scripts/Player/Input/` | Input abstraction layer (`PlayerInputSystem`) |
| `Scripts/Player/Profile/` | Player progression: `PlayerProfile`, `PlayerResource` |
| `Scripts/Entities/Base/` | Abstract entity classes: `EntityBase`, `EntityMovement`, `EntityAttack` |
| `Scripts/Entities/Animals/` | Animal-specific: `Animal`, `AnimalMovement`, `AnimalAttack` |
| `Scripts/Entities/NPCs/` | NPC-specific: `NPC`, `NPCMovement`, `NPCAttack`, `NPCDialogueTrigger` |
| `Scripts/Combat/Projectiles/` | Projectile behavior (`ArrowProjectile`) |
| `Scripts/Environment/Resources/` | Harvestable world objects (`Resource`) |
| `Scripts/World/Dungeon/` | Dungeon system: `DungeonManager`, `DungeonEntranceController`, `DungeonMobController`, `DungeonResourcesController` |
| `Scripts/Managers/` | Singletons/managers: `GameManager`, `DialogueManager` |
| `Scripts/SaveSystem/` | Persistence: `SaveManager`, `PlayerSaveData` |
| `Scripts/UI/HUD/` | Runtime HUD: `HUDController`, `ResourceDisplay` |
| `Scripts/UI/Game/` | Menus & panels: `GameUIController`, `TabController`, `PlayerProfileUI`, `SkillUIBlock`, `DungeonLevelsUI` |
| `Scripts/_Debug/` | Development-only debug tools |

---

## Core Systems

### 🎮 Player System
The player is built from **five focused components** managed by a central `PlayerController`:
- **PlayerMovement** — Walk, run, jump, and dash mechanics with energy consumption
- **PlayerCombat** — Melee (axe/battle axe) and ranged (bow) attacks, plus three special abilities: *Slash*, *Shield Bash*, and *Piercing Arrow*, each with unique knockback forces
- **PlayerHealth** — Implements `IDamageable`; manages health with knockback response
- **PlayerEnergy** — Energy pool for dashing and abilities
- **PlayerInputSystem** — Decoupled input handling layer

### 🐾 Entity System (Animals & NPCs)
All entities extend from a shared base:
- **EntityBase** — Core entity logic (health, level, data loading via `ILoadStatsEntity`)
- **EntityMovement** — Patrol/idle/chase state-driven movement with configurable walk ranges and speeds
- **EntityAttack** — Attack logic with cooldowns, detection radius, and attack range

Animals (Bear, Wolf, Stag, Rabbit, Eagle) and NPCs (Villagers) specialize these base classes for their unique behaviors. NPCs additionally support dialogue through `NPCDialogueTrigger`.

### ⛏️ Resource System
Resources (trees, rocks) implement `IDamageable` and use `ResourceSO` to define:
- Required **tool type** (Axe, Pickaxe)
- **Health** (hit count before destruction)
- **Drop tables** with resource type and amount
- **XP rewards** on harvest

### 🏔️ Dungeon System
A complete dungeon loop:
- **DungeonEntranceController** — World-space entry points triggering dungeon loading
- **DungeonManager** — Orchestrates dungeon levels, progression, and exit
- **DungeonMobController** — Spawns and manages enemies within dungeon levels
- **DungeonResourcesController** — Handles dungeon-specific loot and resources
- **DungeonLevelsUI** — UI representation of dungeon floor progression

### 💬 Dialogue System
- **DialogueDataSO** — Designer-friendly multi-line dialogue authored in the inspector
- **NPCDialogueTrigger** — Proximity-based trigger on NPC entities
- **DialogueManager** — Centralized manager that queues and displays sentences

### 💾 Save System
- **PlayerSaveData** — Serializable data structure for player state
- **SaveManager** — Handles read/write operations for persistence
- Uses defined **Constants** (`SavedHealth`, `SavedEnergy`, `PlayerLevel`) as save keys

### 📊 Progression & XP
- Per-tool/ability XP tracking via `XPDataSO` (attack, shoot, cut, mine, slash, shield bash, piercing arrow)
- `ILevelData` interface for any levelable entity
- Player profile tracks **skill levels** and **ability levels** separately
- Stats scale with a configurable **level multiplier** (default 1.2×)

---

## Data-Driven Design

All game-balance values are defined as **ScriptableObject assets**, not hardcoded:

| ScriptableObject | Purpose |
|---|---|
| `PlayerSO` | Player movement speeds, dash parameters, health, energy, all damage values, ability stats, level scaling |
| `EntitySO` | Entity idle/walk/run speeds, patrol ranges, attack damage/cooldown/range, detection radius, health, level scaling |
| `ResourceSO` | Tool requirements, node health, drop tables (type + amount), XP rewards |
| `XPDataSO` | XP values per tool/ability action |
| `PlayerProfileSO` | Player level data, skill XP entries, ability level data |
| `DialogueDataSO` | Multi-line dialogue sentences for NPC interactions |

This approach allows **tuning without touching code** and supports rapid iteration.

---

## How to Run

1. **Clone** the repository
2. Open with **Unity 6** (version 6000.3.10f1 or compatible)
3. Ensure **Universal Render Pipeline** is enabled (configured in `Assets/Settings/`)
4. Open the main scene from `Assets/Scenes/`
5. Press **Play** ▶️

### Requirements
- Unity 6000.3.x
- .NET Framework 4.7.1 target
- C# 9.0

---

## 🛠️ Technical Highlights

- **Unity 6** with **URP** for optimized 2D rendering
- **C# 9.0** features (pattern matching with `switch` expressions, `field:` serialization attributes)
- **Interface-driven architecture** enabling polymorphic damage, interaction, and data-loading systems
- **OOP inheritance hierarchy** for entities with clean base → specialization patterns
- **ScriptableObject pipeline** for fully data-driven game design
- **Component composition** over monolithic scripts
- **Namespace organization** mirroring folder structure for clean code navigation

---

*Built with ❤️ and a fascination for Slavic mythology.*

---

Feel free to customize the badges, add screenshots/GIFs of gameplay, and include a license section. This README is structured to give recruiters a clear picture of both **what the game does** and—more importantly—**how it's engineered** under the hood. 🎯