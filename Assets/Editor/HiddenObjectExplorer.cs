using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HiddenObjectExplorer : EditorWindow
{
    [MenuItem("Tools/HiddenObjectExplorer")]
    static void Init() => GetWindow<HiddenObjectExplorer>();

    readonly List<GameObject> objects = new();
    readonly HashSet<GameObject> seen = new();
    Vector2 scrollPosition;

    void OnEnable() => FindObjects();

    void FindObjects()
    {
        objects.Clear();
        seen.Clear();
        foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            var root = obj.transform.root.gameObject;
            if (seen.Add(root)) objects.Add(root);
        }
    }

    void FindObjectsAll()
    {
        objects.Clear();
        seen.Clear();
        objects.AddRange(Resources.FindObjectsOfTypeAll<GameObject>());
    }

    HideFlags HideFlagsButton(string title, HideFlags flags, HideFlags value)
    {
        if (GUILayout.Toggle((flags & value) != 0, title, "Button"))
            flags |= value;
        else
            flags &= ~value;
        return flags;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("find top level")) FindObjects();
        if (GUILayout.Button("find ALL object")) FindObjectsAll();
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj == null) continue;

            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(obj.name, obj, typeof(GameObject), true);
            var flags = obj.hideFlags;
            flags = HideFlagsButton("HideInHierarchy", flags, HideFlags.HideInHierarchy);
            flags = HideFlagsButton("HideInInspector", flags, HideFlags.HideInInspector);
            flags = HideFlagsButton("DontSave", flags, HideFlags.DontSave);
            flags = HideFlagsButton("NotEditable", flags, HideFlags.NotEditable);
            obj.hideFlags = flags;
            GUILayout.Label(((int)flags).ToString(), GUILayout.Width(20));
            GUILayout.Space(20);
            if (GUILayout.Button("DELETE"))
            {
                DestroyImmediate(obj);
                FindObjects();
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}
