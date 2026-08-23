# Dragon Blaze — Architecture

Unity **6000.3** (Unity 6). Assembly graph enforced by `asmdef` files.

## Principles
- **Prefer Unity built-ins** over custom code where Unity provides a real feature
  (AudioMixer/AudioSource, SceneManager, ObjectPool, JsonUtility, PlayerPrefs, Resources,
  TMP, Input System). Custom code exists only where Unity has no equivalent:
  the typed `EventBus` and `ServiceLocator`.
- **Data-driven configuration** via `ScriptableObject`s instead of hardcoded values or
  scene-scattered fields.
- **Composition over inheritance**, minimal public surface, sealed classes.
- Everything must be **easily expandable**: adding content means adding data (scenes in
  `levelOrder`, new `PowerUpSO` assets, new `StateSettings`), not wiring new code paths.

## Assembly graph (`Assets/Scripts`)
```
Core  (no scripts depend on it)          EventBus, ServiceLocator, GameConfig, persistence, pools, managers
   ├── Gameplay  (characters, combat, items, dialogue)
   ├── Environment (traps, platforms, parallax)
   └── UI        (managers, menus, HUD)
Debug  (UNITY_EDITOR || DEVELOPMENT_BUILD)  depends on Core + Gameplay
Editor (editor-only tooling)
```
Rules:
- `Gameplay` and `Environment` may reference `Core` only.
- `UI` may reference `Core` + `Gameplay`.
- Never introduce a reference back up the graph (e.g. Gameplay → UI). If a type is needed
  across layers, it lives in `Core` behind an interface.

## Core systems
| System | Type | Purpose |
|---|---|---|
| `EventBus` | static, typed | `Raise<T>/Subscribe<T>/Unsubscribe<T>` for `struct` events (`Core/Events/GameEvents.cs`). One canonical eventing pattern. |
| `ServiceLocator` | static | `Register<T>/Unregister<T>/Get<T>`. Services register in `Awake`, unregister in `OnDestroy`. Reset on domain reload. |
| `GameConfig` | `ScriptableObject` | Singleton loaded from `Resources/GameConfig.asset` via `GameConfig.Default`. Never null at runtime. Holds `levelOrder`, audio keys, save file name, UI thresholds, `StateSettings`. |
| `SaveService` (`Core/Persistence`) | class | Versioned JSON save file. `SaveData.version` defaults to `SaveService.CurrentVersion`; `Load()` runs a `Migrate()` scaffold for forward compatibility. Pure logic, EditMode-testable. |
| `VfxPool` / `GameObjectPool` | static / pools | Built-in `UnityEngine.Pool.ObjectPool` wrappers. |
| `KinematicBody` (`Core.Physics`) | static helpers | Resolves (and promotes Static→Kinematic) a `Rigidbody2D`, then moves bodies via `MovePosition` on the physics step; falls back to transform writes when no body exists. Used by platforms, traps, melee chase, and projectiles — no transform-driven movement of physics objects in `Update`. |
| `RuntimeInitializer` | static | Physics config applied `BeforeSceneLoad`. |

### Persistent managers (DontDestroyOnLoad)
Each registers an interface into `ServiceLocator` and is **self-bootstrapped** via
`[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` so services exist regardless of scene
wiring. A scene instance (with serialized refs) beats the bootstrap via a duplicate guard.

| Manager | Interface | Responsibility |
|---|---|---|
| `GameManager` (`Core.Managers`) | `IGameManager` | Coins, save/load, level-complete handler. |
| `GameStateManager` (`Core.State`) | `IGameStateManager` | State machine driven by `GameConfig.StateSettings`; scene → state by name. |
| `AudioManager` (`Core.Managers`) | `IAudioManager` | Music + pooled one-shot SFX. Optional `AudioMixer` with exposed `MusicVolume`/`SFXVolume`; falls back to per-source volume. |
| `LoadingManager` (`UI.Menus`) | `ISceneLoader` | `LoadScene(name)` / `LoadNextLevel()` with loading-screen progress. |
| `DialogueController` (`Gameplay.Dialogue`) | `IDialogueController` | Typed dialogue queue + typewriter. |
| `UIManager` (`UI.Managers`) | — | UI screens + power-up indicators. |

## Scene navigation
`GameConfig.levelOrder` lists scene **names** in play order
(`MainMenu, Level1, Level2, Level3, Level4, CREDITS`). Navigation is name-based
(`SceneManager.LoadScene(name)`), so **reordering build indices never breaks level flow**.
To add content: add the scene to `levelOrder` + Build Settings; `TryGetNextLevel` drives
"next level" automatically. Index 0 = main menu, index 1 = first level.

## Config → data flow
- `GameConfig.Default` (Resources asset) for global tuning.
- `PlayerConfigSO`, `EnemyConfigSO` for character/enemy stats.
- `PowerUpSO` subclasses (`SpeedPowerUp`, `JumpPowerUp`, `DamagePowerUp`,
  `InvisibilityPowerUp`) describe pickup effects; `PowerUpEffect` pickups reference one.

## Conventions
- **Serialization**: never rename/remove `[SerializeField]` fields or public members that
  scenes/prefabs/assets reference (serialized by name). Never hand-edit scene/prefab YAML
  beyond adding new asset `.meta` files. Move files **with** their `.meta` to keep guids.
- **Style** (`.editorconfig`): 4-space indent, Allman braces, LF, final newline, no tabs.
- **Async**: Unity `Awaitable` + `CancellationTokenSource` for cancellable loops
  (`DialogueController`, i-frames). Fire-and-forget with `_ = MethodAsync()` only when the
  method self-cancels or is short-lived.
- **Events**: gameplay events are `readonly struct`s in `GameEvents.cs`; keep payloads
  value types. Prefer raising events over direct coupling.
- **Registration lifecycle**: register in `Awake`, unregister in `OnDestroy` guarded by a
  reference equality check (`ServiceLocator.Get<T>() == this`).

## Gameplay systems notes
- Damage: single hits go through `CombatExtensions.DamagePlayer` (invisible players are immune); damage-over-time uses `Health.TakeDamagePerSecond`, which bypasses i-frame arming (FireTrap).
- Player power-ups apply timed stat modifiers (`IPlayer.AddModifier`) or cancellable timed invisibility (`IPlayer.SetInvisibilityFor`); both revert on respawn/scene change.
- Room activation is data-driven: `Room.startActive` flags initially live rooms; all other rooms spawn enemies on player entry.
- `Spikehead.maxAttackDistance` bounds each lunge so the trap can never fly off-level.

## Testing
- `Assets/Tests/EditMode` — pure logic: `EventBus`, `ServiceLocator`, `PlayerStats`,
  `GameConfig`, `SaveService`. Run headless with the Unity CLI
  (`unity test --mode EditMode|PlayMode`).
- Add tests for any new pure logic in `Core` (persistence, config, math) so it stays
  editor-independent.

## Startup order (play mode)
1. `SubsystemRegistration` resets static systems (EventBus, ServiceLocator, pools, config cache).
2. `BeforeSceneLoad` bootstraps persistent managers (LoadingManager, AudioManager); physics config applied.
3. Scene `Awake` runs; managers register services; `GameManager` loads save; `GameStateManager` derives state from scene name.