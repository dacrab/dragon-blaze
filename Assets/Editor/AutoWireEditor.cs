using UnityEngine;
using UnityEditor;
using Core.Utilities;

namespace Core.Utilities.Editor
{
    /// <summary>
    /// Editor tool to auto-wire all references in selected GameObjects.
    /// Access via: Tools > Auto-Wire Selected
    /// </summary>
    public class AutoWireEditor : EditorWindow
    {
        [MenuItem("Tools/Auto-Wire/Selected GameObjects")]
        public static void AutoWireSelected()
        {
            var selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("Auto-Wire", "Please select GameObjects in the scene or hierarchy.", "OK");
                return;
            }

            int wiredCount = 0;
            foreach (var go in selected)
            {
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        AutoWireHelper.WireAllFields(component);
                        wiredCount++;
                    }
                }
                EditorUtility.SetDirty(go);
            }

            Debug.Log($"[AutoWire] Wired {wiredCount} components on {selected.Length} GameObjects");
            EditorUtility.DisplayDialog("Auto-Wire", $"Successfully wired {wiredCount} components!", "OK");
        }

        [MenuItem("Tools/Auto-Wire/All in Scene")]
        public static void AutoWireAllInScene()
        {
            if (!EditorUtility.DisplayDialog("Auto-Wire All", 
                "This will auto-wire all components in the current scene. Continue?", "Yes", "No"))
            {
                return;
            }

            var allComponents = Object.FindObjectsByType<Component>(FindObjectsSortMode.None);
            int wiredCount = 0;

            foreach (var component in allComponents)
            {
                if (component != null)
                {
                    AutoWireHelper.WireAllFields(component);
                    wiredCount++;
                }
            }

            Debug.Log($"[AutoWire] Wired {wiredCount} components in scene");
            EditorUtility.DisplayDialog("Auto-Wire", $"Successfully wired {wiredCount} components in scene!", "OK");
        }

        [MenuItem("Tools/Auto-Wire/Selected GameObjects", true)]
        [MenuItem("Tools/Auto-Wire/All in Scene", true)]
        public static bool ValidateMenu()
        {
            return Application.isPlaying == false; // Only available in edit mode
        }
    }
}

