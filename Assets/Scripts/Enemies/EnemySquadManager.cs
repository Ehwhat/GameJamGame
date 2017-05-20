using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyMessages;

namespace EnemyMessages
{
    public enum SquadMessageType
    {
        NewPath,
        Alert
    }

    public class SquadMessage
    {
        public SquadMessageType _messageType;
        public SquadMessage(SquadMessageType type)
        {
            _messageType = type;
        }
    }

    public class SquadNewPathMessage : SquadMessage
    {
        public Vector3[] _newPath;

        public SquadNewPathMessage(SquadMessageType type, Vector3[] newPath) : base(type)
        {
            _newPath = newPath;
        }
    }
    
}

public class EnemySquadManager : MonoBehaviour {

    enum PathPart
    {
        BeforePlayers,
        AfterPlayers,
        Both
    }

    private delegate void MessageRelay();
    private delegate void PathFound(Vector3[] before, Vector3[] after);
    private delegate void PathCompleted(Vector3[] path);

    public EnemyBase[] _members;
    public float _squadSpawnRadius = 5;
    public Vector3 _squadOrigin;
    public bool _spawnOnStart = false;

    private Queue<MessageRelay> _messageRelayQueue = new Queue<MessageRelay>();
   
    private Vector3[] _beforePath;
    private Vector3[] _afterPath;

    private PathPart _calculatedPathParts;
    private Vector3[] _calculatedPath;
    private int _pathingAttempts = 0;
    private int _aliveMembers;

	// Use this for initialization
	void Start () {
        if (_spawnOnStart)
        {
            Spawn(40);
        }
	}
	
	// Update is called once per frame
	void Update () {
        CheckRelayQueue();

    }

    void CheckRelayQueue()
    {
        while(_messageRelayQueue.Count > 0)
        {
            _messageRelayQueue.Dequeue().Invoke();
        }
    }
    
    public void Spawn(float distanceFromPlayers)
    {
        GeneratePathThroughPlayers(distanceFromPlayers); 
    }

    private void SpawnSquad()
    {
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i] != null)
            {
                _members[i] = SpawnMember(_members[i]);
            }
        }
    }

    private void SendMessageToSquad<T>(T message, EnemyBase sender)
    {
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i] != null && _members[i] != sender)
            {
                _members[i].OnRecieveSquadMessage<T>(message);
            }
        }
    }

    public void RelayMessage<T>(T message, EnemyBase sender)
    {
        _messageRelayQueue.Enqueue(new MessageRelay(() => { SendMessageToSquad<T>(message, sender); }));
    }

    private EnemyBase SpawnMember(EnemyBase prefab, int attempts = 0)
    {
        Vector3 randomPosition = GetRandomPositionInSquadRadius(_squadOrigin);
        EnemyBase member = Instantiate<EnemyBase>(prefab);
        member.transform.position = randomPosition;
        member.InitaliseEnemy(this);
        return member;
    }

    private Vector3 GetRandomPositionInSquadRadius(Vector3 squadCentre)
    {
        Vector2 randomCircle = Random.insideUnitCircle * _squadSpawnRadius;
        return squadCentre += new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public Vector3 GetSquadCentre()
    {
        Vector3 distProduct = _squadOrigin;
        if (_members.Length > 0)
        {
            distProduct = _members[0].transform.position;
            if (_members.Length > 1)
            {
                for (int i = 1; i < _members.Length; i++)
                {
                    distProduct += _members[i].transform.position;

                }
            }
            distProduct /= _members.Length;
        }
        return distProduct;
    }

    void OnPathComplete(Vector3[] path)
    {
        _squadOrigin = path[0];
        SpawnSquad();
        RelayMessage<SquadNewPathMessage>(new SquadNewPathMessage(SquadMessageType.NewPath, path), null);
    }

    public bool GeneratePathThroughPlayers(float distance)
    {
        List<Vector3> pathSpots = LevelGenerator.GetLevelPop().GetPathSpots();
        Vector3 playerCentre = GameManager.GetPlayersCentre();
        Vector3 beginPoint = GetRandomPathNodeWithinDistance(playerCentre, pathSpots, distance);
        Vector3 endPoint = GetRandomPathNodeWithinDistance(playerCentre, pathSpots, distance);
        bool pathFound = false;
        PathRequestManager.RequestPath(new PathRequest(beginPoint, playerCentre, (vector, success) =>
        {
            pathFound = success;
            if (success) { RegisterPath(vector, PathPart.BeforePlayers); }
        }));
        PathRequestManager.RequestPath(new PathRequest(playerCentre, endPoint, (vector, success) =>
        {
            pathFound = success;
            if (success) { RegisterPath(vector, PathPart.AfterPlayers); }
        }));
        return true;
    }

    void BuildPath(Vector3[] before, Vector3[] after)
    {
        Vector3[] path = new Vector3[before.Length + after.Length];
        int i = 0;
        foreach(Vector3 point in before)
        {
            path[i] = point;
            i++;
        }
        foreach (Vector3 point in after)
        {
            path[i] = point;
            i++;
        }
        _calculatedPath = path;
        OnPathComplete(path);
    }

    void RegisterPath(Vector3[] path, PathPart part)
    {
        if(part == PathPart.BeforePlayers)
        {
            _beforePath = path;
            if(_calculatedPathParts == PathPart.AfterPlayers)
            {
                _calculatedPathParts = PathPart.Both;
                BuildPath(_beforePath, _afterPath);
            }else
            {
                _calculatedPathParts = PathPart.BeforePlayers;
            }
        }else
        {
            _afterPath = path;
            if (_calculatedPathParts == PathPart.BeforePlayers)
            {
                _calculatedPathParts = PathPart.Both;
                BuildPath(_beforePath, _afterPath);
            }
            else
            {
                _calculatedPathParts = PathPart.AfterPlayers;
            }
        }
    }

    Vector3 GetRandomPathNodeWithinDistance(Vector3 originPoint, List<Vector3> pathSpots, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        if(pathSpots.Count < 1)
        {
            Debug.LogError("No path spots!");
        }
        List<Vector3> viablePathSpots = pathSpots.Where(o => { return CheckPointIsWithinDistance(originPoint, o, minDist, maxDist); }).ToList();
        if (viablePathSpots.Count > 1)
        {
            return viablePathSpots[Random.Range(0, viablePathSpots.Count - 1)];
        }
        Debug.Log("None Found");
        return Vector3.zero;
    }

    bool CheckPointIsWithinDistance(Vector3 originPoint, Vector3 point, float minDist = 0, float maxDist = float.PositiveInfinity)
    {
        float dist = Vector3.Distance(originPoint, point);
        return (dist > minDist && dist < maxDist);
    }

}
