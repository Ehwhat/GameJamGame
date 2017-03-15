using UnityEngine;
using System.Collections;
/// <summary>
/// This is a simple implementation of an Enemy. With patroling, and chasing.
/// I will move each state into its own function later, so its easier to read.
/// </summary>
public class JayEnemy : MonoBehaviour {

    enum EnemyState
    {
        eStationary,
        ePatroling,
        eChasing,
        eEngaging,
        ePathTest
    }

    //Speeds for movement.
    public float chaseSpeed = 4;
    public float patrolSpeed = 2;

    //Vision Information
    float visionRange = 10;
    Transform visionPoint;
    Transform[] playerTransforms;

    //The enemys rigidbody
    private Rigidbody rb;

    //The location of where the Enemy is moving to
    public Transform target;

    int currentPoint = 0;
    float currentWait = 0;
    //An array of all the Enemys patrol points
    public JayPatrolPoint[] patrolPoints;

    //The current state of the enemy
    EnemyState currentState = EnemyState.ePatroling;


    //Pathfinding
    bool tempCalc = false;
    Vector3[] chasePath;
    PathRequest pathRequest;
    //PathRequestManager pathManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        visionPoint = gameObject.transform.FindChild("Eyes").transform;//Attatch the eyes

        //Find players and store there Transforms into the transform array
        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
        playerTransforms = new Transform[g.Length];
        for (int x = 0; x < g.Length; ++x)
        {
            playerTransforms[x] = g[x].transform;
        }

       // pathManager = GameObject.Find("LevelManager").GetComponent<PathRequestManager>();
    }

    void Update()
    {
        //Path Tests
        if (Input.GetKeyDown(KeyCode.P))
        {
            GeneratePath(playerTransforms[0].position);
        }

        //End tests


        if (currentState == EnemyState.eStationary)
        {
            //Do animations when staying still perhaps?
        }
        else if (currentState == EnemyState.ePatroling)
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
                Vector3 rot = (patrolPoints[currentPoint].location.position - rb.position).normalized;

                rot.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(rot);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));

                currentWait = 0;
            }
        }
        else if (currentState == EnemyState.eChasing)
        {
            //Run after the players, shoot them etc. Still need to use Pathfinding
        }
        else if (currentState == EnemyState.ePathTest && !tempCalc)
        {
            currentPoint = 0;
            GeneratePath(playerTransforms[0].position);
        }
        else if (currentState == EnemyState.ePatroling)
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
                Vector3 rot = (chasePath[currentPoint] - rb.position).normalized;

                rot.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(rot);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
        }
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.eStationary)
        {
            //Do animations when staying still perhaps?
        }
        else if (currentState == EnemyState.ePatroling)
        {
            Vector3 distance = (rb.position - patrolPoints[currentPoint].location.position);
            //To stop upwards movement;

            Vector3 direction = distance.normalized;

            //Debug.Log(Mathf.Abs(distance.x));

            /*if (Mathf.Abs(distance.x) < 1)
            {
                direction.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
            }*/
            direction.y = 0;
            rb.MovePosition(rb.position - direction * Time.fixedDeltaTime * patrolSpeed);
        }
        else if (currentState == EnemyState.eChasing)
        {
            //Run after the players, shoot them etc. Still need to use Pathfinding
        }
        else if (currentState == EnemyState.ePathTest)
        {
            Vector3 distance = (rb.position - chasePath[currentPoint]);
            //To stop upwards movement;

            Vector3 direction = distance.normalized;

            direction.y = 0;
            rb.MovePosition(rb.position - direction * Time.fixedDeltaTime * patrolSpeed);
        }

    }

    void Look()
    {
        
        RaycastHit hit;


        //For each players transform, check if they are in range, and within the vision cone.
        for (int playerIndex = 0; playerIndex < playerTransforms.Length; playerIndex++)
        {
            Vector3 playerRaycast = (playerTransforms[playerIndex].position - visionPoint.position).normalized;
            float visionAngle = Vector3.Dot(visionPoint.forward, playerRaycast);
            //Debug.Log(visionAngle);
        }

    }


    void GeneratePath(Vector3 _target)
    {
        pathRequest.pathStart = rb.position;
        pathRequest.pathEnd = _target;
        System.Action<Vector3[], bool> _callback = new System.Action<Vector3[], bool>(pathCallback);
        pathRequest.callback = _callback;

        PathRequestManager.RequestPath(pathRequest);
    }

    void pathCallback(Vector3[] v, bool b)
    {
        chasePath = v;
        currentState = EnemyState.ePathTest;
    }

    //For in the future when we have to pass the points in from the terrain generator. 
    public void SetPatrolPoints(JayPatrolPoint[] _points, int _currentPoint = 0)
    {
        currentPoint = _currentPoint;
        patrolPoints = _points;
    }
}
