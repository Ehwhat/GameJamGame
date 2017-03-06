using UnityEngine;
using System.Collections;

public class NewEnemy : MonoBehaviour {

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

    public Transform chaseTarget;
        
    public NavMeshAgent navMeshAgent;

    private FSM<NewEnemy, TriggerState<NewEnemy>> stateMachine;

    public void Start()
    {
        stateMachine = new FSM<NewEnemy, TriggerState<NewEnemy>>("Patrol", this, new NewEnemy_Patrol("Patrol", this), new NewEnemy_Alert("Alert", this));
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
