# Dragon Blaze

A 2D action platformer featuring fluid movement mechanics, dynamic combat system, and modular architecture built in Unity 6.

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square)](https://dacrab.itch.io/unity-2d-platformer)
[![Gameplay Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)
[![Unity Version](https://img.shields.io/badge/Unity-6000.3.2f1-000000?style=flat-square&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

## Features

### Movement & Combat
- **Advanced Platforming**: Multi-jump, wall slide, wall jump, dash with coyote time
- **Dynamic Combat**: Melee and ranged attacks with visual/audio hit feedback
- **Responsive Controls**: Tight, fluid character movement

### Game Systems
- **Smart Enemy AI**: Patrol and chase behaviors with state management
- **Interactive Environment**: Traps, moving platforms, parallax backgrounds
- **Progression System**: Checkpoint-based saves with JSON persistence
- **Content**: 4 complete levels with varied challenges

## Controls

| Action | Input | Alternative |
|--------|-------|-------------|
| Move | `A` `D` | Arrow Keys |
| Jump | `Space` | - |
| Attack | `Left Mouse` | - |
| Dash | `Shift` | - |
| Interact | `E` | - |
| Pause | `Escape` | - |

## Requirements

- **Unity**: 6000.3.2f1 or later
- **Platform**: Windows, macOS, Linux
- **Input**: Keyboard + Mouse

## Tech Stack

- **Engine**: Unity 6 (6000.3.2f1)
- **Input**: New Input System
- **Async**: UniTask for performance
- **UI**: TextMeshPro for text rendering

## Architecture

```
Scripts/
├── Core/         # EventBus, ServiceLocator, StateMachine<T>, AutoWire, ObjectPool
├── Gameplay/     # Player, Enemies, Combat, Items
├── Environment/  # Traps, Platforms, Rooms, Parallax
└── UI/           # HUD, Menus, Dialogue
```

**Design Patterns Used:**
- Event-driven architecture with EventBus
- Dependency injection via ServiceLocator
- Generic state machines for AI and player states
- Object pooling for performance optimization

## Getting Started

### Prerequisites
- Unity Hub installed
- Unity 6000.3.2f1 or compatible version

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/dacrab/dragon-blaze.git
   cd dragon-blaze
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and select the project folder
   - Wait for Unity to import assets

3. **Play the game**
   - Load `Assets/Levels/_Menu.unity`
   - Press Play button in Unity Editor

### Building

1. Go to **File → Build Settings**
2. Select your target platform
3. Click **Build** or **Build and Run**

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
