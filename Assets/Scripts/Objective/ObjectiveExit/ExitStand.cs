using UnityEngine;
using System.Collections;
using System;

public class ExitStand : MonoBehaviour, IActivatableObject {

    public delegate void ExitStandCallback();

    public ExitStandCallback _callback;

    private bool _hasBeenActivated = false;
    private InputContext _contextInput;
    public void SetCallback(ExitStandCallback callback)
    {
        _callback = callback;
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return !_hasBeenActivated;
    }

    public void OnActivate(PlayerManager player)
    {
        if (_contextInput != null)
        {
            _contextInput.Break();
        }
        _contextInput = InputContextManager.CreateNewRandomInputContext(7, true, transform, GameManager.GetCamera().camera, player.controller, player, OnActivated, null);
    }

    public void OnActivated()
    {
        _hasBeenActivated = true;
        _callback();
    }

    public void OnDeactivate(PlayerManager player)
    {
       
    }
}
