using UnityEngine;
using System.Collections;
using FSM;
using System;

public abstract class PathfindingUnitState<T> : State<T, PathfindingUnitState<T>> where T : PathfindingAIUnitManager<T>{

    public PathfindingUnitState(string key) : base(key)
    {

    }

    public override void StartStateInternal()
    {
        base.StartStateInternal();
        instance.StartCoroutine(UpdatePath());
    }

    public override void EndStateInternal()
    {
        base.EndStateInternal();
        instance.StopCoroutine(FollowPath());
        instance.StopCoroutine(UpdatePath());
    }

    public abstract override void EndState();
    public abstract override void StartState();
    public abstract override void UpdateState();
    public abstract void FixedUpdateState();

    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerExit(Collider other);

    public abstract void OnDrawGizmos();

    public void SetTarget(Vector3 target)
    {
        instance.SetTarget(target);
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
        PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.currentTarget, OnPathFound));

        float sqrMoveThreshold = instance.pathUpdateMoveThreshold * instance.pathUpdateMoveThreshold;
        Vector3 targetPosOld = instance.currentTarget;

        while (true)
        {
            yield return new WaitForSeconds(instance.minPathUpdateTime);
            if ((instance.currentTarget - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(instance.transform.position, instance.currentTarget, OnPathFound));
                targetPosOld = instance.currentTarget;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        //instance.transform.LookAt(instance.path.lookPoints[0]);
        instance.transform.LookAt(new Vector3(instance.path.lookPoints[0].x, instance.transform.position.y, instance.path.lookPoints[0].z));

        float speedPercent = 1;

        while (followingPath && instance.path.turnBoundaries.Length > pathIndex)
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
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(instance.path.lookPoints[0].x, instance.transform.position.y, instance.path.lookPoints[0].z) - instance.transform.position);
                instance.transform.rotation = Quaternion.Lerp(instance.transform.rotation, targetRotation, Time.deltaTime * instance.turnSpeed);
                instance.transform.Translate(Vector3.forward * Time.deltaTime * instance.speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

}
