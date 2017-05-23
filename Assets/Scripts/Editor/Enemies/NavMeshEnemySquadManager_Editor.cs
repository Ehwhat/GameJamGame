using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavMeshEnemySquadManager))]
public class NavMeshEnemySquadManager_Editor : Editor {

    SerializedProperty _spawnOnStart;
    SerializedProperty _enemies;
    SerializedProperty _patrolPath;
    SerializedProperty _squadPatrolType;
    SerializedProperty _squadSpawnType;
    SerializedProperty _squadSpawnPoint;

    public void OnEnable()
    {
        _spawnOnStart = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_SPAWN_ON_START);
        _patrolPath = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_PATROL_PATH);
        _enemies = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_ENEMIES);
        _squadPatrolType = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_SQUAD_PATROL_TYPE);
        _squadSpawnType = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_SQUAD_SPAWN_TYPE);
        _squadSpawnPoint = serializedObject.FindProperty(NavMeshEnemySquadManager.PROPERTY_SQUAD_SPAWN_POINT);
    }

    void OnDisable()
    {
        Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _spawnOnStart.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Spawn On Start"), _spawnOnStart.boolValue);

        EditorGUILayout.HelpBox("Enemy prefabs will be spawned, and enemy instances will just be where they already are", MessageType.Info);
        ListDrawer.Show(_enemies, new GUIContent("Enemies"));
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(_squadPatrolType, new GUIContent("Patrol Type"));
        NavMeshEnemySquadManager.PatrolType patrolType = (NavMeshEnemySquadManager.PatrolType)_squadPatrolType.enumValueIndex;
        if (patrolType == NavMeshEnemySquadManager.PatrolType.usePrefabricatedPath)
        {
            
            ListDrawer.Show(_patrolPath, new GUIContent("Patrol Path"));
        }else
        {
            Tools.hidden = false;
        }
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(_squadSpawnType, new GUIContent("Spawn Type"));
        NavMeshEnemySquadManager.SpawnPosition spawnType = (NavMeshEnemySquadManager.SpawnPosition)_squadSpawnType.enumValueIndex;
        if(spawnType == NavMeshEnemySquadManager.SpawnPosition.PrefabricatedPosition)
        {
            _squadSpawnPoint.isExpanded = true;
            _squadSpawnPoint.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Squad Spawn Point"), _squadSpawnPoint.vector3Value);
        }else
        {
            _squadSpawnPoint.isExpanded = false;
        }

        serializedObject.ApplyModifiedProperties();

    }

    public void OnSceneGUI()
    {
        Tools.hidden = false;
        serializedObject.Update();
        if (_patrolPath.isExpanded)
        {
            if (_patrolPath.arraySize > 1)
            {
                Tools.hidden = true;
                SerializedProperty point;
                for (int i = 0; i < _patrolPath.arraySize - 1; i++)
                {
                    point = _patrolPath.GetArrayElementAtIndex(i);
                    Handles.color = Color.blue;
                    Handles.SphereHandleCap(0, point.vector3Value, Quaternion.identity, HandleUtility.GetHandleSize(point.vector3Value) / 5, EventType.Repaint);
                    Handles.Label(point.vector3Value, new GUIContent("Patrol Point " + i));
                    Handles.DrawDottedLine(point.vector3Value, _patrolPath.GetArrayElementAtIndex(i + 1).vector3Value, 4);

                    if (point.isExpanded)
                    {
                        point.vector3Value = Handles.PositionHandle(point.vector3Value, Quaternion.identity);
                    }

                }
                point = _patrolPath.GetArrayElementAtIndex(_patrolPath.arraySize - 1);
                Handles.SphereHandleCap(0, point.vector3Value, Quaternion.identity, HandleUtility.GetHandleSize(point.vector3Value) / 5, EventType.Repaint);
                Handles.Label(point.vector3Value, new GUIContent("Patrol Point " + (_patrolPath.arraySize - 1)));

                if (point.isExpanded)
                {
                    point.vector3Value = Handles.PositionHandle(point.vector3Value, Quaternion.identity);
                }

                Handles.DrawDottedLine(point.vector3Value, _patrolPath.GetArrayElementAtIndex(0).vector3Value, 4);
            }else if(_patrolPath.arraySize > 0)
            {
                Tools.hidden = true;
                Handles.color = Color.blue;
                SerializedProperty point = _patrolPath.GetArrayElementAtIndex(0);
                Handles.SphereHandleCap(0, point.vector3Value, Quaternion.identity, HandleUtility.GetHandleSize(point.vector3Value) / 5, EventType.Repaint);
                Handles.Label(point.vector3Value, new GUIContent("Patrol Point 0"));

                if (point.isExpanded)
                {
                    point.vector3Value = Handles.PositionHandle(point.vector3Value, Quaternion.identity);
                }

            }
        }
        if (_squadSpawnPoint.isExpanded)
        {
            Tools.hidden = true;
            Handles.color = Color.green;
            Handles.ConeHandleCap(0, _squadSpawnPoint.vector3Value + Vector3.up * HandleUtility.GetHandleSize(_squadSpawnPoint.vector3Value) / 2, Quaternion.LookRotation(Vector3.down), HandleUtility.GetHandleSize(_squadSpawnPoint.vector3Value), EventType.Repaint);
            Handles.Label(_squadSpawnPoint.vector3Value, new GUIContent("Squad Spawn Point"));
            _squadSpawnPoint.vector3Value = Handles.PositionHandle(_squadSpawnPoint.vector3Value, Quaternion.identity);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
