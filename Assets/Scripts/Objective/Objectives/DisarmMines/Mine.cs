using UnityEngine;
using System.Collections;
using System;

public class Mine : MonoBehaviour, IActivatableObject {

    private InputContext _contextInput;
    public bool _disarmed;

    public void OnActivate(PlayerManager player)
    {
        if (!_disarmed)
        {
            if (_contextInput != null)
            {
                _contextInput.Break();
            }
            _contextInput =  InputContextManager.CreateNewRandomInputContext(7, false, transform, GameManager.GetCamera().camera, player.controller, player, OnDisarm, OnFailure);
        }
    }

    public void OnDeactivate(PlayerManager player)
    {
        if (_contextInput != null)
        {
            _contextInput.Break();
        }
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return true;
    }

    void OnDisarm()
    {
        _disarmed = true;
    }
	
    void OnFailure()
    {
        _disarmed = true;
    }

}
