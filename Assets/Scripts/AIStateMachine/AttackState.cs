using UnityEngine;
using System.Collections;
using System;

public class AttackState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public AttackState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        Look();
        Chase();
        Attack();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Unused
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }
    
    public void ToAttackState()
    {
        Debug.Log("Can't transition to same state");
    }
    public void Chase()
    {
        enemy.meshrendererFlag.material.color = Color.cyan;
    }

    public void Look()
    {
        RaycastHit hit;
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
    public void Attack()
    {

    }
}
