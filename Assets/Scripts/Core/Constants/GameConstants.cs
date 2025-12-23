using UnityEngine;

namespace Core.Constants
{
    /// <summary>
    /// Centralized game constants and enums for maintainability and type safety.
    /// </summary>
    public static class GameConstants
    {
        #region Tags
        public static class Tags
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string Ground = "Ground";
            public const string Checkpoint = "Checkpoint";
            public const string Collectible = "Collectible";
            public const string Projectile = "Projectile";
            public const string Trap = "Trap";
        }
        #endregion

        #region Layers
        public static class Layers
        {
            public const string Ground = "Ground";
            public const string Default = "Default";
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string Projectile = "Projectile";
            
            public static int GetLayerMask(string layerName)
            {
                return LayerMask.GetMask(layerName);
            }
        }
        #endregion

        #region Animation Parameters
        public static class Animation
        {
            public const string Grounded = "grounded";
            public const string Run = "run";
            public const string Die = "die";
            public const string Respawn = "respawn";
            public const string MeleeAttack = "meleeAttack";
            public const string Moving = "moving";
            public const string Hurt = "hurt";
            public const string Jump = "jump";
            public const string Dash = "dash";
        }
        #endregion

        #region Input
        public static class Input
        {
            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
        }
        #endregion

        #region Scenes
        public static class Scenes
        {
            public const int MainMenu = 0;
            public const int FirstLevel = 1;
        }
        #endregion

        #region Save System
        public static class Save
        {
            public const string SaveFileName = "/savefile.json";
            public const string MusicVolume = "musicVolume";
            public const string SoundVolume = "soundVolume";
            public const string MasterVolume = "masterVolume";
        }
        #endregion

        #region Physics
        public static class Physics
        {
            public const float DefaultGravity = -9.81f;
            public const int MaxCollisionChecks = 10;
        }
        #endregion

        #region Timing
        public static class Timing
        {
            public const float DefaultIFrameDuration = 1.0f;
            public const int DefaultFlashCount = 5;
        }
        #endregion
    }

    #region Enums
    /// <summary>
    /// Game state enumeration for state management
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        Dialogue,
        GameOver,
        Loading
    }

    /// <summary>
    /// Audio type enumeration
    /// </summary>
    public enum AudioType
    {
        Music,
        Sound,
        Master
    }

    /// <summary>
    /// Damage type enumeration for combat system
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magic,
        Fire,
        Ice,
        Poison
    }

    /// <summary>
    /// Power-up type enumeration
    /// </summary>
    public enum PowerUpType
    {
        SpeedBoost,
        HigherJump,
        Invisibility,
        DamageBoost,
        HealthBoost
    }
    #endregion
}
