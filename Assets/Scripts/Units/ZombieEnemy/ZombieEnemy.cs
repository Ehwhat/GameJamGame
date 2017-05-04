using UnityEngine;
using System.Collections;

public class ZombieEnemy : PathfindingAIUnitManager<ZombieEnemy> {

    public float wanderRadius;

	// Use this for initialization
	void Start () {
        stateMachine = new FSM<ZombieEnemy, PathfindingUnitState<ZombieEnemy>>("Wander", this, new ZombieEnemy_Wander("Wander"));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
