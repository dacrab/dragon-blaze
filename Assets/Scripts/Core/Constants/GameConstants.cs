namespace Core.Constants
{
    public static class GameConstants
    {
        public static class Tags
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string Checkpoint = "Checkpoint";
        }

        public static class Animation
        {
            public const string Grounded = "grounded";
            public const string Run = "run";
            public const string Die = "die";
            public const string Respawn = "respawn";
            public const string MeleeAttack = "meleeAttack";
            public const string Moving = "moving";
            public const string Hurt = "hurt";
        }

        public static class Scenes
        {
            public const int MainMenu = 0;
            public const int FirstLevel = 1;
        }

        public static class Save
        {
            public const string SaveFileName = "/savefile.json";
            public const string MusicVolume = "musicVolume";
            public const string SoundVolume = "soundVolume";
        }
    }

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        Dialogue,
        GameOver,
        Loading
    }
}
