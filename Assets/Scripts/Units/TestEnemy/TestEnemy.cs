using UnityEngine;
using System.Collections;

[System.Serializable]
public class TestEnemy : AIUnitManager<TestEnemy> {

	void Start () {
	    
	}
	
	void Update () {
        aiStateMachine.Update();
	}
}
