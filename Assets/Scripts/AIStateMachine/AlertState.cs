using UnityEngine;
using System.Collections;

public class AlertState : IEnemyState
{

    private readonly StatePatternEnemy enemy;

    private float searchTimer;

    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Search();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Unused
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        searchTimer = 0f;
    }

    public void ToAlertState()
    {
        Debug.Log("Can't transition to smae state");
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
    }

    private void Look()
    {
        RaycastHit hit;

        //raycast to try and find the player
        //Line for Debuging, raycast is broken.
        Debug.DrawLine(enemy.eyes.transform.position, enemy.eyes.transform.forward, Color.yellow, 500);
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {

            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }

    private void Search()
    {
        //sets flag to be yellow to indicate state change.
        enemy.meshrendererFlag.material.color = Color.yellow;
        //stops the enemy moving(if it did)
        enemy.navMeshAgent.Stop();
        enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
        searchTimer += Time.deltaTime;

        if (searchTimer >= enemy.searchingDuration)
            ToPatrolState();
    }
}
