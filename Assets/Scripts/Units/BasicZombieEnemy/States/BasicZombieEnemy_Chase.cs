using UnityEngine;
using System.Collections;

public class BasicZombieEnemy_Chase : UnitState<BasicZombieEnemy> {

	float targetDistance;

	bool isCloseEnoughToAttack = false;

	public BasicZombieEnemy_Chase(string key) : base(key)
	{
	}

	public override void EndState(){}
	public override void StartState(){}
	public override void UpdateState(){
		MoveTowardsTarget ();
	}
	public override void FixedUpdateState(){}

	public override void OnTriggerEnter(Collider other){}
	public override void OnTriggerExit(Collider other){}

	public override void OnDrawGizmos(){}

	void MoveTowardsTarget(){
		Vector3 targetVector = instance.transform.position - instance.currentTarget.transform.position;
		targetDistance = targetVector.magnitude;
		if (targetDistance <= instance.attackRange) {
			isCloseEnoughToAttack = true;
			AttackTarget ();
		}
		instance.transform.position += -targetVector.normalized * instance.speed * Time.deltaTime;
	}

	void AttackTarget(){
		instance.currentTarget.DamageFor (instance.damage);
	}

}
