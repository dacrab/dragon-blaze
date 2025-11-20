# 🐉 Dragon Blaze

*An enchanting 2D platformer featuring fluid movement, engaging combat, and a polished architecture built with Unity.*

<div align="center">
  
[![Play Now](https://img.shields.io/badge/Play%20Now-itch.io-FA5C5C?style=for-the-badge&logo=itch.io)](https://dacrab.itch.io/unity-2d-platformer)
[![Watch Gameplay](https://img.shields.io/badge/Watch_Gameplay-4285F4?style=for-the-badge&logo=google-drive&logoColor=white)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view?usp=drive_link)

</div>

## ✨ Features

### 🎮 Core Gameplay
- **Fluid Movement System**: Built on a custom `PlayerController` featuring coyote time, multi-jumps, wall sliding, and dynamic dashing.
- **Combat System**: Ranged and melee combat mechanics with responsive hit detection and visual feedback.
- **Enemy AI**: Diverse enemy types including patrolling melee guards and ranged spellcasters using finite state machine logic.
- **Interaction System**: Quest-giving NPCs, dialogue systems, and interactive world objects like Magic Stones.

### 🛠️ Technical Systems
- **Robust Architecture**: Clean, domain-driven project structure separating `Core` systems, `Gameplay` logic, `Environment` interactions, and `UI`.
- **Event-Driven Architecture**: Decoupled systems using a central `EventBus` for UI updates, audio triggers, and game state changes.
- **Save & Persistence**: Reliable JSON-based save system tracking player progress, currency, and level state.
- **Object Pooling**: Optimized projectile and particle spawning for smooth performance.
- **Audio Management**: Centralized `SoundManager` with volume controls and persistence.

### 🌍 Environment
- **Dynamic World**: Parallax background scrolling with depth and mouse-follow effects.
- **Traps & Hazards**: Variety of traps including Firetraps, Arrow Dispensers, Spikeheads, and Falling Platforms.
- **Level Design**: Seamless room transitions and checkpoint systems.

## 🚀 Architecture Overview

The project follows a modern, modular folder structure for better scalability:

```
Assets/Scripts/
├── Core/           # Singleton managers, Input, Events, Persistence
├── Gameplay/       # Characters (Player/Enemy), Combat, Items, Health
├── Environment/    # Traps, Platforms, Parallax, Room logic
└── UI/             # HUD, Menus, Dialogue systems
```

## 🎮 Controls

| Action | Input |
|--------|-------|
| **Move** | Arrow Keys / A & D |
| **Jump** | Spacebar |
| **Attack** | Left Mouse Button |
| **Dash** | Left Shift (while moving) |
| **Interact** | E Key |
| **Wall Slide** | Hold direction against wall |
| **Pause** | Escape |

## 📦 Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/dacrab/dragon-blaze.git
   ```
2. Open the project in Unity (Recommended Version: 2022.3 LTS or newer).
3. Open the `_Menu` scene in `Assets/Levels`.
4. Press Play!

## 🗺️ Roadmap

- [x] **Core Mechanics**: Movement, Combat, Dash, Wall Slide.
- [x] **Currency System**: Coin collection and persistence.
- [x] **Save System**: Checkpoints and data serialization.
- [ ] **Shop System**: Spend collected coins on upgrades.
- [ ] **Boss Battles**: Epic encounters with complex patterns.
- [ ] **Achievements**: Steam/Platform integration.

## 👥 Credits

- **Developer**: [DaCrab](https://github.com/dacrab)
- **Engine**: [Unity Technologies](https://unity.com/)
- **Assets**: [Unity Asset Store](https://assetstore.unity.com/)
- **Sound**: [Freesound](https://freesound.org/)

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📬 Contact

For bug reports or suggestions: [vkavouras@proton.me](mailto:vkavouras@proton.me)
