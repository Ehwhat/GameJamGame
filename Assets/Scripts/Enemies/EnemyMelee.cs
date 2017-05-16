using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnemyMessages;
public class EnemyMelee : EnemyBase, IHitTarget {

    private enum PatrolDirection
    {
        Forwards,
        Backwards
    }

    private float attackSpeed = 1f;
    private float lastAttacktime;

    public float speed = 15f;
    public float turnSpeed = 20f;
    public float seeingRadius = 15f;

    private UnitMovement movement = new UnitMovement();

    private Vector3[] patrolPath;
    private int currentPatrolPoint;
    private PatrolDirection patrolDirection;

    private PlayerManager lastKnownPlayer;
    private Vector3 lastKnownPlayerPosition;

    private Vector3 currentAttackPosition;

    public float health = 100;


    public override void OnSquadSpawn()
    {
        AddMessageListener<SquadNewPathMessage>(HandleNewPathMessage);
    }

    // Use this for initialization
    void Start () {
        _enemyState = EnemyState.Patrolling;
        movement.Initalise(transform);
    }
	
	// Update is called once per frame
	void Update () {
        if (_enemyState != EnemyState.Dead)
        {
            CheckForPlayer();
            HandleMovement();
        }
    }
    void FoundPlayers(List<PlayerManager> players)
    {
        List<PlayerManager> validTargets = new List<PlayerManager>();
        _enemyState = EnemyState.Attacking;
        bool newPlayerFound = true;
        foreach (PlayerManager player in players)
        {
            if (player == lastKnownPlayer && player._playerState != PlayerManager.PlayerState.Dead)
            {
                newPlayerFound = false;
                break;
            }
            else if (player._playerState != PlayerManager.PlayerState.Dead)
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

    void CheckForPlayer()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, seeingRadius);
        List<PlayerManager> players = new List<PlayerManager>();
        foreach (Collider c in col)
        {
            PlayerManager player = c.GetComponent<PlayerManager>();
            if (player != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
                {
                    players.Add(player);
                }
            }
        }
        if (players.Count < 1 && _enemyState == EnemyState.Attacking)
        {
            _enemyState = EnemyState.Alert;
        }
        else if (players.Count > 0)
        {
            FoundPlayers(players);
            MeleePlayer(lastKnownPlayer);
        }
    }

    void HandleNewPathMessage(SquadNewPathMessage message)
    {
        patrolPath = message._newPath;
        if (patrolPath.Length > 0)
        {
            int bestPoint = 0;
            float bestDistance = Vector3.Distance(patrolPath[0], transform.position);
            for (int i = 1; i < patrolPath.Length; i++)
            {
                float distance = Vector3.Distance(patrolPath[i], transform.position);
                if (distance < bestDistance)
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

    void GoToNextPatrolPoint()
    {
        if (patrolDirection == PatrolDirection.Forwards)
        {
            currentPatrolPoint++;
            if (currentPatrolPoint > patrolPath.Length - 1)
            {
                currentPatrolPoint = patrolPath.Length - 1;
                patrolDirection = PatrolDirection.Backwards;
            }
        }
        else
        {
            currentPatrolPoint--;
            if (currentPatrolPoint < 1)
            {
                currentPatrolPoint = 1;
                patrolDirection = PatrolDirection.Forwards;
            }
        }
    }

 

    void MeleePlayer(PlayerManager player)
    {
        if ( player != null && player._playerState != PlayerManager.PlayerState.Dead)
        {
            if(Time.time- lastAttacktime > attackSpeed)
            {
                lastAttacktime = Time.time;
                if (Vector3.Distance(transform.position, player.transform.position) < 0.1f);
                {
                     player.DamageFor(1);
                }
            }
        }
    }
    
    // Movement funtions
    void HandleAttackMovement()
    {
        if (currentAttackPosition == null || Vector3.Distance(transform.position, currentAttackPosition) < 0.5f)
        {
            currentAttackPosition = lastKnownPlayer.transform.position;
        }
        movement.MoveAlongDirection(lastKnownPlayer.transform.position - transform.position, speed * Time.deltaTime);

    }

    void HandlePatrolMovement()
    {

        if (patrolPath != null && patrolPath.Length > 0)
        {
            if (Vector3.Distance(transform.position, patrolPath[currentPatrolPoint]) < 0.5f)
            {
                GoToNextPatrolPoint();
            }
            Vector3 direction = patrolPath[currentPatrolPoint] - transform.position;
            movement.MoveAlongDirection(direction, speed * Time.deltaTime);
        }
    }
    void HandleAlertMovement()
    {
        Vector3 direction = lastKnownPlayerPosition - transform.position;
        movement.MoveAlongDirection(direction, speed * Time.deltaTime);
    }

    //checks which state the enmy is in, and moves appropriatley
    void HandleMovement()
    {
        if (_enemyState == EnemyState.Patrolling)
        {
            HandlePatrolMovement();
        }
        else if (_enemyState == EnemyState.Alert)
        {
            HandleAlertMovement();
        }
        else if (_enemyState == EnemyState.Attacking)
        {
            HandleAttackMovement();
        }
    }

    public void OnDamageHit(HitData hit)
    {
        health -= hit.damage;
        if(health <= 0)
        {
            _enemyState = EnemyState.Dead;
        }
    }
}
