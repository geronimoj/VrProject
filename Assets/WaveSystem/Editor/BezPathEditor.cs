using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;

[CustomEditor(typeof(BezPath))]
[CanEditMultipleObjects]
public class BezPathEditor : Editor
{
    SerializedProperty nodes;
    SerializedProperty pathName;
    SerializedProperty visible;
    SerializedProperty type;
    SerializedProperty loopDelay;
    SerializedProperty timers;
    SerializedProperty events;

    bool showInGuiLocal = false;

    GameObject PathManager;
    void OnEnable()
    {
        nodes = serializedObject.FindProperty("nodes");
        pathName = serializedObject.FindProperty("pathName");
        visible = serializedObject.FindProperty("visible");
        type = serializedObject.FindProperty("type");
        loopDelay = serializedObject.FindProperty("loopDelay");
        timers = serializedObject.FindProperty("timerList");
        events = serializedObject.FindProperty("eventList");

        PathManager = GameObject.Find("PathManager");
    }

    private void OnSceneGUI()
    {
        if (!showInGuiLocal||Application.IsPlaying(target))
            return;

        BezPath splinePath = target as BezPath;

        Handles.color = Color.white;

        GameObject obj = splinePath.gameObject;

        Vector3 pos = Vector3.zero;
        if (obj!=PathManager)
            pos = obj.transform.position;

        bool firstNode = true;
        foreach (var node in splinePath.nodes)
        {
            if (firstNode)
            {
                firstNode = false;
                continue;
            }
            EditorGUI.BeginChangeCheck();
            Vector3 p = Handles.PositionHandle(node.geoPos + pos, Quaternion.identity) - pos;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(splinePath, "Move Node");
                EditorUtility.SetDirty(splinePath);
                node.geoPos = p;
            }
        }

        for (int i = 1; i < splinePath.points.Length; i++)
        {
            Handles.color = Color.white;
            Vector3 start = splinePath.points[i - 1];
            Vector3 end = splinePath.points[i];
            Handles.DrawLine(start + pos, end + pos);
            Color orbCol = Color.red / splinePath.points.Length * i;
            orbCol.a = 1;
            Handles.color = orbCol;
            Handles.SphereHandleCap(0, start + pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f, EventType.Repaint);
        }

        splinePath.BakePath();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUIUtility.labelWidth = 80;
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(pathName,GUILayout.Width(200));
        EditorGUILayout.Space(20);
        EditorGUIUtility.labelWidth = 30;
        EditorGUILayout.PropertyField(type,GUILayout.Width(110));
        EditorGUILayout.Space(20);
        if (type.intValue == (int)PathType.LinearLoop || type.intValue == (int)PathType.OddLoop)
            EditorGUILayout.PropertyField(loopDelay, GUILayout.Width(70));
        EditorGUIUtility.labelWidth = 45;
        EditorGUILayout.PropertyField(visible);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        EditorGUIUtility.labelWidth = 15;

        #region Points
        using (SerializedProperty it = nodes.Copy())
        {
            int counter = 0;
            int arrayNum = 0;
            float epsilon = 0.1f;

            float lastTime = 0;
            int nodeNum = 0;

            //EditorGUILayout.BeginVertical(GUILayout.Width(200));

            while (it.Next(true))
            {
                switch (it.type)
                {
                    case "ArraySize":
                        arrayNum++;
                        if (arrayNum ==1)
                        {
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
                        }
                        break;
                    case "float":
                        if (arrayNum == 1)                                                              //If it is the position array
                        {
                            if (nodeNum > 0)                                                            //If it is beyond the first node (as first is locked to 0,0,0,0)
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
                                        break;
                                    case 3:

                                        if (it.floatValue < lastTime + epsilon)
                                            it.floatValue = lastTime + epsilon;
                                        lastTime = it.floatValue;
                                        EditorGUILayout.PropertyField(it, new GUIContent("T"));
                                        EditorGUILayout.EndHorizontal();
                                        break;
                                }
                            }
                            if (counter == 3)
                                nodeNum++;
                            counter = (counter + 1) % 4;
                        }

                        break;


                    case "_":
                        EditorGUILayout.PropertyField(it);

                        break;
                }
            }
        }
        #endregion

        EditorGUILayout.Space(30);

        int eventNum = 0;

        using (SerializedProperty it = events.Copy())
        {
            int arrayNum = 0;
            while (it.Next(true))
            {
                Debug.Log(it.type);
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
                            if (it.intValue > 0)
                                it.intValue--;
                        }
                        if (GUILayout.Button("+"))
                            it.intValue++;
                        eventNum = it.intValue;
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        break;
                    case "_":
                        break;
                }
            }
        }

        #region Events
        EditorGUILayout.BeginHorizontal(GUILayout.Width(500));

        EditorGUILayout.BeginVertical(GUILayout.Width(450));
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Event");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        using (SerializedProperty it = events.Copy())
        {
            int arrayNum = 0;
            while (it.Next(true))
            {
                Debug.Log(it.type);
                switch (it.type)
                {
                    case "UnityEvent":
                        EditorGUILayout.PropertyField(it);
                        break;
                    case "_":
                        break;
                }
            }
        }
        EditorGUILayout.EndVertical();


        EditorGUIUtility.labelWidth = 50;
        EditorGUILayout.BeginVertical(GUILayout.Width(50));
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Time");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        using (SerializedProperty it = timers.Copy())
        {
            int arrayNum = 0;
            while (it.Next(true))
            {
                Debug.Log(it.type);
                switch (it.type)
                {
                    case "ArraySize":
                        arrayNum++;
                        if (arrayNum != 1)
                            break;
                        it.intValue = eventNum;
                        break;
                    case "float":
                        EditorGUILayout.BeginVertical(GUILayout.Height(95));
                        GUILayout.FlexibleSpace();
                        it.floatValue = EditorGUILayout.FloatField(it.floatValue,GUILayout.Height(20),GUILayout.Width(50));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();
                        break;
                    case "_":
                        break;
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
         #endregion

            serializedObject.ApplyModifiedProperties();

        showInGuiLocal = visible.boolValue;
    }
}