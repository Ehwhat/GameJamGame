using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DisarmMinesObjective))]
public class DisarmMinesObjective_Editor : BoundObjective_Editor {

    SerializedProperty _mineAmount;

    new public void OnEnable()
    {
        base.OnEnable();
        _mineAmount = serializedObject.FindProperty(DisarmMinesObjective.PROPERTY_MINE_AMOUNT);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        

        serializedObject.ApplyModifiedProperties();
    }
}
