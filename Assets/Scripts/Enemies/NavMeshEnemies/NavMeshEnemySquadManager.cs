using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshEnemySquadManager : MonoBehaviour {

    public enum PatrolType
    {
        PatrolThroughPlayer,
        PatrolThroughObjectives
    }

    public NavMeshEnemyBase[] _enemies;
    private Vector3[] _patrolPath;

    void Start()
    {
        _patrolPath = GeneratePatrolPath();
        transform.position = _patrolPath[1];
        InitaliseEnemies();
    }

    void InitaliseEnemies()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            NavMeshEnemyBase enemy = _enemies[i];
            if(enemy.hideFlags == HideFlags.HideInHierarchy)
            {
                enemy = Instantiate(enemy);
                enemy.transform.position = transform.position;
                enemy.transform.parent = transform;
            }
            enemy.SetPatrolPath(_patrolPath);

        }
    }

    public Vector3[] GeneratePatrolPath()
    {
        Vector3[] path = new Vector3[4];
        List<Vector3> objectivePositions = new List<Vector3>(ObjectiveManager.getObjectivePositions());
        objectivePositions.Add(GameManager.GetPlayersCentre());
        path[0] = objectivePositions[Random.Range(0, objectivePositions.Count - 1)];

        List<Vector3> pathSpots = LevelGenerator.GetLevelPop().GetPathSpots();
        Vector3[] selectedPathSpots = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            path[i + 1] = pathSpots[Random.Range(0, pathSpots.Count - 1)];//GetRandomPathNodeWithinDistance(path[i], pathSpots, 40);
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
