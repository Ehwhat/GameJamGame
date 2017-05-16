using UnityEngine;
using System.Collections.Generic;
using EnemyMessages;
using System;
using System.Collections;
/// <summary>
/// This is a simple implementation of an Enemy. With patroling, and chasing.
/// I will move each state into its own function later, so its easier to read.
/// 
/// We can stop the enemy from processing when they are far away from the player
/// </summary>
public class JayEnemy : EnemyBase, IHitTarget
{

    enum JayEnemyState
    {
        eStationary,
        ePatroling,
        eChasing,
        eEngaging,
        ePathTest
    }

    public struct JayTestStruct
    {
        public JayEnemy _this;
    }

   

    float health = 150.0f;

    public Renderer rendMesh;

    //Speeds for movement.
    public float chaseSpeed = 4;
    public float patrolSpeed = 2;

    //Movement Vectors
    Vector3 distance;

    //Vision Information
    float visionRange = 10;
    Transform visionPoint;
    public Transform[] playerTransforms;
    int chaseTarget = 0;

    //The enemys rigidbody
    private Rigidbody rb;
    private Vector3 rb_pos;

    //The location of where the Enemy is moving to
    //public Transform target;

    public int currentPoint = 0;
    float currentWait = 0;
    //An array of all the Enemys patrol points
    public JayPatrolPoint[] patrolPoints;

    //The current state of the enemy
    JayEnemyState currentState = JayEnemyState.ePatroling;


    //Pathfinding
    bool tempCalc = false;
    public Vector3[] chasePath;
    PathRequest pathRequest;

    //Fixing Speed tests
    float pathFindWaitTime = 0.0f;
    WaitForSeconds wait = new WaitForSeconds(0.1f);
    WaitForFixedUpdate f_wait = new WaitForFixedUpdate();

    public WeaponManager weaponManager;

    bool dead = false;
    //PathRequestManager pathManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb_pos = rb.position;
       // visionPoint = gameObject.transform.FindChild("Eyes").transform;//Attatch the eyes

        //Find players and store there Transforms into the transform array
        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
        playerTransforms = new Transform[g.Length];
        for (int x = 0; x < g.Length; ++x)
        {
            playerTransforms[x] = g[x].transform;
        }

        JayTestStruct test;
        test._this = this;
        _enemyState = EnemyState.Patrolling;
        AddMessageListener<JayTestStruct>(HandleTestMessage);
        SendMessageToSquad<JayTestStruct>(test);
    }

    void Update()
    {
        if (health > 0)
        {
            rb_pos = rb.position;//Might have to move this to fixed


            if (currentState == JayEnemyState.eStationary)
            {
                //Do animations when staying still perhaps?
            }
            else if (currentState == JayEnemyState.ePatroling && patrolPoints.Length != 0)
            {
                Look();//Do vision checks

                //Keep moving between patrol points
                if (currentWait <= patrolPoints[currentPoint].waitTime)
                {
                    currentWait += Time.deltaTime;
                }
                else
                {
                    if (currentPoint + 1 < patrolPoints.Length)
                    {
                        //Switch to the next point
                        currentPoint++;
                    }
                    else
                    {
                        //Back to first point
                        currentPoint = 0;
                    }

                    //Temp Fix
                    Vector3 rot = (patrolPoints[currentPoint].location.position - rb_pos).normalized;

                    rot.y = rb.position.y;
                    Quaternion lookRotation = Quaternion.LookRotation(rot);
                    //rb.rotation = Quaternion()
                    rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));

                    currentWait = 0;
                }
            }
            else if (currentState == JayEnemyState.ePatroling && patrolPoints.Length != 0)
            {

                if (currentPoint + 1 < chasePath.Length)
                {
                    //Switch to the next point
                    currentPoint++;
                }
                else
                {
                    //Back to first point
                    currentPoint = 0;
                }

                //Temp Fix
                Vector3 rot = (chasePath[currentPoint] - rb_pos).normalized;

                rot.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(rot);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
            }
            else if (currentState == JayEnemyState.eChasing && !tempCalc)
            {
                currentPoint = 0;
                GeneratePath(playerTransforms[chaseTarget].position);
                tempCalc = true;
            }
            else if (currentState == JayEnemyState.eChasing && chasePath != null)
            {
                //Check if we are on the current chase Point, then increment.
                if (currentPoint + 1 < chasePath.Length && (rb.position - chasePath[currentPoint]).magnitude <= 1)//1 should be the engagement distance
                {

                    currentPoint++;
                }


                //Temp Fix
                Vector3 rot = (chasePath[currentPoint] - rb.position).normalized;
                rot.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(rot);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
            }
        }
    }

    void FixedUpdate()
    {
        if (health > 0.0f)
        {
            if (currentState == JayEnemyState.eStationary)
            {
                //Do animations when staying still perhaps?
            }
            else if (currentState == JayEnemyState.ePatroling && patrolPoints.Length != 0)
            {
                distance = (rb.position - patrolPoints[currentPoint].location.position).normalized;

                distance.y = 0;
                rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * patrolSpeed);
            }
            else if (currentState == JayEnemyState.eChasing && chasePath != null)
            {
                pathFindWaitTime += Time.fixedDeltaTime;
                //Run after the players, shoot them etc. Still need to use Pathfinding
                distance = (rb.position - chasePath[currentPoint]).normalized;
                distance.y = 0;
                rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * chaseSpeed);

                //Path Update
                if (pathFindWaitTime >= 0.1f)
                {
                    pathFindWaitTime = 0.0f;
                    GeneratePath(playerTransforms[chaseTarget].position);
                }
            }
            else if (currentState == JayEnemyState.ePathTest)
            {
                distance = (rb.position - chasePath[currentPoint]).normalized;
                distance.y = 0;
                rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * patrolSpeed);
            }

            //Update player transforms
            if (playerTransforms.Length == 0)
            {
                GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
                playerTransforms = new Transform[g.Length];
                for (int x = 0; x < g.Length; ++x)
                {
                    playerTransforms[x] = g[x].transform;
                }
            }
        }

    }

    void Look()
    {

        //For each players transform, check if they are in range, and within the vision cone.
        for (int playerIndex = 0; playerIndex < playerTransforms.Length; playerIndex++)
        {
            Vector3 playerRaycast = (playerTransforms[playerIndex].position - visionPoint.position).normalized;
            float playerDistance = Vector3.Distance(playerTransforms[playerIndex].position, visionPoint.position);
            float visionAngle = Vector3.Dot(visionPoint.forward, playerRaycast);

            //TODO : Need to do a raycast to see if we are seeing the player through a wall as well.
            if (visionAngle > 0.2f && playerDistance <= 15.0f)
            {
                rendMesh.material.color = Color.red;
                chaseTarget = playerIndex;
                currentState = JayEnemyState.eChasing;
            }
            //Debug.Log(visionAngle);
        }

    }


    void GeneratePath(Vector3 _target)
    {

        pathRequest.pathStart = rb.position;
        pathRequest.pathEnd = _target;
        System.Action<Vector3[], bool> _callback = new System.Action<Vector3[], bool>(PathCallback);
        pathRequest.callback = _callback;

        PathRequestManager.RequestPath(pathRequest);
    }

    void PathCallback(Vector3[] v, bool _success)
    {

        if (_success)
        {
            chasePath = v;
            currentPoint = 0;
         
        }
        else
        {
            //Failed, so we have to do something to help the enemy get a path, for now  we can just remove the old path
            //chasePath = null;
            //StopCoroutine(UpdatePath());
        }
    }

    void ClearPath()
    {
        chasePath = null;
        chaseTarget = 0;
    }

    //For in the future when we have to pass the points in from the terrain generator. 
    public void SetPatrolPoints(JayPatrolPoint[] _points, int _currentPoint = 0)
    {
        currentPoint = _currentPoint;
        patrolPoints = _points;
    }

    public override void OnSquadSpawn()
    {
        Debug.Log("Squad Spawned");
    }

    void HandleTestMessage(JayTestStruct message)
    {
        Debug.Log("I recieved " + message._this.name);
    }

    void HandleNewPathMessage(SquadNewPathMessage message)
    {
        Debug.Log("Path message");
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

    public void OnDamageHit(HitData hit)
    {
        //Change state to attacking, if wasnt.

        //Take Damage
        health -= hit.damage;
        if (health <= 0.0f && !dead)
        {
            dead = true;
            Die(hit);
        }
    }

    void Die(HitData lastHit)
    {
            Vector3 hitPoint = lastHit.rayHit.point;
            Vector3 forceHit = (rb.position - lastHit.rayHit.point).normalized * -100;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForceAtPosition(forceHit, hitPoint);
            StartCoroutine(DisableRotation());
    }

    IEnumerator DisableRotation()
    {
        yield return new WaitForSeconds(1.0f);
        rb.angularDrag = 2.0f;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
    }
}