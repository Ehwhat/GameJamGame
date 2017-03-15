using UnityEngine;
using System.Collections;

public class BasicZombieEnemy : AIUnitManager<BasicZombieEnemy> {

	public string targetTag = "Player";
	public UnitManager currentTarget;

	public float speed = 5;
	public float damage = 10;
	public float attackRange = 1;

	// Use this for initialization
	void Start () {
		stateMachine = new FSM<BasicZombieEnemy, UnitState<BasicZombieEnemy>> ("Spawn", this, new BasicZombieEnemy_Spawn ("Spawn"), new BasicZombieEnemy_Chase("Chase"));
	}
	
	// Update is called once per frame
	void Update () {
		stateMachine.UpdateCurrentState ();
	}
}
