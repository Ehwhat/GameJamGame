using UnityEngine;
using UnityEditor;
using System.Collections;

public static class ListDrawer {

    public static void Show(SerializedProperty list, GUIContent label)
    {
        if (list == null)
        {
            EditorGUILayout.LabelField(label);
            EditorGUILayout.HelpBox("InspectorUtility.ListDrawer.Show(): Cannot draw list with label[" + label.text + "] as the list is null!", MessageType.Error);
        }
        else if (!list.isArray)
        {
            EditorGUILayout.LabelField(label);
            EditorGUILayout.HelpBox("InspectorUtility.ListDrawer.Show(): Cannot draw " + list.name + " as it is not an array or list.", MessageType.Error);
        }
        else
        {
            // Draw the header for the array
            EditorGUILayout.BeginHorizontal();
            list.isExpanded = EditorGUILayout.Foldout(list.isExpanded, label);
            list.arraySize = EditorGUILayout.IntField("Size:", list.arraySize);
            if (GUILayout.Button("+", EditorStyles.miniButton))
            {
                list.InsertArrayElementAtIndex(0);
            }
            EditorGUILayout.EndHorizontal();

            // Draw individual elements
            ShowInInspector(list, label);
        }
    }

    private static void ShowInInspector(SerializedProperty list, GUIContent label)
    {
        if (list.isExpanded)
        {
            ++EditorGUI.indentLevel;
            if (list.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Editing multiple objects!", MessageType.Warning);
            }
            for (int i = 0; i < list.arraySize; ++i)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);

                bool destroyThis = false;
                EditorGUILayout.BeginHorizontal();
                if (item == null)
                {
                    EditorGUILayout.LabelField("Error: Element is null!", GUI.skin.GetStyle("HelpBox"));
                    ShowListButtons(list, i, out destroyThis);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, "[" + i.ToString() + "]");
                    ShowListButtons(list, i, out destroyThis);
                    EditorGUILayout.EndHorizontal();
                    if (item.isExpanded)
                    {
                        EditorGUILayout.PropertyField(item, GUIContent.none);
                    }
                }


                if (destroyThis)
                {
                    int old_size = list.arraySize;
                    list.DeleteArrayElementAtIndex(i);
                    if (old_size == list.arraySize)
                    {
                        // References are only cleared when deleted at first, so we delete again to remove the element itself
                        list.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            --EditorGUI.indentLevel;
        }
    }

    private static void ShowListButtons(SerializedProperty list, int arrayIndex, out bool destroyThis)
    {
        bool gui_enabled = GUI.enabled;
        if (arrayIndex < 1)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("/\\", EditorStyles.miniButtonLeft))
        {
            list.MoveArrayElement(arrayIndex, arrayIndex - 1);
        }
        GUI.enabled = gui_enabled;

        if (arrayIndex == list.arraySize - 1)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("\\/", EditorStyles.miniButtonRight))
        {
            list.MoveArrayElement(arrayIndex, arrayIndex + 1);
        }
        GUI.enabled = gui_enabled;

        if (GUILayout.Button("-", EditorStyles.miniButtonLeft))
        {
            destroyThis = true;
        }
        else
        {
            destroyThis = false;
        }
        if (GUILayout.Button("+", EditorStyles.miniButtonRight))
        {
            list.InsertArrayElementAtIndex(arrayIndex);
        }
    }

}
