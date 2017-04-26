using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ObjectiveAbstract), true)]
public abstract class Objective_Editor : Editor {

    bool _objectiveScoreFoldout;

    SerializedProperty _objectiveSuccessScore;
    SerializedProperty _objectiveFailureScore;

    public void OnEnable()
    {
        _objectiveSuccessScore = serializedObject.FindProperty(ObjectiveAbstract.PROPERTY_OBJECTIVE_SUCCESS_SCORE);
        _objectiveFailureScore = serializedObject.FindProperty(ObjectiveAbstract.PROPERTY_OBJECTIVE_FAILURE_SCORE);
    }

    public override void OnInspectorGUI()
    {
        _objectiveScoreFoldout = EditorGUILayout.Foldout(_objectiveScoreFoldout, new GUIContent("Objective Scores"));
        if (_objectiveScoreFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            _objectiveSuccessScore.floatValue = EditorGUILayout.FloatField(new GUIContent("Success Score"), _objectiveSuccessScore.floatValue);
            _objectiveFailureScore.floatValue = EditorGUILayout.FloatField(new GUIContent("Failure Score"), _objectiveFailureScore.floatValue);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
    }



}
