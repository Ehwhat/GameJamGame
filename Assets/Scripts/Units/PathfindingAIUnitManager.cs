using UnityEngine;
using System.Collections;

public class PathfindingAIUnitManager<T> : AIUnitManager<T> where T : PathfindingAIUnitManager<T>
{

    protected new FSM<T, PathfindingUnitState<T>> stateMachine;

    public float minPathUpdateTime = 0.2f;
    public float pathUpdateMoveThreshold = 0.5f;

    public Vector3 currentTarget;
    public float speed = 50;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    public Path path;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTarget(Vector3 target)
    {
        currentTarget = target;
    }
}
