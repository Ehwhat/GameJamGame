using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySquadManager : MonoBehaviour {

    enum PathPart
    {
        BeforePlayers,
        AfterPlayers
    }

    public EnemyBase[] enemies;

    private Vector3[] beforePath;
    private Vector3[] afterPath;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Spawn(float distance)
    {

    }

    Vector3[] GeneratePathThroughPlayers(float distance)
    {
        List<Vector3> pathSpots = GameManager.GetPathNodes();
        Vector3 playerCentre = GameManager.GetPlayersCentre();
        Vector3 beginPoint = GetRandomPathNodeWithinDistance(playerCentre, pathSpots, distance);
        Vector3 endPoint = GetRandomPathNodeWithinDistance(playerCentre, pathSpots, distance);

        PathRequestManager.RequestPath(new PathRequest(beginPoint, playerCentre, (vector, success) => {
            if (success) { BuildPath }
        }));

    }

    public void RequestPath()
    {
        
    }

    public void BuildPath(Vector3[] path, PathPart part)
    {
        if(part == PathPart.BeforePlayers)
        {

        }else
        {

        }
    }

    Vector3 GetRandomPathNodeWithinDistance(Vector3 originPoint, List<Vector3> pathSpots, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        List<Vector3> viablePathSpots = pathSpots.Where(o => { return CheckPointIsWithinDistance(originPoint, o, minDist, maxDist); }).ToList();
        return viablePathSpots[Random.Range(0, viablePathSpots.Count - 1)];
    }

    bool CheckPointIsWithinDistance(Vector3 originPoint, Vector3 point, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        float dist = Vector3.Distance(originPoint, point);
        return (dist > minDist && dist < maxDist);
    }

}
