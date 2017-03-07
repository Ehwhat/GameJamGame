using UnityEngine;
using System.Collections;

public class TestEnemy : AIUnitManager<TestEnemy> {

    public Vector3 patrolPosition;

	// Use this for initialization
	void Start () {
        stateMachine = new FSM<TestEnemy, UnitState<TestEnemy>>("Patrol", this, new TestEnemy_Patrol("Patrol"));
        patrolPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        stateMachine.UpdateCurrentState();
	}
}
