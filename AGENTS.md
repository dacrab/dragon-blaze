# AGENTS.md

Guidance for AI coding agents and contributors working in this Unity 6 project.

## Project facts
- Engine: **Unity 6000.3.22f1** (Unity 6), C# with modern syntax (target-typed `new`, switch
  expressions, `Awaitable`, `linearVelocity`, built-in `UnityEngine.Pool.ObjectPool`).
- Local **Unity CLI** is available: `unity test --mode EditMode|PlayMode` (runs the editor in
  batch mode, requires the installed editor + sign-in). Use it to verify compilation and run
  tests; the user also compiles in the Unity editor.
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

## Verification (do this before finishing changes)
1. `rg` for dangling references to renamed/removed members.
2. Formatting: 4-space indent, Allman braces, LF, final newline, no tabs, no trailing
   whitespace (`.editorconfig`).
3. Cross-assembly `using` audit: every namespace must resolve within the referencing
   assembly set.
4. Compile + run tests with the Unity CLI: `unity test --mode EditMode` and
   `unity test --mode PlayMode`. Check the NUnit XML report (`passed`/`failed` counts).
   Then `unity build --target StandaloneLinux64 -o /tmp/opencode/build .` — the player
   build compiles with different defines and has caught errors that editor test runs
   masked (stale assemblies).
5. For pure logic without Unity overhead, a dotnet 9 repro under `/tmp/opencode` is a
   fast first pass, but the CLI test run is authoritative.
6. Flag anything that needs the user's Unity editor (scene wiring, prefab rework,
   AudioMixer asset creation) rather than silently doing risky YAML surgery.

## Commands
The Unity CLI (`unity test --mode EditMode|PlayMode`, `unity build --target StandaloneLinux64
-o <exe> .`) is the build/lint gate; there is no separate project script and **no CI** — the
local CLI run is authoritative. State `git status`/`git diff` on request; never commit unless
explicitly asked.

## Re-enabling CI (removed Aug 2026 as overkill for a solo project)
CI was dropped because tests/builds run locally in minutes and game-ci requires Unity license
secrets for *any* job. To restore: add `.github/workflows/unity.yml` with `game-ci/unity-test-runner@v4`
(matrix EditMode/PlayMode) and `game-ci/unity-builder@v4` (StandaloneLinux64/WebGL), each env-wired to
`UNITY_LICENSE`/`UNITY_EMAIL`/`UNITY_PASSWORD`. Personal-license `.ulf`: dispatch
`game-ci/unity-request-activation-file@v2`, convert the `.alf` at license.unity3d.com/manual (or via
Unity Hub → Manage Licenses → Add), then `gh secret set UNITY_LICENSE < file.ulf`. Both files are in
git history if ever committed; otherwise rewrite from this note.