# WIP: manual wiring checklist

This branch is a large refactor + bug-fix pass. All code is verified by the Unity CLI
(`EditMode` 34/34, `PlayMode` 12/12, `StandaloneLinux64` build succeeds), but a few things
could not be proven headlessly and need a human in the editor.

Open the project, load `MainMenu`, press Play, then walk a level.

## 1. Things I already wired in prefab YAML (please eyeball these)

These were edited as text. They load and the asset structure validates (no dangling or
duplicate fileIDs), but visual placement is unverified.

| Prefab | Change | What to check |
|---|---|---|
| `Assets/Prefabs/Canvas/Core.prefab` | Added a `CoinCounter` TMP object under `UICanvas`, pointed `UIManager.coinText` at it | Coin count visible top-left in every level; correct font, readable, not clipped |
| `Assets/Prefabs/Canvas/Core.prefab` | `HealthBar.currentHealthBar` now points at `HealthbarCurrent` | Health bar moves when you take damage (it was dead: `{fileID: 0}` before) |
| `Assets/Prefabs/Canvas/Core.prefab` | `UIManager.indicatorPrefab` now references `PowerUpIndicator.prefab` | Pick up a power-up: an indicator appears and fades (panel is auto-created at runtime) |
| `Assets/Prefabs/Canvas/Canvas.prefab` | Moved `DialogueController` from `DialogueBoxIMG` (inactive) up to the active `Canvas` object; wired `nameText`, `dialogueText`, `dialogueBox` | Talk to an NPC: box opens **and** closes, name + text show, typewriter runs |

`DialogueBoxIMG` is still `m_IsActive: 0` in the prefab — that is now correct, the controller
toggles it. If dialogue opens but stays blank, the TMP refs are the thing to check.

Backups of the pre-edit prefabs are in `/tmp/opencode/prefab-backup/` (may be gone after a
reboot; use git history instead).

## 2. Still needs wiring by hand

### Loading screen in levels
`UIManager.loadingScreen` and `loadingImage` are `{fileID: 0}` in `Core.prefab`, so
level→level and level→menu transitions show no loading bar. I did **not** add these because
there is no loading UI object inside `Core.prefab` to point at — the whole loading screen only
exists inside `MainMenu.unity`.

Options:
- Add a loading screen object to `Core.prefab` and wire the two fields, or
- Move the loading UI to a persistent canvas shared by all scenes.

Code side is ready: `LoadingManager` will show/hide it and can no longer get stuck visible.

### Power-up indicator panel (optional)
`UIManager.indicatorPanel` is intentionally left empty. A serialized panel inside a per-scene
canvas gets destroyed on scene load while `UIManager` persists, so the manager now creates its
own container under the nearest `Canvas` at runtime. Wire it only if you want indicators
anchored somewhere specific; the field overrides the auto-created panel.

### Continue button
`UIManager.continueButton` is `{fileID: 0}` in `Core.prefab` (correct — Continue only belongs
in the main menu, where `MainMenu.unity` wires it). Leave it.

## 3. Behaviour changes worth knowing

- **`ContactDamage` now hits once on entry.** It used to apply `damage` every physics step
  (~50×/sec at 50 Hz). If `Spikes.prefab` felt tuned to the old behaviour, retune
  `damage`. For a *continuous* hazard use `ContactDamagePerSecond` (value is per second).
- **`GameConfig.menuScenes`** is new (`MainMenu`, `Credits`). Any scene listed there gets the
  free-cursor UI state instead of gameplay state. Add new menu/outro scenes there.
- **`Elevator` now moves in `FixedUpdate` via `KinematicBody`** instead of writing
  `localPosition` in `Update` under a kinematic body. It should still visibly bob up/down; if
  it looks steppy, set Rigidbody2D **Interpolation** on the platform.
- **Respawn without a checkpoint** now returns you to the level's spawn point instead of
  leaving you dead while the UI resumes play.
- **Debug F4 is now real god mode** (`Health.SetInvulnerable`), not invisibility. F9 speed no
  longer unpauses the game.

## 4. Not done (deliberately)

- No gameplay is added; this is correctness + structure only.
- `DebugConsole` still self-bootstraps in the editor (asmdef keeps it out of release builds),
  so cheat keys are live while you play-test. That is unchanged.
- Level scenes were not re-authored. If `ContactDamage`/`Elevator` tuning changes how a level
  feels, adjust the prefabs/instances rather than the scripts.
