using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ObjectiveExit))]
public class ObjectiveExitEditor : Objective_Editor
{

    SerializedProperty _exitStand;
    SerializedProperty _exitTime;
    SerializedProperty _innerProjector;
    SerializedProperty _outerProjector;

    new public void OnEnable()
    {
        base.OnEnable();
        _exitStand = serializedObject.FindProperty(ObjectiveExit.PROPERTY_EXIT_STAND);
        _exitTime = serializedObject.FindProperty(ObjectiveExit.PROPERTY_EXIT_WAIT);
        _innerProjector = serializedObject.FindProperty(ObjectiveExit.PROPERTY_INNER_PROJECTOR);
        _outerProjector = serializedObject.FindProperty(ObjectiveExit.PROPERTY_OUTER_PROJECTOR);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_exitStand, new GUIContent("Exit Stand"));
        _exitTime.floatValue = EditorGUILayout.FloatField(new GUIContent("Exit Time"), _exitTime.floatValue);
        EditorGUILayout.PropertyField(_innerProjector, new GUIContent("Inner Projector"));
        EditorGUILayout.PropertyField(_outerProjector, new GUIContent("Outer Projector"));
        
        

        serializedObject.ApplyModifiedProperties();
    }
}
