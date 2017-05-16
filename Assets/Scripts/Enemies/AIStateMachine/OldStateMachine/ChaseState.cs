using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{ 
    private readonly StatePatternEnemy enemy;

    RaycastHit hit;

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
        Debug.Log("Can't transition to same state");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        Debug.Log("Can't transition to same state");
    }

    public void ToAttackState()
    {
        enemy.currentState = enemy.attackState;
    }
    private void Look()
    {
        
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        //instead of raycasting foraward, it will be towards the player as the enemy has found the player

        //Debug.DrawRay(enemy.eyes.transform.position, enemy.transform.forward * enemy.sightRange, Color.yellow, 500);
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.transform.LookAt(enemy.chaseTarget.position);
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

        if(Vector3.Distance(hit.transform.position, enemy.transform.position) > enemy.attackRange)
        {
               //path towards the player 
        }
        else
        {
            ToAttackTimer();
        }

        Debug.Log(Vector3.Distance(hit.transform.position, enemy.transform.position));
        
    }

    private IEnumerator ToAttackTimer()
    {
        yield return new WaitForSeconds(5f);
        ToAttackState();
    }
}
