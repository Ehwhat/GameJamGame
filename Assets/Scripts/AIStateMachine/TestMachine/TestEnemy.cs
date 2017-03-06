using UnityEngine;
using System.Collections;

public class TestEnemy : MonoBehaviour {

    public FSM<TestEnemy> stateMachine;

	// Use this for initialization
	void Start () {
        stateMachine = new FSM<TestEnemy>("Start", this, new TestEnemy_Partol("Start"));
	}
	
	// Update is called once per frame
	void Update () {
        stateMachine.UpdateCurrentState();
	}
}
