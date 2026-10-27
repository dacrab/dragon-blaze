# AGENTS.md

Guidance for AI coding agents and contributors working in this Unity 6 project.

## Project facts
- Engine: **Unity 6000.3.27f1** (Unity 6), C# with modern syntax (target-typed `new`, switch
  expressions, `Awaitable`, `linearVelocity`, built-in `UnityEngine.Pool.ObjectPool`). The version
  pinned in `ProjectSettings/ProjectVersion.txt` is authoritative if this line disagrees.
- Verify with the local **Unity CLI** (see Commands).
- See `ARCHITECTURE.md` for the system design.

## Hard rules
- **Never rename or remove serialized fields / public members** that scenes, prefabs, or
  `.asset` files reference (Unity serializes by name). Moving files requires moving their
  `.meta` (guid-preserving) or scene/prefab/asset references break.
- **Never hand-edit scene/prefab `.unity`/`.prefab` YAML** for wiring. Prefer code changes
  that work with whatever wiring exists, and tell the user when editor work is required.
- **Respect the asmdef graph**: Gameplay/Environment → Core, UI → Core+Gameplay.
  Gameplay must never reference UI. Put cross-layer contracts in `Core` as interfaces.
- Keep one canonical eventing pattern: **`EventBus`** (typed struct events). Don't reintroduce
  `EventChannel` or ad-hoc static singletons — persistent systems register via `ServiceLocator`.

## Patterns to follow
- Register/unregister lifecycle: `ServiceLocator.Register<T>(this)` in `Awake`,
  `if (ReferenceEquals(ServiceLocator.Get<T>(), this)) ServiceLocator.Unregister<T>()` in `OnDestroy`.
- Access config via `GameConfig.Default` (Resources singleton; never null at runtime).
- Navigate scenes by **name** (`ISceneLoader.LoadScene(name)` / `LoadNextLevel()`), never
  by build index. Add new levels to `GameConfig.levelOrder` in the `Resources/GameConfig.asset`.
- Audio: `ServiceLocator.Get<IAudioManager>()?.PlaySound(clip)`. Do not add new AudioSource
  wiring to GameManager — audio lives in `AudioManager`.
- Persistent managers self-bootstrap with `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`
  plus a duplicate guard in `Awake`, so services exist regardless of scene placement.
- Async: `Awaitable` + `CancellationTokenSource`. Cancel in `OnDisable`/destruction paths.
- Pure logic (persistence, config, stats, math) should be testable without Unity scene
  overhead and get EditMode tests in `Assets/Tests/EditMode`.
- Keep changes small. No new abstractions or scripts without a concrete bug they fix.

## Verification (do this before finishing changes)
1. `rg` for dangling references to renamed/removed members.
2. Formatting: 4-space indent, Allman braces, LF, final newline, no tabs, no trailing
   whitespace (`.editorconfig`).
3. `unity test --mode EditMode`, `unity test --mode PlayMode`, then
   `unity build --target StandaloneLinux64 -o /tmp/opencode/build .`. The player build is the
   real gate: it compiles with different defines and catches errors editor runs mask.
4. Flag anything that needs the user's Unity editor (scene wiring, prefab rework) rather than
   silently doing risky YAML surgery.

## Commands
`unity test --mode EditMode|PlayMode` and `unity build --target StandaloneLinux64 -o <exe> .`
are the build/test gate. There is no CI and no project script — the local run is authoritative.
Report `git status`/`git diff` on request; never commit unless explicitly asked.
