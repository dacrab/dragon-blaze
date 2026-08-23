using UnityEngine;

namespace Core.Events
{
    public readonly struct ScoreChangedEvent
    {
        public readonly int Score;

        public ScoreChangedEvent(int score) => Score = score;
    }

    public readonly struct PlayerDiedEvent { }

    public readonly struct PlayerRespawnEvent { }

    public readonly struct GamePausedEvent
    {
        public readonly bool Paused;

        public GamePausedEvent(bool paused) => Paused = paused;
    }

    public readonly struct LevelCompletedEvent { }

    public readonly struct DialogueStateChangedEvent
    {
        public readonly bool Open;

        public DialogueStateChangedEvent(bool open) => Open = open;
    }

    public readonly struct HealthChangedEvent
    {
        public readonly float Current;
        public readonly float Max;

        public HealthChangedEvent(float current, float max)
        {
            Current = current;
            Max = max;
        }
    }

    public readonly struct PowerUpActivatedEvent
    {
        public readonly string Name;
        public readonly Sprite Icon;
        public readonly float Duration;

        public PowerUpActivatedEvent(string name, Sprite icon, float duration)
        {
            Name = name;
            Icon = icon;
            Duration = duration;
        }
    }
}
