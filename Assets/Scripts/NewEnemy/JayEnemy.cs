using UnityEngine;
using System.Collections;
/// <summary>
/// This is a simple implementation of an Enemy. With patroling, and chasing.
/// I will move each state into its own function later, so its easier to read.
/// 
/// We can stop the enemy from processing when they are far away from the player
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
    EnemyState currentState = EnemyState.ePatroling;


    //Pathfinding
    bool tempCalc = false;
    Vector3[] chasePath;
    PathRequest pathRequest;

    //Temp node display
    public GameObject nodePrefab;
    GameObject[] nodes;

    //Fixing Speed tests
    WaitForSeconds wait = new WaitForSeconds(0.1f);
    WaitForFixedUpdate f_wait = new WaitForFixedUpdate();
    //PathRequestManager pathManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb_pos = rb.position;
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
        

        rb_pos = rb.position;//Might have to move this to fixed


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
                Vector3 rot = (patrolPoints[currentPoint].location.position - rb_pos).normalized;

                rot.y = rb.position.y;
                Quaternion lookRotation = Quaternion.LookRotation(rot);
                rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));

                currentWait = 0;
            }
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
            Vector3 rot = (chasePath[currentPoint] - rb_pos).normalized;

            rot.y = rb.position.y;
            Quaternion lookRotation = Quaternion.LookRotation(rot);
            rb.MoveRotation(Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
        }
        else if (currentState == EnemyState.eChasing && !tempCalc)
        {
            currentPoint = 0;
            GeneratePath(playerTransforms[chaseTarget].position);
            tempCalc = true;
            StartCoroutine(UpdatePath());
        }
        else if (currentState == EnemyState.eChasing && chasePath != null)
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

    void FixedUpdate()
    {
        if (currentState == EnemyState.eStationary)
        {
            //Do animations when staying still perhaps?
        }
        else if (currentState == EnemyState.ePatroling)
        {
            distance = (rb.position - patrolPoints[currentPoint].location.position).normalized;

            distance.y = 0;
            rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * patrolSpeed);
        }
        else if (currentState == EnemyState.eChasing && chasePath != null)
        {
            //Run after the players, shoot them etc. Still need to use Pathfinding
            distance = (rb.position - chasePath[currentPoint]).normalized;
            distance.y = 0;
            rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * chaseSpeed);

        }
        else if (currentState == EnemyState.ePathTest)
        {
            distance = (rb.position - chasePath[currentPoint]).normalized;
            distance.y = 0;
            rb.MovePosition(rb.position - distance * Time.fixedDeltaTime * patrolSpeed);
        }

    }

    void Look()
    {
       
        //For each players transform, check if they are in range, and within the vision cone.
        for (int playerIndex = 0; playerIndex < playerTransforms.Length; playerIndex++)
        {
            Vector3 playerRaycast = (playerTransforms[playerIndex].position - visionPoint.position).normalized;
            float visionAngle = Vector3.Dot(visionPoint.forward, playerRaycast);

            //TODO : Need to do a raycast to see if we are seeing the player through a wall as well.
            if (visionAngle > 0.2f)
            {
                rendMesh.material.color = Color.red;
                chaseTarget = playerIndex;
                currentState = EnemyState.eChasing;
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
        currentPoint = 0;
        if (_success)
        {
            chasePath = v;

            //currentState = EnemyState.ePathTest;

           /* nodes = new GameObject[v.Length];
            //Generate display
            for (int x = 0; x < v.Length; x++)
            {
                nodes[x] = Instantiate(nodePrefab, v[x], Quaternion.identity) as GameObject;
            }*/
        }
        else
        {
            //Failed, so we have to do something to help the enemy get a path, for now  we can just remove the old path
            chasePath = null;
            StopCoroutine(UpdatePath());
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

    IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return wait;
            yield return f_wait;

           /* for (int x = 0; x < nodes.Length; x++)
            {
                Destroy(nodes[x]);
            }*/

           // Debug.Log("Update Path");
            GeneratePath(playerTransforms[chaseTarget].position);
        }
    }
}
