using UnityEngine;
using System.Collections;
using System;

public class EntryPoint : MonoBehaviour, IActivatableObject
{

    public enum EntryPointState
    {
        Empty,
        Occupied
    }

    public PlayerManager _playerInside;
    public EntryPointState _entryPointState;

    private Vector3 _playerOldLocalPosition;
    private Transform _playerOldParent;

    private bool _leavePressed;

    public bool ActivateCheck(PlayerManager player)
    {
        return (_playerInside == null) ;
    }

    public void OnActivate(PlayerManager player)
    {
        _leavePressed = true;
        HoldPlayer(player);
    }

    public void OnDeactivate(PlayerManager player)
    {
        //throw new NotImplementedException();
    }

    public void Update()
    {
        if(_playerInside != null)
        {
            if(_playerInside.controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f && !_leavePressed)
            {
                ReleasePlayer();
            }else if(_playerInside.controller.GetTrigger(XboxTrigger.LeftTrigger) < 0.1f)
            {
                _leavePressed = false;
            }
        }
    }

    public void ReleasePlayer()
    {
        if (_playerInside != null)
        {
            _playerInside.transform.position = transform.position + _playerOldLocalPosition;
            _playerInside.transform.parent = _playerOldParent;
            _playerInside.UnhidePlayer();
            _playerInside = null;
        }
    }

    public void HoldPlayer(PlayerManager player)
    {
        _playerInside = player;
        _playerInside.HidePlayer();

        _playerOldLocalPosition = _playerInside.transform.position - transform.position ;
        _playerOldParent = _playerInside.transform.parent;

        _playerInside.transform.position = transform.position;
        _playerInside.transform.parent = transform;
    }

    public PlayerControlManager.PlayerController GetPlayerController()
    {
        if(_playerInside != null)
        {
            return _playerInside.controller;
        }
        return null;
    }
}
