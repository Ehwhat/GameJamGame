using UnityEngine;
using System.Collections;
using System;

public class NewEnemy_Patrol : TriggerState<NewEnemy> {

    NewEnemy currentEnemy;


    public NewEnemy_Patrol(string key, NewEnemy enemy) : base(key)
    {
        currentEnemy = enemy;
    }

    public override void EndState()
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RequestStateChange("Alert");
        }
    }

    public override void StartState()
    {

    }

    public override void UpdateState()
    {
        Look();
        Patrol();
    }

    private void Look()
    {
        RaycastHit hit;
        //Debug.Log("patrol Looking");
        if (Physics.Raycast(currentEnemy.eyes.transform.position, currentEnemy.eyes.forward, out hit) && hit.collider.CompareTag("Player"))
        {
            if (hit.collider.CompareTag("Player"))
            {
                //Debug.Log(" PATROL LOOK HIT");
                currentEnemy.chaseTarget = hit.transform;
                RequestStateChange("Alert");
            }
        }
    }




    void Patrol()
    {
        currentEnemy.meshrendererFlag.material.color = Color.green;
    }
}
