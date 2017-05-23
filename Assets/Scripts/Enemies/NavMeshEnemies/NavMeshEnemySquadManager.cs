using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshEnemySquadManager : MonoBehaviour {

    public enum PatrolType
    {
        AutoGeneratePath,
        usePrefabricatedPath,
        WaitAtSpawn
    }

    public enum SpawnPosition
    {
        TransformPosition,
        PrefabricatedPosition,
        FirstPathPosition,
        RandomPathPosition,
        RandomPosition
    }

    public bool _spawnOnStart = false;
    public static string PROPERTY_SPAWN_ON_START = "_spawnOnStart";

    public NavMeshEnemyBase[] _enemies;
    public static string PROPERTY_ENEMIES = "_enemies";

    [SerializeField]
    private List<Vector3> _patrolPath;
    public static string PROPERTY_PATROL_PATH = "_patrolPath";

    [SerializeField]
    private PatrolType _squadPatrolType;
    public static string PROPERTY_SQUAD_PATROL_TYPE = "_squadPatrolType";

    [SerializeField]
    private SpawnPosition _squadSpawnPosition;
    public static string PROPERTY_SQUAD_SPAWN_TYPE = "_squadSpawnPosition";

    [SerializeField]
    private Vector3 _squadSpawnPoint;
    public static string PROPERTY_SQUAD_SPAWN_POINT = "_squadSpawnPoint";

    void Start()
    {
        //transform.rotation = transform.parent.rotation;
        if (_spawnOnStart)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        SetPatrolPath();
        SetPosition();
        InitaliseEnemies();
    }

    public void Spawn(params Vector3[] points)
    {
        SetPatrolPath(points);
        SetPosition();
        InitaliseEnemies();
    }

    void InitaliseEnemies()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            NavMeshEnemyBase enemy = _enemies[i];
            if (enemy.hideFlags == HideFlags.HideInHierarchy) // Checks if the enemy is in the scene, or a prefab (Cause prefabs)
            {
                enemy = Instantiate(enemy);
                enemy.transform.parent = transform;
                enemy.SetAgentPosition(transform.position);
            }
            enemy.SetPatrolPath(_patrolPath);

        }
    }

    private void SetPatrolPath(){
        if (_squadPatrolType == PatrolType.AutoGeneratePath)
        {
            _patrolPath = GeneratePatrolPath();
        }
        else if (_squadPatrolType == PatrolType.WaitAtSpawn)
        {
            _patrolPath.Clear();
        }else
        {
            _patrolPath = LocalPatrolPath(_patrolPath);
        }
    }

    private void SetPatrolPath(params Vector3[] points)
    {
        _patrolPath.Clear();
        _patrolPath.AddRange(points);
    }

    private void SetPosition()
    {
        if (_squadSpawnPosition == SpawnPosition.FirstPathPosition)
        {
            if (_patrolPath.Count > 0)
            {
                transform.position = _patrolPath[0];
            }
        }
        else if (_squadSpawnPosition == SpawnPosition.PrefabricatedPosition)
        {
            transform.localPosition = _squadSpawnPoint;
        }
        else if (_squadSpawnPosition == SpawnPosition.RandomPathPosition)
        {
            if (_patrolPath.Count > 0)
            {
                transform.position = _patrolPath[Random.Range(0, _patrolPath.Count)];
            }
        }else if(_squadSpawnPosition == SpawnPosition.RandomPosition)
        {
            List<Vector3> pathSpots = LevelGenerator.GetLevelPop().GetPathSpots();
            transform.position = pathSpots[Random.Range(0, pathSpots.Count - 1)];
        }
    }

    private List<Vector3> LocalPatrolPath(List<Vector3> path)
    {
        List<Vector3> localPath = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y+180, 0)), Vector3.one);
            localPath.Add(localToWorldMatrix.MultiplyPoint3x4(path[i])); //Why does unity not have a TransformPoint that's not using scale? Who knows, but this works 
        }
        return localPath;
    }

    public List<Vector3> GeneratePatrolPath()
    {
        List<Vector3> path = new List<Vector3>();
        List<Vector3> objectivePositions = new List<Vector3>(ObjectiveManager.getObjectivePositions());
        objectivePositions.Add(GameManager.GetPlayersCentre());
        path.Add(objectivePositions[Random.Range(0, objectivePositions.Count - 1)]);

        List<Vector3> pathSpots = LevelGenerator.GetLevelPop().GetPathSpots();
        Vector3[] selectedPathSpots = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            path.Add(pathSpots[Random.Range(0, pathSpots.Count - 1)]);//GetRandomPathNodeWithinDistance(path[i], pathSpots, 40);
        }
        return path;
    }

    Vector3 GetRandomPathNodeWithinDistance(Vector3 originPoint, List<Vector3> pathSpots, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        if (pathSpots.Count < 1)
        {
            Debug.LogError("No path spots!");
        }
        List<Vector3> viablePathSpots = pathSpots.Where(o => { return CheckPointIsWithinDistance(originPoint, o, minDist, maxDist); }).ToList();
        if (viablePathSpots.Count > 1)
        {
            return viablePathSpots[Random.Range(0, viablePathSpots.Count - 1)];
        }
        return pathSpots[Random.Range(0, pathSpots.Count - 1)];
    }

    bool CheckPointIsWithinDistance(Vector3 originPoint, Vector3 point, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        float dist = Vector3.Distance(originPoint, point);
        return (dist > minDist && dist < maxDist);
    }

}
