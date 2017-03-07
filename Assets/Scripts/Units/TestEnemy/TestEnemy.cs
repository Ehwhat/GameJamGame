using UnityEngine;
using System.Collections;

public class TestEnemy : AIUnitManager<TestEnemy> {

    public float minPathUpdateTime = 0.2f;
    public float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 50;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    public Path path;

    // Use this for initialization
    void Start () {
        stateMachine = new FSM<TestEnemy, UnitState<TestEnemy>>("Patrol", this, new TestEnemy_Patrol("Patrol"));
        if (stateMachine == null)
            Debug.Log("null");
	}
	
	// Update is called once per frame
	void Update () {
        //stateMachine.UpdateCurrentState();
	}
}
