# Dragon Blaze

A 2D action platformer with fluid movement, dynamic combat, and modular architecture built in Unity 6.

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square)](https://dacrab.itch.io/unity-2d-platformer)
[![Gameplay Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)
[![Unity](https://img.shields.io/badge/Unity-6000.3.2f1-000000?style=flat-square&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

## Gameplay

- Multi-jump, wall slide, wall jump, dash with coyote time
- Melee and ranged combat with hit feedback
- Enemy AI with patrol/chase states
- Traps, moving platforms, parallax backgrounds
- Checkpoint saves with JSON persistence
- 4 levels

## Controls

| Action | Key |
|--------|-----|
| Move | `A/D` or Arrows |
| Jump | `Space` |
| Attack | `Left Click` |
| Dash | `Shift` |
| Interact | `E` |
| Pause | `Esc` |

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # EventBus, ServiceLocator, StateMachine, Managers
│   ├── Gameplay/       # Player, Enemies, Combat, Items
│   ├── Environment/    # Traps, Platforms, Parallax
│   └── UI/             # HUD, Menus, Dialogue
├── Scenes/             # MainMenu, Level1-4, Credits
├── Prefabs/            # Player, Enemies, Platforms, Traps, UI
└── Animation/          # Player, Enemy, UI animations
```

## Setup

1. Clone: `git clone https://github.com/dacrab/dragon-blaze.git`
2. Open in Unity Hub (6000.3.2f1+)
3. Load `Assets/Scenes/MainMenu.unity`
4. Press Play

## Tech

- Unity 6 with New Input System
- UniTask for async operations
- TextMeshPro for UI

## License

MIT
