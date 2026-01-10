using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void RemoveMissing()
    {
        var objs = Resources.FindObjectsOfTypeAll<GameObject>();
        int count = 0;
        foreach (var obj in objs)
        {
            if (obj.hideFlags != HideFlags.None) continue;
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            if (removed > 0)
            {
                count += removed;
                EditorUtility.SetDirty(obj);
            }
        }
        Debug.Log($"Removed {count} missing scripts");
    }
}
