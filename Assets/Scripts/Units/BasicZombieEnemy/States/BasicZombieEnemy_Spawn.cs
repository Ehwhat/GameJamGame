using UnityEngine;
using System.Collections;

public class BasicZombieEnemy_Spawn : UnitState<BasicZombieEnemy> {

	public BasicZombieEnemy_Spawn(string key) : base(key)
	{
	}

	public override void EndState(){}
	public override void StartState(){}
	public override void UpdateState(){
		if (instance.currentTarget == null) {
			instance.currentTarget = findNearestTarget ();
		} else {
			RequestStateChange ("Chase");
		}
	}
	public override void FixedUpdateState(){}

	public override void OnTriggerEnter(Collider other){}
	public override void OnTriggerExit(Collider other){}

	public override void OnDrawGizmos(){}

	UnitManager findNearestTarget(){
		GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag ("Player");

		UnitManager closetUnit = null;
		float closestUnitDistance = Mathf.Infinity;

		foreach (GameObject target in potentialTargets) {
			UnitManager manager = target.GetComponent<UnitManager> ();
			if (manager != null && !manager.isDead) {
				if (Vector3.Distance (instance.transform.position, target.transform.position) < closestUnitDistance) {
					closetUnit = manager;
				}
			}
		}

		return closetUnit;

	}

}
