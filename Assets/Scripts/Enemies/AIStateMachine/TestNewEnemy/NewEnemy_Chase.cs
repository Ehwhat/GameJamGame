using UnityEngine;
using System.Collections;

public class NewEnemy_Chase : TriggerState<NewEnemy>
{

    NewEnemy currentEnemy;
    RaycastHit hit;

    public NewEnemy_Chase(string key, NewEnemy enemy) : base(key)
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
        instance.StartCoroutine(UpdatePath());
    }

    public override void UpdateState()
    {
        Look();
        Chase();
    }

    //implemented from TestEnemy_patrol

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            instance.path = new Path(waypoints, instance.transform.position, instance.turnDst, instance.stoppingDst);
            instance.StopCoroutine(FollowPath());
            instance.StartCoroutine(FollowPath());
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.chaseTarget.position, OnPathFound));

        float sqrMoveThreshold = instance.pathUpdateMoveThreshold * instance.pathUpdateMoveThreshold;
        Vector3 targetPosOld = instance.chaseTarget.position;

        while (true)
        {
            yield return new WaitForSeconds(instance.minPathUpdateTime);
            if ((instance.chaseTarget.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.chaseTarget.position, OnPathFound));
                targetPosOld = instance.chaseTarget.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        instance.transform.LookAt(instance.path.lookPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(instance.transform.position.x, instance.transform.position.z);
            while (instance.path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == instance.path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= instance.path.slowDownIndex && instance.stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(instance.path.turnBoundaries[instance.path.finishLineIndex].DistanceFromPoint(pos2D) / instance.stoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }
                //Quaternion targetRotation = Quaternion.LookRotation(new Vector3(instance.path.lookPoints[pathIndex].x, 0.5f, instance.path.lookPoints[pathIndex].z) - instance.transform.position);
                // instance.transform.rotation = Quaternion.Lerp(instance.transform.rotation, targetRotation, Time.deltaTime * instance.turnSpeed);
                //instance.transform.Translate(Vector3.forward * Time.deltaTime * instance.speed * speedPercent, Space.Self);
                //
                //Generate a directional velocity based on the targets location, and the current RB
                Vector3 velocity = (new Vector3(currentEnemy.rb.position.x, currentEnemy.rb.position.y, currentEnemy.rb.position.z) - new Vector3(instance.chaseTarget.position.x, currentEnemy.rb.position.y, instance.chaseTarget.position.z)).normalized * Time.fixedDeltaTime;

                
                currentEnemy.rb.MovePosition((currentEnemy.rb.position - velocity));
                yield return new WaitForFixedUpdate();
              //  Debug.Log("Enemy Velocity = " + velocity);
            }

            yield return null;
        }
    }


    // End of TestEnemy_Patrol

    private void Look()
    {

        Vector3 enemyToTarget = (currentEnemy.chaseTarget.position + currentEnemy.offset) - currentEnemy.eyes.transform.position;
        //instead of raycasting foraward, it will be towards the player as the enemy has found the player

        Debug.DrawRay(currentEnemy.eyes.transform.position, currentEnemy.transform.forward * currentEnemy.sightRange, Color.yellow, 500);
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
    private void Chase()
    {
        currentEnemy.meshrendererFlag.material.color = Color.red;

        if (Vector3.Distance(hit.transform.position, currentEnemy.transform.position) > currentEnemy.attackRange)
        {
            //path towards the player 
            //instance.StartCoroutine(UpdatePath());
        }
        else
        {
           // Debug.Log("Path stopped");
            //instance.StopCoroutine(UpdatePath());
        }

       // Debug.Log(Vector3.Distance(hit.transform.position, currentEnemy.transform.position));

    }
}
