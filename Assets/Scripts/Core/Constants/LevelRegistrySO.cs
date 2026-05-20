using UnityEngine;
using System.Collections.Generic;

namespace Core.Constants
{
    [System.Serializable]
    public class LevelEntry
    {
        public string displayName;
        public int sceneIndex;
        public Sprite thumbnail;
        public bool unlocked = true;
        [TextArea] public string description;
    }

    [CreateAssetMenu(fileName = "LevelRegistry", menuName = "DragonBlaze/Config/Level Registry")]
    public class LevelRegistrySO : ScriptableObject
    {
        [SerializeField] List<LevelEntry> levels = new();

        public IReadOnlyList<LevelEntry> Levels => levels;
        public int Count => levels.Count;

        public LevelEntry GetLevel(int index) =>
            index >= 0 && index < levels.Count ? levels[index] : null;

        public int GetNextSceneIndex(int currentSceneIndex)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].sceneIndex == currentSceneIndex && i + 1 < levels.Count)
                    return levels[i + 1].sceneIndex;
            }
            return -1;
        }

        public LevelEntry FindBySceneIndex(int sceneIndex) =>
            levels.Find(l => l.sceneIndex == sceneIndex);
    }
}
