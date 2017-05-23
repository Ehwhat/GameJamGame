using UnityEngine;
using System.Collections;

public class SquadSpawner : MonoBehaviour {

    public NavMeshEnemySquadManager[] _squadPrefabs;
    public bool _active = true;
    public float _spawnDelay = 20;
    public int _squadAmount = 3;
    public float _spawnMinDist = 20;
    public float _spawnMaxDist = 40;

    void Start()
    {
        StartCoroutine(SpawnSquadsPeriodically());
    }

    IEnumerator SpawnSquadsPeriodically()
    {
        while (true)
        {
            for (int i = 0; i < _squadAmount; i++)
            {
                if (_active)
                {
                    SpawnSquad(Random.Range(_spawnMinDist, _spawnMaxDist));
                }
            }
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    public NavMeshEnemySquadManager SpawnSquad(float distance)
    {
        NavMeshEnemySquadManager squad = Instantiate(GetRandomSquad());
        squad.Spawn();
        return squad;
    }

    public NavMeshEnemySquadManager SpawnSquad(float distance, NavMeshEnemySquadManager squad)
    {
        squad = Instantiate(squad);
        squad.Spawn();
        return squad;
    }

    public NavMeshEnemySquadManager SpawnSquad(Vector3 goToPoint, NavMeshEnemySquadManager squad)
    {
        squad = Instantiate(squad);
        squad.Spawn(goToPoint);
        return squad;
    }

    public NavMeshEnemySquadManager SpawnSquad(Vector3 goToPoint)
    {
        NavMeshEnemySquadManager squad = Instantiate(GetRandomSquad());
        squad.Spawn(goToPoint);
        return squad;
    }


    private NavMeshEnemySquadManager GetRandomSquad()
    {
        return _squadPrefabs[Random.Range(0, _squadPrefabs.Length)];
    }



}
