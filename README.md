# Dragon Blaze

A 2D action platformer built with Unity 6, featuring responsive movement mechanics, combat systems, and a clean modular architecture.

[![Play on itch.io](https://img.shields.io/badge/Play-itch.io-FA5C5C?style=flat-square&logo=itch.io)](https://dacrab.itch.io/unity-2d-platformer)
[![Gameplay Video](https://img.shields.io/badge/Gameplay-Video-4285F4?style=flat-square&logo=google-drive)](https://drive.google.com/file/d/1A_-qFr5LuwZUnVla1aqEab6fWn9i16fv/view)

## Gameplay

- **Movement**: Run, multi-jump, wall slide, wall jump, and dash
- **Combat**: Melee attacks and ranged projectiles with hit feedback
- **Enemies**: Patrolling melee guards and ranged spellcasters with FSM-based AI
- **Environment**: Traps (fire, arrows, spikes), falling platforms, and parallax backgrounds
- **Progression**: 4 levels with checkpoints, coin collection, and save/load system

## Controls

| Action | Input |
|--------|-------|
| Move | `A`/`D` or Arrow Keys |
| Jump | `Space` |
| Attack | Left Mouse |
| Dash | `Shift` (while moving) |
| Interact | `E` |
| Pause | `Escape` |

## Architecture

```
Assets/Scripts/
├── Core/           # Reusable framework systems
│   ├── Combat/     # DamageInfo, damage types
│   ├── Events/     # EventBus for decoupled communication
│   ├── Input/      # InputReader (new Input System)
│   ├── Interfaces/ # IDamageable, IPoolable, IService
│   ├── Managers/   # SingletonManager<T>, GameManager, SoundManager
│   ├── Optimization/   # ObjectPoolManager (Unity ObjectPool<T>)
│   ├── Persistence/    # JSON save system
│   ├── Services/   # ServiceLocator for DI
│   ├── State/      # Generic StateMachine<T>
│   └── Utilities/  # AutoWire system, extensions
├── Gameplay/       # Game-specific logic
│   ├── Characters/ # Player, Enemies, NPCs
│   ├── Combat/     # Projectiles, hitboxes
│   ├── Health/     # Health component
│   └── Items/      # Collectables, powerups
├── Environment/    # World systems
│   ├── Parallax/   # Background scrolling
│   ├── Platforms/  # Moving/falling platforms
│   ├── Rooms/      # Level transitions
│   └── Traps/      # Hazards with TrapBase
└── UI/             # HUD, menus, dialogue
```

### Key Patterns

**AutoWire** - Automatic dependency injection via attributes:
```csharp
[AutoWire(AutoWireAttribute.WireType.Self)]
[SerializeField] private Rigidbody2D rb;

[AutoWire(AutoWireAttribute.WireType.Scene)]
[SerializeField] private UIManager uiManager;

private void Awake() => AutoWireHelper.WireAllFields(this);
```

**EventBus** - Decoupled event communication:
```csharp
// Subscribe
EventBus.OnPlayerDied += HandleDeath;

// Publish
EventBus.RaisePlayerDied();
```

**ServiceLocator** - Runtime service access:
```csharp
ServiceLocator.Register<IAudioService>(soundManager);
var audio = ServiceLocator.Get<IAudioService>();
```

**StateMachine<T>** - Generic FSM for entities:
```csharp
stateMachine.RegisterState(EnemyState.Patrol, new PatrolState());
stateMachine.SetInitialState(EnemyState.Patrol);
stateMachine.ChangeState(EnemyState.Chase);
```

## Requirements

- Unity 6000.3+ (Unity 6)
- Input System package
- TextMeshPro
- UniTask (async/await)

## Setup

```bash
git clone https://github.com/dacrab/dragon-blaze.git
```

1. Open in Unity Hub
2. Load `Assets/Levels/_Menu.unity`
3. Press Play

## Project Status

**Implemented:**
- Core movement (coyote time, wall mechanics, dash)
- Combat with IDamageable interface and DamageInfo
- Enemy AI with patrol and chase behaviors
- Event-driven architecture
- Object pooling for projectiles
- Save/load system
- 4 playable levels

**Planned:**
- Player state machine integration
- Shop system for upgrades
- Boss encounters
- Achievement system

## License

MIT License - see [LICENSE](LICENSE)

## Contact

[vkavouras@proton.me](mailto:vkavouras@proton.me)
