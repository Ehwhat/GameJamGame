using UnityEngine;
using System.Collections;
using FSM;
using System;

public class TestEnemy_Partol : State<TestEnemy> {

    public TestEnemy_Partol(string key) : base(key)
    {
    }

    public override void EndState()
    {
        Debug.Log("End");
    }

    public override void StartState()
    {
        Debug.Log("Start");
    }

    public override void UpdateState()
    {
        Debug.Log("Update");
    }

}
