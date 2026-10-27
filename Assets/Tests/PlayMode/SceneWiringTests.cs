using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Environment.Interactables;
using Environment.Rooms;
using Gameplay.Combat;

namespace DragonBlaze.Tests
{
    public class SceneWiringTests
    {
        [UnityTest]
        public IEnumerator Level1_HasHazardsAndExit()
        {
            yield return LoadScene("Level1");

            Assert.GreaterOrEqual(Object.FindObjectsByType<ContactDamage>(FindObjectsSortMode.None).Length, 2,
                "Level1 spike objects lost their damage component");
            Assert.GreaterOrEqual(Object.FindObjectsByType<LevelExit>(FindObjectsSortMode.None).Length, 1,
                "Level1 exit stone lost its script");
        }

        [UnityTest]
        public IEnumerator Level2_HasDoorsWiredToRooms()
        {
            yield return LoadScene("Level2");

            var doors = Object.FindObjectsByType<Door>(FindObjectsSortMode.None);
            Assert.Greater(doors.Length, 0, "Level2 has no doors");

            foreach (var door in doors)
            {
                var serialized = new UnityEditor.SerializedObject(door);
                Assert.IsNotNull(serialized.FindProperty("room").objectReferenceValue,
                    $"{door.name} has no room reference");
            }
        }

        [UnityTest]
        public IEnumerator ElevatorPrefab_MovesItsPlatform()
        {
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Sprites/Pixel Art Platformer - Village Props/Prefab/PF Village Props - Elevator.prefab");
            Assert.IsNotNull(prefab, "elevator prefab missing");

            var instance = Object.Instantiate(prefab);
            try
            {
                var platform = instance.transform.Find("Platform");
                Assert.IsNotNull(platform, "elevator has no Platform child");

                yield return new WaitForSeconds(0.2f);
                float startY = platform.localPosition.y;
                // The elevator waits `waitTime` (default 1s) before moving, so sample past that with margin
                // instead of racing the boundary.
                yield return new WaitForSeconds(2f);

                Assert.AreNotEqual(startY, platform.localPosition.y, "elevator platform never moved");
            }
            finally
            {
                Object.Destroy(instance);
            }
        }

        private static IEnumerator LoadScene(string sceneName)
        {
            var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (load != null && !load.isDone) yield return null;
            yield return null;
        }
    }
}
