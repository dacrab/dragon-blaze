# Dragon Blaze

A small 2D action platformer for desktop, built in Unity.

Haven't played it yet? There's a build on [itch.io](https://dacrab.itch.io/unity-2d-platformer).

You get a nimble hero, four levels, and plenty of ways to stay in the air — wall slides, wall jumps, a dash, and a couple of extra jumps. Enemies patrol until they spot you, then give chase. Power-ups change how you fight, dialogue moves the story along, and checkpoints keep your progress.

## Controls

| Action | Key |
| --- | --- |
| Move | `A` / `D` or arrow keys |
| Jump | `Space` |
| Attack | Left click |
| Dash | `Shift` |
| Interact | `E` |
| Pause | `Esc` |

## Playing it locally

Install Unity 6 (6000.3.27f1) through Unity Hub, then grab the project:

```bash
git clone https://github.com/dacrab/dragon-blaze.git
```

Open the folder in Unity Hub, load `Assets/Scenes/MainMenu.unity`, and press Play. If Unity asks about the version, `ProjectSettings/ProjectVersion.txt` has the exact one.

## Worth knowing

- Keyboard and mouse only.
- Saves are local JSON files, so nothing leaves your machine.
- Levels, enemies, and how the hero feels are tuned in data assets rather than hard-coded, which makes the game easy to tweak.

Made with Unity 6, the New Input System, and TextMeshPro.

## License

MIT
