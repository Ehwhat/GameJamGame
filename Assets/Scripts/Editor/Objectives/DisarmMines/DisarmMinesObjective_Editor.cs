using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DisarmMinesObjective))]
public class DisarmMinesObjective_Editor : BoundObjective_Editor {

    SerializedProperty _mineAmount;
    SerializedProperty _minePrefab;

    new public void OnEnable()
    {
        base.OnEnable();
        _mineAmount = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_MINE_AMOUNT);
        _minePrefab = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_MINE_PREFAB);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        _mineAmount.intValue = EditorGUILayout.IntField(new GUIContent("Mine Amount"), _mineAmount.intValue);
        EditorGUILayout.PropertyField(_minePrefab, new GUIContent("Mine Prefab"));

        serializedObject.ApplyModifiedProperties();
    }
}
