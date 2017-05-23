using UnityEngine;
using System.Collections;
using System;

public class Mine : MonoBehaviour, IActivatableObject {

    private InputContext _contextInput;
    public bool _disarmed;
    public float _explodeRadius = 3;
    public ParticleSystem _particleSystem;
    private bool _playerUsing = false;

    public void OnActivate(PlayerManager player)
    {
        if (!_disarmed)
        {
            _playerUsing = true;
            if (_contextInput != null )
            {
                _contextInput.Break();
            }
            _contextInput =  InputContextManager.CreateNewRandomInputContext(7, false, transform, GameManager.GetCamera().camera, player.controller, player, OnDisarm, OnFailure);
        }
    }

    public void OnDeactivate(PlayerManager player)
    {
        _playerUsing = false;
        if (_contextInput != null)
        {
            _contextInput.Break();
        }
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return !_disarmed;
    }

    void OnDisarm()
    {
        _disarmed = true;
    }
	
    void OnFailure()
    {
        Explode();
        _disarmed = true;
    }

    void Explode()
    {
        _particleSystem.Play();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explodeRadius);
        if(colliders.Length > 0)
        {
            foreach(Collider c in colliders)
            {
                DamageableObject d = c.GetComponent<DamageableObject>();
                if (d)
                {
                    d.DamageFor(1000);
                }
            }
        }
    }

}
