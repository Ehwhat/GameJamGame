using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemyNavMesh : NavMeshEnemyBase, IHitTarget
{

    public struct PlayerFoundInfo
    {
        public PlayerManager _player;
        public float _distance;

        public PlayerFoundInfo(PlayerManager player, float distance)
        {
            _player = player;
            _distance = distance;
        }

    }

    public float _health = 50;

    public LayerMask _playerMask;
    public float _alertDistance = 32;
    public float _attackDistance = 8;

    public WeaponManager _weaponManager;
    public Rigidbody _rigidbody;

    Vector3 currentTarget;

    public override void OnSquadSpawn()
    {
        
    }

    // Use this for initialization
    void Start () {
        MoveAlongPatrolPath();
	}
	
	// Update is called once per frame
	void Update () {
        PlayerFoundInfo[] foundPlayers = CheckForPlayers();
        if (_enemyState != EnemyState.Dead)
        {
            if (foundPlayers.Length > 0)
            {
                for (int i = 0; i < foundPlayers.Length; i++)
                {
                    if (foundPlayers[i]._distance < _attackDistance && foundPlayers[i]._player._playerState != PlayerManager.PlayerState.Dead)
                    {
                        _weaponManager.AimWeaponAt(foundPlayers[i]._player.transform);
                        _weaponManager.FireWeapon();
                        break;
                    }
                    else
                    {
                        _enemyState = EnemyState.Alert;
                        _agent.SetDestination(GameManager.GetPlayersCentre() + UnityEngine.Random.insideUnitSphere*3);
                    }
                }
            }
            else
            {
                _enemyState = EnemyState.Patrolling;
            }
        }

    }


    private PlayerFoundInfo[] CheckForPlayers()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, _alertDistance, _playerMask);
        List<PlayerFoundInfo> players = new List<PlayerFoundInfo>();
        foreach (Collider c in col)
        {
            PlayerManager player = c.GetComponent<PlayerManager>();
            if (player != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
                {
                    if (hit.collider.gameObject == player.gameObject)
                    {
                        players.Add(new PlayerFoundInfo(player, Vector3.Distance(transform.position, player.transform.position)));
                    }
                }
            }
        }
        return players.ToArray();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentTarget, 2);
    }

    public void OnDamageHit(HitData hit)
    {
        _health -= hit.damage;
        if(_health <= 0)
        {
            _enemyState = EnemyState.Dead;
            Die(hit);
        }
    }

    void Die(HitData lastHit)
    {
        Vector3 hitPoint = lastHit.rayHit.point;
        Vector3 forceHit = (lastHit.rayHit.point-_rigidbody.position).normalized * -100;
        _agent.enabled = false;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.AddForceAtPosition(forceHit, hitPoint);
        StartCoroutine(DisableRotation());
    }

    IEnumerator DisableRotation()
    {

        yield return new WaitForSeconds(3.0f);
        _rigidbody.angularDrag = 2.0f;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.useGravity = false;
        transform.GetComponent<CapsuleCollider>().enabled = false;
    }
}
