using UnityEngine;
using System.Collections;
using FSM;
using System;

[System.Serializable]
public class TestEnemy_Patrol : State<TestEnemy>
{

    public new string stateKey = "Patrol";

    public override void EndState(TestEnemy instance)
    {
        Debug.Log("End State");
    }

    public override void StartState(TestEnemy instance)
    {
        Debug.Log("Start State");
    }

    public override void UpdateState(TestEnemy instance)
    {
        Debug.Log("Update State");
    }
}
