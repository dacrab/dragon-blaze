# dragon-blaze — Type-Safety Hardening (verified via Unity CLI)

All changes verified headlessly with Unity 6000.3.23f1 batchmode:

- **EditMode**: 26/26 tests passed — 0 compile errors, 0 compiler warnings
- **PlayMode**: 7/7 tests passed (incl. `Instance_ResolvesFromResources`, `EnableGameplayInput_DoesNotThrow` which exercise the InputReader changes at runtime) — 0 errors, 0 warnings

## What changed

**NRT enabled project-wide** — `Assets/csc.rsp` with `-nullable:enable` for all player assemblies. The codebase's 90 hand-maintained `?.` null contracts are now compiler-enforced.

**Nullability made truthful (~40 files)**: nullable events, singleton caches, `SaveService.Load() → SaveData?`, `ServiceLocator.Get<T>() → T?`, `GameConfig.MainMenuSceneName/FirstLevelSceneName → string?`, `VfxPool.Spawn → GameObject?`, `KinematicBody.Prepare → Rigidbody2D?` + nullable `MoveTo(rb)`, `ProjectileBase.Fire → ProjectileBase?`, nullable `IAudioManager.PlaySound(clip)`/`PlayMusic(clip)`, `ISceneLoader.LoadScene(string?)` with null-name fallback in LoadingManager, nullable fields across all consumers. No `#pragma` suppressions; two documented `!` (VfxPool prefab-invariant; NUnit `Assert.Ignore` flow) and test-idiom `null!` setup fields.

**Runtime hardening (pre-existing audit fixes, unchanged)**:

- `InputReader`: action lookups via `FindAction` (null-returning API — the map indexer throws `KeyNotFoundException`) with `Debug.LogError` + skip on missing actions, preventing whole-input NREs on asset/code drift
- `GameManager`: `TotalCoins` clamped to ≥ 0 on load; saved `levelName` validated against `GameConfig.levelOrder` with first-level fallback
- `LoadingManager`: missing scene falls back to the first level instead of stranding the player
- `EnemyBase`/`ParallaxBackground`: prefab component contracts null-checked with `Debug.LogError` at startup

**Compile fixes surfaced by the first-ever full batchmode compile under NRT**:

- `VfxPool.cs`: missing `using System;` (`OperationCanceledException`); `UnityEngine.Object` disambiguated
- PowerUps (Damage/Invisibility/Jump/Speed): missing `using UnityEngine;` (`CreateAssetMenu` unresolved)
- `EventBus.Unsubscribe`: now removes empty delegate entries instead of storing null (compiler-caught latent bug)
- `MeleeEnemy`/`RangedEnemy`: null-guarded `config`/`anim`/`playerTransform` derefs

**Editor-generated artifacts**: `Assets/csc.rsp.meta`, missing `.cs.meta` files (Core/Physics, MultiplierPowerUp), and Unity's own `packages-lock.json` refresh (com.unity.2d.common 12.0.3 → 12.0.4) are committed alongside.

## Verification commands

```bash
"$UNITY" -batchmode -nographics -runTests -testPlatform EditMode -projectPath . -testResults editmode.xml -logFile editmode.log
"$UNITY" -batchmode -nographics -runTests -testPlatform PlayMode -projectPath . -testResults playmode.xml -logFile playmode.log
# EditMode: 26 passed / 0 failed; PlayMode: 7 passed / 0 failed; grep 'error CS|warning CS' both logs → 0 / 0
```
