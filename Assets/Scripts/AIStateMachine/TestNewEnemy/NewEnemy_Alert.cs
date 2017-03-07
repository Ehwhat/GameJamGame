using UnityEngine;
using System.Collections;

public class NewEnemy_Alert : TriggerState<NewEnemy>
{

    NewEnemy currentEnemy;

    private float searchTimer;

    public NewEnemy_Alert(string key, NewEnemy enemy) : base(key)
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
        Search();
    }

    private void Look()
    {
        RaycastHit hit;

        //Debug.Log("Alert Looking");

        //raycast to try and find the player
        //Line for Debuging, raycast is broken.
        //Debug.DrawRay(enemy.eyes.transform.position, enemy.transform.forward* enemy.sightRange, Color.yellow, 500);
        if (Physics.Raycast(currentEnemy.eyes.transform.position, currentEnemy.transform.forward, out hit, currentEnemy.sightRange))
        {
            // Debug.Log("I saw something!");
            if (hit.collider.CompareTag("Player"))
            {
                //  Debug.Log(" ALERT SEARCH HIT");
                currentEnemy.chaseTarget = hit.transform;
                RequestStateChange("Chase");
            }

        }

    }

    private void Search()
    {
        //sets flag to be yellow to indicate state change.
        currentEnemy.meshrendererFlag.material.color = Color.yellow;
        //stops the enemy moving(if it did)
        //enemy.navMeshAgent.Stop();
        currentEnemy.transform.Rotate(0, currentEnemy.searchingTurnSpeed * Time.deltaTime, 0);
        searchTimer += Time.deltaTime;

        if (searchTimer >= currentEnemy.searchingDuration)
        {
            Debug.Log("going back to patroling");
            RequestStateChange("Patrol");
        }

    }

}
