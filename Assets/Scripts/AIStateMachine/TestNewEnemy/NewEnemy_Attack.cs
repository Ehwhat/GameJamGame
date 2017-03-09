using UnityEngine;
using System.Collections;

public class NewEnemy_Attack : TriggerState<NewEnemy>
{

    NewEnemy currentEnemy;
    RaycastHit hit;

    private float searchTimer;

    public NewEnemy_Attack(string key, NewEnemy enemy) : base(key)
    {
        currentEnemy = enemy;
    }

    public override void EndState()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void StartState()
    {
        searchTimer = 0f;
    }

    public override void UpdateState()
    {
        Look();
        Attack();
    }

    public void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (currentEnemy.chaseTarget.position + currentEnemy.offset) - currentEnemy.eyes.transform.position;
        //instead of raycasting foraward, it will be towards the player as the enemy has found the player

        //Debug.DrawRay(enemy.eyes.transform.position, enemy.transform.forward * enemy.sightRange, Color.yellow, 500);
        if (Physics.Raycast(currentEnemy.eyes.transform.position, enemyToTarget, out hit, currentEnemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            currentEnemy.transform.LookAt(currentEnemy.chaseTarget.position);
            currentEnemy.chaseTarget = hit.transform;
        }
        else
        {
            RequestStateChange("Alert");
        }
    }

    public void Attack()
    {

    }

}
