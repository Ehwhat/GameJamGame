using UnityEngine;
using System.Collections;
using System;

public class ZombieEnemy_Wander : PathfindingUnitState<ZombieEnemy> {

    private Vector3 wanderOrigin;

    public ZombieEnemy_Wander(string key) : base(key)
    {
    }

    public override void EndState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wanderOrigin, instance.wanderRadius);
    }

    public override void OnTriggerEnter(Collider other)
    {
        
    }

    public override void OnTriggerExit(Collider other)
    {
        
    }

    public override void StartState()
    {
        GenerateNewWanderPoint();
    }

    public override void UpdateState()
    {
        if(Vector3.Distance(instance.currentTarget, instance.transform.position) < 5)
        {
            GenerateNewWanderPoint();

        }
    }

    private void GenerateNewWanderPoint()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle*instance.wanderRadius;
        SetTarget(wanderOrigin + new Vector3(randomCircle.x,0,randomCircle.y));
    }
    
}
