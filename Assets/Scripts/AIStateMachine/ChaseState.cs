using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{ 
    private readonly StatePatternEnemy enemy;

    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Chase();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Unused
    }


    public void ToPatrolState()
    {
        Debug.Log("Can't transition to smae state");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        Debug.Log("Can't transition to smae state");
    }

    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        //instead of raycasting foraward, it will be towards the player as the enemy has found the player
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
        }
        else
        {
            ToAlertState();
        }
    }

    private void Chase()
    {
        enemy.meshrendererFlag.material.color = Color.red;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();
    }
}
