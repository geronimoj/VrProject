using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    SerializedProperty pathSearch;
    SerializedProperty formationSearch;
    SerializedProperty spawnTime;
    SerializedProperty position;
    SerializedProperty visible;

    bool showInGuiLocal = false;

    GameObject PathManager;
    GameObject FormationManager;

    private void OnEnable()
    {
        pathSearch = serializedObject.FindProperty("path");
        formationSearch = serializedObject.FindProperty("formation");

        spawnTime = serializedObject.FindProperty("spawnTime");

        position = serializedObject.FindProperty("position");

        visible = serializedObject.FindProperty("visible");

        PathManager = GameObject.Find("PathManager");
        FormationManager = GameObject.Find("FormationManager");
    }

    private void OnSceneGUI()
    {
        if (!showInGuiLocal)
            return;

        Wave wave = target as Wave;
        BezPath path = wave.pathComp;
        Formation form = wave.formComp;

        //Draw handles
        if (form)
        {
            for(int i=0;i<form.spawns.Count;i++)
            {
                Vector3 pos = form.spawns[i];
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.5f, EventType.Repaint);

                //Draw paths
                if (path)
                    for (int j = 1; j < path.points.Length; j++)
                    {
                        Handles.color = Color.white;
                        Vector3 start = path.points[j - 1];
                        Vector3 end = path.points[j];
                        Handles.DrawLine(start + pos, end + pos);
                    }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 50;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.PropertyField(position,GUILayout.Width(200));
        EditorGUIUtility.labelWidth = 30;
        GUILayout.Space(20);
        spawnTime.floatValue = Mathf.Max(EditorGUILayout.FloatField("Time", spawnTime.floatValue, GUILayout.Width(80)), 0);

        EditorGUILayout.Space(20);
        EditorGUIUtility.labelWidth = 45;
        EditorGUILayout.PropertyField(visible);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUIUtility.labelWidth = 60;

        EditorGUILayout.PropertyField(formationSearch, GUILayout.Width(400));
        if (FormationExists())
            EditorGUILayout.LabelField("Found!");
        else
            EditorGUILayout.LabelField("Not found!");

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.PropertyField(pathSearch, GUILayout.Width(400));
        if (PathExists())
            EditorGUILayout.LabelField("Found!");
        else
            EditorGUILayout.LabelField("Not found!");

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        showInGuiLocal = visible.boolValue;
    }

    private bool PathExists()
    {
        if (pathSearch.stringValue == "")
            return false;
        BezPath[] paths = PathManager.GetComponents<BezPath>();
        for (int i = 0; i < paths.Length; i++)
            if (paths[i].pathName == pathSearch.stringValue)
                return true;
        return false;
    }

    private bool FormationExists()
    {
        if (formationSearch.stringValue == "")
            return false;
        Formation[] forms = FormationManager.GetComponents<Formation>();
        for (int i = 0; i < forms.Length; i++)
            if (forms[i].formation == formationSearch.stringValue)
                return true;
        return false;
    }
}
