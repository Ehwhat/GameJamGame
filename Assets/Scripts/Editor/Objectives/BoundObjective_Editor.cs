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

    public void OnSceneGUI()
    {
        serializedObject.Update();
        BoundObjective.BoundsType boundType = (BoundObjective.BoundsType)_objectiveBoundsType.enumValueIndex;
        BoundObjective objective = (BoundObjective)serializedObject.targetObject;
        if (_objectiveBoundsFoldout)
        {
            if (boundType == BoundObjective.BoundsType.Box)
            {
                Handles.DrawWireCube(objective.transform.position, new Vector3(_objectiveBoundsArea.vector2Value.x, 1, _objectiveBoundsArea.vector2Value.y));
                _objectiveBoundsArea.vector2Value = new Vector2(
                    Handles.ScaleSlider(_objectiveBoundsArea.vector2Value.x, objective.transform.position, Vector3.left, Quaternion.identity, HandleUtility.GetHandleSize(objective.transform.position), 0.5f),
                    Handles.ScaleSlider(_objectiveBoundsArea.vector2Value.y, objective.transform.position, Vector3.back, Quaternion.identity, HandleUtility.GetHandleSize(objective.transform.position), 0.5f));
            }
            else if (boundType == BoundObjective.BoundsType.Circle)
            {
                Handles.DrawWireDisc(objective.transform.position,Vector3.up, _objectiveBoundsRadius.floatValue);
                _objectiveBoundsRadius.floatValue = Handles.ScaleSlider(_objectiveBoundsRadius.floatValue, objective.transform.position, Vector3.left, Quaternion.identity, HandleUtility.GetHandleSize(objective.transform.position), 0.5f);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    //

}
