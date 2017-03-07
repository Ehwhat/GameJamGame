using UnityEngine;
using System.Collections;
using System;

public class TestEnemy_Patrol : UnitState<TestEnemy> {



    public TestEnemy_Patrol(string key) : base(key)
    {
    }


    public override void EndState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        
    }

    public override void OnTriggerExit(Collider other)
    {
        
    }

    public override void StartState()
    {
        instance.StartCoroutine(UpdatePath());
    }

    public override void UpdateState()
    {
        
    }

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
        PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.target.position, OnPathFound));

        float sqrMoveThreshold = instance.pathUpdateMoveThreshold * instance.pathUpdateMoveThreshold;
        Vector3 targetPosOld = instance.target.position;

        while (true)
        {
            yield return new WaitForSeconds(instance.minPathUpdateTime);
            if ((instance.target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.target.position, OnPathFound));
                targetPosOld = instance.target.position;
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
                Quaternion targetRotation = Quaternion.LookRotation(instance.path.lookPoints[pathIndex] - instance.transform.position);
                instance.transform.rotation = Quaternion.Lerp(instance.transform.rotation, targetRotation, Time.deltaTime * instance.turnSpeed);
                instance.transform.Translate(Vector3.forward * Time.deltaTime * instance.speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

}
