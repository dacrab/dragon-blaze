# Project Refactor Repair Guide

Following the modernization to Unity 6000 and the cleanup of legacy scripts, some references in your Scenes and Prefabs have broken because the script files were moved or replaced. Unity identifies these as "Missing Script" or "Unknown".

## 1. Fix "Missing Script" on GameObjects

You will see "The referenced script on this Behaviour (Game Object 'X') is missing!" in the Console.

### **Object: `Saw` (and other moving traps)**
*   **Issue**: The old `Enemy_Sideways` script was deleted/renamed.
*   **Fix**:
    1.  Select the **Saw** object in your Scene or Prefab.
    2.  Remove the "Missing Script" component.
    3.  Add the **EnemySideways** component (`Assets/Scripts/Environment/Traps/EnemySideways.cs`).
    4.  Assign a `TrapStatsSO` if you have one, or it will use defaults.

### **Object: `ArrowTrap`, `Firetrap`, `Spikehead`**
*   **Issue**: Scripts may have been moved or meta files recreated.
*   **Fix**:
    1.  Check these objects for "Missing Script".
    2.  Re-attach the corresponding script (`ArrowTrap`, `Firetrap`, `Spikehead` found in `Assets/Scripts/Environment/Traps/`).

### **Object: `MovingPlatform`**
*   **Issue**: Meta file reference may be broken.
*   **Fix**:
    1.  Check platforms for "Missing Script".
    2.  Re-attach **MovingPlatform** or **BoundingPlatform** from `Assets/Sprites/Pixel Art Platformer - Village Props/Script/`.

### **Object: `txt ( : )` (Score Display)**
*   **Issue**: The old `ScoreDisplay` script was moved/renamed.
*   **Fix**:
    1.  Select the **txt ( : )** object in the Canvas/HUD.
    2.  Remove the "Missing Script" component.
    3.  Add the **ScoreDisplay** component (`Assets/Scripts/UI/HUD/ScoreDisplay.cs`).
    4.  Drag the **TextMeshPro - Text (UI)** component from the same object into the `Coin Text` field of the script.

## 2. Fix Player Setup

You are seeing "PlayerController component not found on Player!".

*   **Issue**: `PlayerMovement` was removed and replaced by a modular `PlayerController` system.
*   **Fix**:
    1.  Select your **Player** GameObject.
    2.  Remove the missing `PlayerMovement` component.
    3.  Add the following components:
        *   **PlayerController**
        *   **PlayerLocomotion**
        *   **PlayerVisuals**
        *   **PlayerAudio** (optional)
        *   **PlayerPowerups**
    4.  **Important**: In the **PlayerController** component inspector, locate the `Input Reader` field.
    5.  Find the `InputReader` asset in your project (search for t:InputReader) and drag it into this slot.

## 3. Fix "Stats SO missing" Warnings

You are seeing "Stats SO missing on MeleeEnemy".

*   **Issue**: The `EnemyBase` script expects a `CharacterStatsSO` to define health and damage.
*   **Fix**:
    1.  Select the **MeleeEnemy** prefab or instance.
    2.  Locate the **CharacterStatsSO** field.
    3.  Find a suitable stats asset (e.g., `EnemyStats_Melee`) in your project and assign it.
    4.  If none exists, you can create one (Right Click > Create > Gameplay > CharacterStatsSO) or ignore the warning (defaults will be used).

## 4. Fix Package Manager "Immutable Folder" Error

You are seeing "Couldn't delete ... because it's in an immutable folder".

*   **Issue**: The Unity Package Cache in `Library/` is corrupted or has permissions issues.
*   **Fix**:
    1.  **Close Unity**.
    2.  Delete the `Library` folder in your project root. (This is safe; Unity regenerates it).
    3.  Re-open Unity. It will re-download packages and rebuild the database.

