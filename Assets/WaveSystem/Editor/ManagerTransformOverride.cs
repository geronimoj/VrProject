using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(Transform),true)]
public class ManagerTransformOverride : Editor
{
    bool manager = false;

    Editor defaultEditor;

    GameObject parent;

    Manager managerObj;

    Vector3 hidePos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    private void OnEnable()
    {
        defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
        parent = (target as Transform).gameObject;
        managerObj = parent.GetComponent<Manager>();
    }

    void OnDisable()
    {
        //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        //Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
            disableMethod.Invoke(defaultEditor, null);
        DestroyImmediate(defaultEditor);
    }
    public override void OnInspectorGUI()
    {
        manager = parent.gameObject.CompareTag("Manager");

        if (manager)
        {
            if (!managerObj)
            {
                managerObj = parent.AddComponent<Manager>();
                managerObj.hideFlags = HideFlags.HideInInspector;
                managerObj.storedPos = parent.transform.position;
                parent.transform.position = hidePos;
            }
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorStyles.label.alignment = TextAnchor.MiddleCenter;
            EditorStyles.label.fixedWidth = 500;
            EditorStyles.label.fixedHeight = 40;
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.LabelField("Manager GUI Obscured. Please ensure the GameObject is top level, that it has no children, and that no references to this transform are made!");

            EditorStyles.label.alignment = TextAnchor.MiddleLeft;
            EditorStyles.label.fixedWidth = 0;
            EditorStyles.label.fixedHeight = 0;
            EditorStyles.label.wordWrap = false;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (managerObj)
            {
                parent.transform.position = managerObj.storedPos;
                if (!parent.GetComponent<DeleteComponent>())
                    parent.AddComponent<DeleteComponent>().componentReference = managerObj;
                managerObj = null;
            }
            defaultEditor.OnInspectorGUI();
        }

    }
}
