using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BoundObjective), true)]
public class BoundObjective_Editor : Objective_Editor  {

    bool _objectiveBoundsFoldout = false;

    SerializedProperty _objectiveBoundsType;
    SerializedProperty _objectiveBoundsArea;
    SerializedProperty _objectiveBoundsRadius;

    new public void OnEnable()
    {
        base.OnEnable();
        _objectiveBoundsType = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_BOUNDS_TYPE);
        _objectiveBoundsRadius = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_BOUNDS_RADIUS);
        _objectiveBoundsArea = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_BOUNDS_BOX);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _objectiveBoundsFoldout = EditorGUILayout.Foldout(_objectiveBoundsFoldout, new GUIContent("Objective Bounds"));
        if (_objectiveBoundsFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_objectiveBoundsType, new GUIContent("Bounds Type"));
            EditorGUILayout.BeginHorizontal();

            BoundObjective.BoundsType boundType = (BoundObjective.BoundsType)_objectiveBoundsType.enumValueIndex;
            if (boundType == BoundObjective.BoundsType.Box)
            {
                _objectiveBoundsArea.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Bounds Area"), _objectiveBoundsArea.vector2Value);
            }else if(boundType == BoundObjective.BoundsType.Circle)
            {
                _objectiveBoundsRadius.floatValue = EditorGUILayout.FloatField(new GUIContent("Bounds Radius"), _objectiveBoundsRadius.floatValue);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
    }

    //

}
