# Dragon Blaze

A 2D action platformer with fluid movement, dynamic combat, and modular architecture built in Unity 6.

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square)](https://dacrab.itch.io/unity-2d-platformer)
[![Gameplay Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)
[![Unity](https://img.shields.io/badge/Unity-6000.3.2f1-000000?style=flat-square&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

## Gameplay

- Multi-jump, wall slide, wall jump, dash with coyote time
- Melee and ranged combat with hit feedback
- Enemy AI with patrol/chase states (data-driven via ScriptableObjects)
- Traps, moving platforms, parallax backgrounds
- Checkpoint saves with JSON persistence
- Difficulty scaling per level
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

## Architecture

```
Assets/Scripts/
├── Core/                    # Zero gameplay dependencies
│   ├── Analytics/           # Pluggable analytics backends (SO-based)
│   ├── Constants/           # GameConfig, LevelRegistry, GameConstants
│   ├── Events/              # EventBus (typed events + domain reset)
│   ├── Extensions/          # Vector, Transform, Component helpers
│   ├── Input/               # InputReader (ScriptableObject)
│   ├── Loading/             # Addressables AssetLoader
│   ├── Managers/            # GameManager (save/audio)
│   ├── Pooling/             # GameObjectPool + PoolRegistry
│   ├── Services/            # ServiceLocator + interfaces
│   └── State/               # GameStateManager
├── Gameplay/                # Depends on Core only
│   ├── Characters/Enemies/  # EnemyBase, EnemyConfigSO, DifficultyScaling
│   ├── Characters/NPCs/     # TalkableNPC (uses ServiceLocator)
│   ├── Characters/Player/   # Player + PlayerConfigSO
│   ├── Combat/              # Health, ProjectileBase, AttackHitbox
│   └── Items/               # Collectibles, PowerUps
├── Environment/             # Depends on Core + Gameplay
│   ├── Parallax/            # ParallaxBackground
│   ├── Platforms/           # Falling, Sticky, Waypoint
│   ├── Rooms/               # Room activation
│   └── Traps/               # Fire, Arrow, Sideways, Spikehead + TrapConfigSO
├── UI/                      # Depends on Core + Gameplay
│   ├── Dialogue/            # DialogueController (implements IDialogueService)
│   ├── HUD/                 # HealthBar, ScoreDisplay
│   ├── Managers/            # UIManager
│   └── Menus/               # Menu, Loading, Credits
├── Debug/                   # Stripped from release builds (asmdef constraint)
└── Tests/                   # EditMode + PlayMode (NUnit)
```

## Assembly Definitions

| Assembly | Dependencies | Purpose |
|----------|-------------|---------|
| `DragonBlaze.Core` | InputSystem, Addressables | Foundation layer |
| `DragonBlaze.Gameplay` | Core | Game mechanics |
| `DragonBlaze.Environment` | Core, Gameplay | Level elements |
| `DragonBlaze.UI` | Core, Gameplay, TMP | Interface |
| `DragonBlaze.Debug` | Core, Gameplay | Dev cheats (F1-F9) |
| `DragonBlaze.Editor` | All | Editor tools |
| `DragonBlaze.Tests.*` | All | Unit/integration tests |

## Adding Content

| Want to add... | Do this |
|---|---|
| Enemy variant | Duplicate `EnemyConfigSO`, tweak stats |
| New enemy type | Extend `EnemyBase`, create config SO |
| New trap | Create `TrapConfigSO`, optionally new script |
| New level | Add scene to build settings + `LevelRegistrySO` |
| New projectile | Prefab with `ProjectileBase`, set `poolKey` |
| Analytics backend | Extend `AnalyticsBackendSO` |

## Setup

1. Clone: `git clone https://github.com/dacrab/dragon-blaze.git`
2. Open in Unity Hub (6000.3.2f1+)
3. Install Addressables package if prompted
4. Load `Assets/Scenes/MainMenu.unity`
5. Press Play

## Tech

- Unity 6 with `Awaitable` async (no coroutines)
- New Input System (ScriptableObject-based InputReader)
- Addressables for async asset loading
- Object pooling for projectiles/VFX
- Assembly definitions for compile-time dependency enforcement
- TextMeshPro for UI

## Debug Keys (Editor/Development builds only)

| Key | Action |
|-----|--------|
| F1 | Heal player |
| F2 | +100 coins |
| F3 | Kill all enemies |
| F4 | Toggle invincibility |
| F5 | Force save |
| F9 | Toggle 3x speed |

## License

MIT
