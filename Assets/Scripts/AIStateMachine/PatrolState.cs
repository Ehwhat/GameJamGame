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
        //Debug.Log("patrol Looking");
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.forward, out hit) && hit.collider.CompareTag("Player"))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log(" PATROL LOOK HIT");
                enemy.chaseTarget = hit.transform;
                ToChaseState();
            }
        }
    }

    void Patrol()
    {

        enemy.meshrendererFlag.material.color = Color.green;
        
        //movement?
        
        //enemy.navMeshAgent.destination = enemy.wayPoints[nextWayPoint].position;
        //enemy.navMeshAgent.Resume();

        //if(enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        //{
        //    nextWayPoint = (nextWayPoint + 1) % enemy.wayPoints.Length;
        //}
    }
}
