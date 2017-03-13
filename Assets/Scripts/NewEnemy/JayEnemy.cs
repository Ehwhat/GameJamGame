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
        eEngaging
    }

    //Speeds for movement.
    public float chaseSpeed = 4;
    public float patrolSpeed = 2;

    //Vision Information
    float visionRange = 10;
    bool sweepRight = true;
    Transform visionPoint;

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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        visionPoint = gameObject.transform.FindChild("Eyes").transform;//Attatch the eyes
    }

    void Update()
    {
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
                if ( currentPoint + 1 < patrolPoints.Length)
                {
                    //Switch to the next point
                    currentPoint++;
                }
                else
                {
                    //Back to first point
                    currentPoint = 0;
                }
                currentWait = 0;
            }
        }
        else if (currentState == EnemyState.eChasing)
        {
            //Run after the players, shoot them etc. Still need to use Pathfinding
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
            Vector3 velocity = (patrolPoints[currentPoint].location.position - rb.position).normalized;
            velocity.y = 0;//To stop upwards movement;
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime * patrolSpeed);
        }
        else if (currentState == EnemyState.eChasing)
        {
            //Run after the players, shoot them etc. Still need to use Pathfinding
        }

    }

    void Look()
    {
        //Rotate the visionPoint
        if (sweepRight)
        {
            visionPoint.localEulerAngles = new Vector3(visionPoint.localEulerAngles.x, Mathf.PingPong(Time.time, 46), visionPoint.localEulerAngles.z);
            if (visionPoint.localEulerAngles.y >= 45)
            {
                sweepRight = !sweepRight;//Switch to sweeping the other way
            }
        }
        else
        {
            visionPoint.localEulerAngles = new Vector3(visionPoint.localEulerAngles.x, Mathf.PingPong(Time.time, -46), visionPoint.localEulerAngles.z);
            if (visionPoint.localEulerAngles.y <= -45)
            {
                sweepRight = !sweepRight;//Switch to sweeping the other way
            }
        }
       

        RaycastHit hit;
        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(visionPoint.forward);
        Debug.DrawLine(visionPoint.position, localForward * 10, Color.red);
        if (Physics.Raycast(visionPoint.position, localForward, out hit, 100))
        {
            
           
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player Seen");
            }

        }
    }
}
