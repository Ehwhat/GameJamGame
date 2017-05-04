using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingSphere : MonoBehaviour {

    public GameObject _healSphereMesh;
    public float _radius;
    public float _healPerSecond = 10;
    public float _disableTimer = 5;

    public LayerMask _playerMask;

    [SerializeField]
    private PlayerManager _owningPlayer;
    private float _startTime;

    public void initialise(PlayerManager player)
    {
        _owningPlayer = player;
    }

    void Start()
    {
        _startTime = Time.time;
        //transform.parent = _owningPlayer.transform;
    }

	// Update is called once per frame
	void Update ()
    {
        if (_owningPlayer != null)
        {
            moveToPlayer(_owningPlayer);
        }
        if (Time.time - _startTime < _disableTimer)
        {
            PlayerManager[] foundPlayers = FindAllPlayersInRadius(_radius);
            HealAllPlayers(foundPlayers);
        }else
        {
            Destroy(gameObject);
        }
           
	}

    PlayerManager[] FindAllPlayersInRadius(float radius)
    {
        List<PlayerManager> players = new List<PlayerManager>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, _playerMask);
        foreach(Collider c in hitColliders)
        {
            PlayerManager player = c.GetComponent<PlayerManager>();
            if(player != null)
            {
                players.Add(player);
            }
        }
        return players.ToArray();
    }

    void HealAllPlayers(PlayerManager[] players)
    {
        foreach(PlayerManager player in players)
        {
            player.HealFor(_healPerSecond * Time.deltaTime);
        }
    }
    void moveToPlayer(PlayerManager player) 
    {
        transform.position = player.transform.position;
    }
}
