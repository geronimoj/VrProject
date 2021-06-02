﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyReticleSystem))]
[CanEditMultipleObjects]
public class ReticleEditor : Editor
{
    SerializedProperty origin;
    SerializedProperty shape;
    SerializedProperty radius;
    SerializedProperty billboard;
    SerializedProperty prefab;
    SerializedProperty scale;
    private void OnEnable()
    {
        origin = serializedObject.FindProperty("_reticleOrigin");
        shape = serializedObject.FindProperty("_shape");
        radius = serializedObject.FindProperty("_radius");
        billboard = serializedObject.FindProperty("_billboardReticles");
        prefab = serializedObject.FindProperty("_reticlePrefab");
        scale = serializedObject.FindProperty("_reticleScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(origin);
        EditorGUILayout.PropertyField(shape);

        EnemyReticleSystem sys = target as EnemyReticleSystem;
        if (sys.Shape == EnemyReticleSystem.ReticleShape.Circle)
            EditorGUILayout.PropertyField(radius);

        EditorGUILayout.PropertyField(billboard);
        EditorGUILayout.PropertyField(prefab);
        EditorGUILayout.PropertyField(scale);
        serializedObject.ApplyModifiedProperties();
    }
}