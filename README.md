# Dragon Blaze

A 2D action platformer with fluid movement, dynamic combat, and modular architecture built in Unity.

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square)](https://dacrab.itch.io/unity-2d-platformer)
[![Gameplay Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

## Gameplay

- Multi-jump, wall slide, wall jump, dash with coyote time
- Melee and ranged combat with hit feedback
- Enemy AI with patrol/chase states (data-driven via ScriptableObjects)
- Traps, moving platforms, parallax backgrounds
- Checkpoint saves with JSON persistence
- 4 levels with difficulty scaling

## Controls

| Action | Key |
|--------|-----|
| Move | `A/D` or Arrows |
| Jump | `Space` |
| Attack | `Left Click` |
| Dash | `Shift` |
| Interact | `E` |
| Pause | `Esc` |

## Setup

1. Clone: `git clone https://github.com/dacrab/dragon-blaze.git`
2. Open in Unity Hub (6000.3.2f1+)
3. Install Addressables package if prompted
4. Load `Assets/Scenes/MainMenu.unity` and press Play

## Tech

Unity 6 · New Input System · Addressables · Object Pooling · Assembly Definitions · TextMeshPro

## License

MIT
