using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnemyMessages;


public class EnemyTest : EnemyBase {

    public struct TestStruct
    {
        public EnemyTest enemyTest;
        
    }

    private enum SightState
    {
        PlayerInSight,
        PlayerOutOfSight
    }

    private enum PatrolDirection
    {
        Forwards,
        Backwards
    }

    public float speed = 15f;
    public float turnSpeed = 20f;
    public float seeingRadius = 15f;
    public LayerMask playerMask;
    public WeaponManager weaponManager;

    private UnitMovement movement = new UnitMovement();
    private Vector3[] patrolPath;
    private int currentPatrolPoint;
    private PatrolDirection patrolDirection;

    private Vector3 currentAttackPosition;
    private Vector3 currentDirection;

    private PlayerManager lastKnownPlayer;
    private Vector3 lastKnownPlayerPosition;
    private SightState sightState;

    public override void OnSquadSpawn()
    {
        AddMessageListener<SquadNewPathMessage>(HandleNewPathMessage);
        movement.Initalise(transform);
    }

    // Use this for initialization
    void Start () {
        TestStruct test;
        test.enemyTest = this;
        _enemyState = EnemyState.Patrolling;
        AddMessageListener<TestStruct>(HandleTestMessage);
        SendMessageToSquad<TestStruct>(test);
	}

    void Update()
    {
        CheckForPlayer();
        HandleMovement();
    }

    void CheckForPlayer()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, seeingRadius, playerMask);
        List<PlayerManager> players = new List<PlayerManager>();
        foreach(Collider c in col)
        {
            PlayerManager player = c.GetComponent<PlayerManager>();
            if(player != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
                {
                    players.Add(player);
                }
            }
        }
        if(players.Count < 1 && _enemyState == EnemyState.Attacking)
        {
            _enemyState = EnemyState.Alert;
        }else if(players.Count > 0)
        {
            FoundPlayers(players);
            ShootPlayer(lastKnownPlayer);
        }
    }

    void FoundPlayers(List<PlayerManager> players)
    {
        List<PlayerManager> validTargets = new List<PlayerManager>();
        _enemyState = EnemyState.Attacking;
        bool newPlayerFound = true;
        foreach(PlayerManager player in players)
        {
            if (player == lastKnownPlayer && player._playerState != PlayerManager.PlayerState.Dead)
            {
                newPlayerFound = false;
                break;
            }else if(player._playerState != PlayerManager.PlayerState.Dead)
            {
                validTargets.Add(player);
            }
        }
        if (newPlayerFound && validTargets.Count > 0)
        {
            lastKnownPlayer = validTargets[Random.Range(0, validTargets.Count)];
        }
        else if (lastKnownPlayer == null || lastKnownPlayer._playerState == PlayerManager.PlayerState.Dead)
        {
            _enemyState = EnemyState.Patrolling;
        }
        else
        {
            lastKnownPlayerPosition = lastKnownPlayer.transform.position;
        }
    }

    void ShootPlayer(PlayerManager player)
    {
        if (player._playerState != PlayerManager.PlayerState.Dead)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            Debug.Log(targetDirection);
            float currentAngle = Vector3.Angle(Vector3.forward, targetDirection);
            weaponManager.FireWeapon(currentAngle);
        }
    }

    void HandleMovement()
    {
        if(_enemyState == EnemyState.Patrolling)
        {
            HandlePatrolMovement();
        }
        else if(_enemyState == EnemyState.Alert)
        {
            HandleAlertMovement();
        }else if(_enemyState == EnemyState.Attacking)
        {
            HandleAttackMovement();
        }
    }

    void HandleAttackMovement()
    {
        if (currentAttackPosition == null || Vector3.Distance(transform.position, currentAttackPosition) < 0.5f)
        {
            currentAttackPosition = GetRandomPointAroundPlayers(2, 5);
        }
        movement.MoveAlongDirection(currentAttackPosition, Mathf.Sin(Time.time * 0.1f) * 5 * Time.deltaTime);

    }

    void HandleAlertMovement()
    {
        Vector3 direction = lastKnownPlayerPosition - transform.position;
        movement.MoveAlongDirection(direction, speed*Time.deltaTime);
    }


    void HandlePatrolMovement()
    {
        if(patrolPath != null && patrolPath.Length > 0)
        {
            if(Vector3.Distance(transform.position, patrolPath[currentPatrolPoint]) < 0.5f)
            {
                GoToNextPatrolPoint();
            }
            Vector3 direction = patrolPath[currentPatrolPoint] - transform.position;
            movement.MoveAlongDirection(direction, speed*Time.deltaTime);
        }
    }

    Vector3 GetRandomPointAroundPlayers(float minDistanceToPlayers, float maxDistanceToPlayers)
    {
        Vector3 circle = Random.insideUnitCircle * Random.Range(minDistanceToPlayers, maxDistanceToPlayers);
        return GameManager.GetPlayersCentre() + new Vector3(circle.x, circle.y, circle.z);
    }

    void GoToNextPatrolPoint()
    {
        if(patrolDirection == PatrolDirection.Forwards)
        {
            currentPatrolPoint++;
            if (currentPatrolPoint > patrolPath.Length-1)
            {
                currentPatrolPoint = patrolPath.Length - 1;
                patrolDirection = PatrolDirection.Backwards;
            }
        }else
        {
            currentPatrolPoint--;
            if (currentPatrolPoint < 1)
            {
                currentPatrolPoint = 1;
                patrolDirection = PatrolDirection.Forwards;
            }
        }
    }

    void HandleTestMessage(TestStruct message)
    {
        Debug.Log("I recieved " + message.enemyTest.name);
    }

    void HandleNewPathMessage(SquadNewPathMessage message)
    {
        patrolPath = message._newPath;
        if (patrolPath.Length > 0)
        {
            int bestPoint = 0;
            float bestDistance = Vector3.Distance(patrolPath[0],transform.position);
            for (int i = 1; i < patrolPath.Length; i++)
            {
                float distance = Vector3.Distance(patrolPath[i], transform.position);
                if(distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = i;
                }
            }
            currentPatrolPoint = bestPoint;
            patrolDirection = PatrolDirection.Forwards;
        }
        Vector3 difference = (patrolPath[0] - transform.position);
        for (int i = 1; i < patrolPath.Length; i++)
        {
            patrolPath[0] += difference;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (patrolPath != null)
        {
            foreach (Vector3 point in patrolPath)
            {
                Gizmos.DrawSphere(point, 1);
            }
        }
    }
}
