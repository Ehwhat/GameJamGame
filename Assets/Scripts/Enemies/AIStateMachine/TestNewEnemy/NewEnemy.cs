using UnityEngine;
using System.Collections;


public class NewEnemy : AIUnitManager<TestEnemy>
{

    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 20f;
    public float attackRange = 10f;

    public float minPathUpdateTime = 0.2f;
    public float pathUpdateMoveThreshold = 0.5f;

    public float speed = 50;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    public Path path;

    //where the enemy will path to in patrol state, can be changed later as enemy doesnt actually move
    public Transform[] wayPoints;

    //position to raycast from to find player
    public Transform eyes;
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    //state indicator above enemy
    public MeshRenderer meshrendererFlag;

    public Transform chaseTarget;

    private FSM<NewEnemy, TriggerState<NewEnemy>> stateMachine;

    //Rigidbody additions
    public Rigidbody rb;
    public Transform movePoint; // What the enemy moves towards when chasing the player

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        movePoint = gameObject.transform.Find("MovePoint").transform;
        stateMachine = new FSM<NewEnemy, TriggerState<NewEnemy>>("Patrol", this, new NewEnemy_Patrol("Patrol", this), new NewEnemy_Alert("Alert", this), new NewEnemy_Chase("Chase", this), new NewEnemy_Attack("Attack", this));
        if (stateMachine == null)
            Debug.Log("null");
    }

    public void Update()
    {
        stateMachine.UpdateCurrentState();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        stateMachine.currentState.OnTriggerEnter(other);
    }

}
