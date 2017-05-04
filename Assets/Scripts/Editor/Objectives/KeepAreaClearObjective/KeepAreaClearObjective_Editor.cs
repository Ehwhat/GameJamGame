using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(KeepAreaClearObjective))]
public class KeepAreaClearObjective_Editor : BoundObjective_Editor {

    SerializedProperty _timeGoal;

    new public void OnEnable()
    {
        base.OnEnable();
        _timeGoal = serializedObject.FindProperty(KeepAreaClearObjective.PROPERTY_TIME_GOAL);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        _timeGoal.floatValue = EditorGUILayout.FloatField(new GUIContent("Time Goal"), _timeGoal.floatValue);

        serializedObject.ApplyModifiedProperties();

    }

}
