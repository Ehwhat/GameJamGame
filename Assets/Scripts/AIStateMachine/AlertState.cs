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
        Debug.Log("Can't transition to same state");
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
    }
    public void ToAttackState()
    {
        Debug.Log("this should never be possible.");
    }

    private void Look()
    {
        RaycastHit hit;

        //Debug.Log("Alert Looking");

        //raycast to try and find the player
        //Line for Debuging, raycast is broken.
        //Debug.DrawRay(enemy.eyes.transform.position, enemy.transform.forward* enemy.sightRange, Color.yellow, 500);
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.transform.forward, out hit, enemy.sightRange))
        {
           // Debug.Log("I saw something!");
            if(hit.collider.CompareTag("Player"))
            {
              //  Debug.Log(" ALERT SEARCH HIT");
                enemy.chaseTarget = hit.transform;
                ToChaseState();
            }
            
        }
        
    }

    private void Search()
    {
        //sets flag to be yellow to indicate state change.
        enemy.meshrendererFlag.material.color = Color.yellow;
        //stops the enemy moving(if it did)
        //enemy.navMeshAgent.Stop();
        enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
        searchTimer += Time.deltaTime;

        if (searchTimer >= enemy.searchingDuration)
        {
            Debug.Log("going back to patroling");
            ToPatrolState();
        }
           
    }
}
