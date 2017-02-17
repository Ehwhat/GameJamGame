using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState {

    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;

    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Patrol();

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ToAlertState();
        }
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
        enemy.currentState = enemy.chaseState;
    }

    private void Look()
    {
        RaycastHit hit;

        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }

    void Patrol()
    {
        enemy.meshrendererFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.wayPoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if(enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            nextWayPoint = (nextWayPoint + 1) % enemy.wayPoints.Length;
        }
    }
}
