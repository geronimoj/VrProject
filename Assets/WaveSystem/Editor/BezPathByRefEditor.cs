using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezPathByRef))]
[CanEditMultipleObjects]
public class BezPathByRefEditor : Editor
{
    SerializedProperty pathSearch;
    SerializedProperty pathComp;
    SerializedProperty visible;
    bool showInGuiLocal = false;
    GameObject PathManager;
    void OnEnable()
    {
        pathComp = serializedObject.FindProperty("pathRef");
        pathSearch = serializedObject.FindProperty("path");
        visible = serializedObject.FindProperty("visible");

        PathManager = GameObject.Find("PathManager");
    }

    private void OnSceneGUI()
    {
        BezPathByRef pathRef = target as BezPathByRef;
        BezPath splinePath = pathRef.pathRef;

        GameObject obj = pathRef.gameObject;
        Vector3 pos = obj.transform.position;

        if (showInGuiLocal)
            if (splinePath)
                for (int i = 1; i < splinePath.points.Length; i++)
                {
                    Handles.color = Color.white;
                    Vector3 start = splinePath.points[i - 1];
                    Vector3 end = splinePath.points[i];
                    Handles.DrawLine(start + pos, end + pos);
                }

    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 60;
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(pathSearch,GUILayout.Width(200));
        EditorGUILayout.Space(20);
        EditorGUIUtility.labelWidth = 45;
        EditorGUILayout.PropertyField(visible);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        showInGuiLocal = visible.boolValue;

        BezPathByRef pathRef = target as BezPathByRef;
        if (!PathExists())
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("NOT FOUND!");
            if (GUILayout.Button("Create"))
            {
                BezPath newPath = PathManager.AddComponent<BezPath>();
                newPath.pathName = pathSearch.stringValue;
                newPath.visible = false;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(pathComp);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Localize"))
            {
                var newPath = (CopyComponent(pathRef.pathRef, pathRef.gameObject) as BezPath);
                newPath.pathName += " (local)";
                pathRef.gameObject.AddComponent<DeleteComponent>().componentReference = pathRef;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private bool PathExists()
    {
        BezPath[] paths = PathManager.GetComponents<BezPath>();
        for (int i = 0; i < paths.Length; i++)
            if (paths[i].pathName == pathSearch.stringValue)
                return true;
        return false;
    }

    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}