# Dragon Blaze

2D action platformer with fluid movement, combat, and modular architecture.

[![Play](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square)](https://dacrab.itch.io/unity-2d-platformer)
[![Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)

## Features

- Multi-jump, wall slide, wall jump, dash with coyote time
- Melee and ranged combat with hit feedback
- Enemy AI with patrol/chase behaviors
- Traps, moving platforms, parallax backgrounds
- Checkpoint system with JSON save/load
- 4 levels

## Controls

| Action | Key |
|--------|-----|
| Move | `A` `D` / Arrows |
| Jump | `Space` |
| Attack | `LMB` |
| Dash | `Shift` |
| Interact | `E` |
| Pause | `Esc` |

## Tech Stack

- Unity 6 (6000.3.2f1)
- New Input System
- UniTask
- TextMeshPro

## Architecture

```
Scripts/
├── Core/         # EventBus, ServiceLocator, StateMachine<T>, AutoWire, ObjectPool
├── Gameplay/     # Player, Enemies, Combat, Items
├── Environment/  # Traps, Platforms, Rooms, Parallax
└── UI/           # HUD, Menus, Dialogue
```

## Setup

```bash
git clone https://github.com/dacrab/dragon-blaze.git
```

Open in Unity Hub → Load `Assets/Levels/_Menu.unity` → Play

## License

MIT
