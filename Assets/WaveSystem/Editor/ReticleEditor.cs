using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyReticleSystem))]
[CanEditMultipleObjects]
public class ReticleEditor : Editor
{
    private SerializedProperty layers = null;
    private SerializedProperty useAsDisplay = null;
    private SerializedProperty adDisplay = null;
    private SerializedProperty origin = null;
    private SerializedProperty shape = null;
    private SerializedProperty radius = null;
    private SerializedProperty billboard = null;
    private SerializedProperty keepTracking = null;
    private SerializedProperty prefab = null;
    private SerializedProperty scale = null;
    private void OnEnable()
    {
        layers = serializedObject.FindProperty("m_quadLayers");
        useAsDisplay = serializedObject.FindProperty("_useAsDisplay");
        origin = serializedObject.FindProperty("_reticleOrigin");
        adDisplay = serializedObject.FindProperty("_additionalDisplays");
        shape = serializedObject.FindProperty("_shape");
        radius = serializedObject.FindProperty("_radius");
        billboard = serializedObject.FindProperty("_billboardReticles");
        prefab = serializedObject.FindProperty("_reticlePrefab");
        scale = serializedObject.FindProperty("_reticleScale");
        keepTracking = serializedObject.FindProperty("_trackRemovedEnemies");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(layers);
        EditorGUILayout.PropertyField(origin);
        EditorGUILayout.PropertyField(shape);
        EditorGUILayout.PropertyField(adDisplay);
        EditorGUILayout.PropertyField(useAsDisplay);

        EnemyReticleSystem sys = target as EnemyReticleSystem;
        if (sys.Shape == EnemyReticleSystem.ReticleShape.Circle)
            EditorGUILayout.PropertyField(radius);

        EditorGUILayout.PropertyField(billboard);
        EditorGUILayout.PropertyField(keepTracking);
        EditorGUILayout.PropertyField(prefab);
        EditorGUILayout.PropertyField(scale);
        serializedObject.ApplyModifiedProperties();
    }
}
