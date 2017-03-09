using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour
{
    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 20f;
    public float attackRange = 10f;

    //where the enemy will path to in patrol state, can be changed later as enemy doesnt actually move
    public Transform[] wayPoints;

    //position to raycast from to find player
    public Transform eyes;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    //state indicator above enemy
    public MeshRenderer meshrendererFlag;

    [HideInInspector]
    public Transform chaseTarget;
    [HideInInspector]
    public IEnemyState currentState;
    [HideInInspector]
    public ChaseState chaseState;
    [HideInInspector]
    public AttackState attackState;
    [HideInInspector]
    public AlertState alertState;
    [HideInInspector]
    public PatrolState patrolState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        //creates an instance of each state
        chaseState = new ChaseState (this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);
        
        navMeshAgent = GetComponent<NavMeshAgent>();
    }


    void Start()
    {
        //default state on play
        currentState = patrolState;
    }

    void Update()
    {
        currentState.UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Not monobehaviour OnTriggerEnter
        currentState.OnTriggerEnter(other);
    }
}

