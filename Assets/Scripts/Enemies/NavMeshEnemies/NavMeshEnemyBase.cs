using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class NavMeshEnemyBase : EnemyBase {

    [SerializeField]
    protected bool _debug = false;
    [SerializeField]
    private float _patrolStoppingDistance = 3;

    protected NavMeshAgent _agent;
    protected List<Vector3> _patrolPath;

    private Vector3 _destPatrolPoint;
    private int _destPatrolIndex;
    private int _currentPatrolIndex;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        StartCoroutine(CheckPatrolDistance());
    }

    public void SetPatrolPath(List<Vector3> path)
    {
        _patrolPath = path;
    }

    public void MoveAlongPatrolPath()
    {
        _destPatrolIndex = UnityEngine.Random.Range(0, _patrolPath.Count - 1);
        NextPatrolPoint();
        _enemyState = EnemyState.Patrolling;
        _agent.destination = _destPatrolPoint;
    }

    public void SetAgentPosition(Vector3 pos)
    {
        _agent.Warp(pos);
    }

    private void NextPatrolPoint()
    {
        if(_patrolPath.Count < 1)
        {
            return;
        }
        _destPatrolPoint = _patrolPath[_destPatrolIndex];
        _currentPatrolIndex = _destPatrolIndex;
        _destPatrolIndex = (_destPatrolIndex + 1) % _patrolPath.Count;
    }

    IEnumerator CheckPatrolDistance()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return CheckIfPatrolDistance(); });
            NextPatrolPoint();
            _agent.destination = _destPatrolPoint;
        }
    }

    private bool CheckIfPatrolDistance()
    {
        return (_enemyState == EnemyState.Patrolling && Vector3.Distance(transform.position, _destPatrolPoint) < _patrolStoppingDistance);
    }

    protected virtual void OnDrawGizmos()
    {
        if (_debug)
        {
            for (int i = 0; i < _patrolPath.Count-1; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_patrolPath[i], _patrolPath[i + 1]);
                if (i == _currentPatrolIndex)
                {
                    Gizmos.color = Color.red;
                }
                
                Gizmos.DrawSphere(_patrolPath[i], 2);
            }
            if (_patrolPath.Count - 1 == _currentPatrolIndex)
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(_patrolPath[_patrolPath.Count - 1], 2);
        }
    }

}
