using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Formation))]
public class FormationEditor : Editor
{
    SerializedProperty formationName;
    SerializedProperty spawnPositions;
    SerializedProperty spawnObject;
    SerializedProperty visible;

    bool showInGuiLocal = false;

    Formation obj;
    private void OnEnable()
    {
        formationName = serializedObject.FindProperty("formation");
        spawnPositions = serializedObject.FindProperty("spawns");
        spawnObject = serializedObject.FindProperty("spawnObject");
        visible = serializedObject.FindProperty("visible");

        obj = target as Formation;
    }

    private void OnSceneGUI()
    {
        if (!showInGuiLocal)
            return;

        for(int i=0; i<obj.spawns.Count;i++)
        {
            Vector3 pos = obj.spawns[i];
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos)*0.5f, EventType.Repaint);
            EditorGUI.BeginChangeCheck();

            Vector3 p = Handles.PositionHandle(pos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "Move Node");
                EditorUtility.SetDirty(obj);
                obj.spawns[i] = p;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty it = spawnPositions.Copy();
        int arrayNum = 0;
        int counter = 0;

        EditorGUIUtility.labelWidth = 80;
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(formationName, GUILayout.Width(200));
        EditorGUILayout.Space(20);
        EditorGUIUtility.labelWidth = 45;
        EditorGUILayout.PropertyField(visible);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUIUtility.labelWidth = 80;
        EditorGUILayout.PropertyField(spawnObject, GUILayout.Width(200));

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 15;

        while (it.Next(true))
        {
            switch (it.type)
            {
                case "ArraySize":
                    arrayNum++;
                    if (arrayNum != 1)
                        break;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-"))
                    {
                        if (it.intValue > 1)
                            it.intValue--;
                    }
                    if (GUILayout.Button("+"))
                        it.intValue++;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    break;

                case "float":
                    if (arrayNum == 1)
                    {
                        switch (counter)
                        {
                            case 0:
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(it, new GUIContent("X"));
                                break;
                            case 1:
                                EditorGUILayout.PropertyField(it, new GUIContent("Y"));
                                break;
                            case 2:
                                EditorGUILayout.PropertyField(it, new GUIContent("Z"));
                                EditorGUILayout.EndHorizontal();
                                break;
                        }
                        counter = (counter + 1) % 3;
                    }
                    break;

                case "_": break;
            }
        }

        serializedObject.ApplyModifiedProperties();

        showInGuiLocal = visible.boolValue;
    }
}
