using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerManager : ControlledUnitManager, IActivatableObject {

    public delegate DropManager.DropJob MarkerCallback(Vector3 markerPosition);

    public enum PlayerState
    {
        Alive,
        Dead,
        Context,
        PlaceMarker,
        Hiding
    }

    public PlayerUI playerUI;
    public Color playerColour = Color.red;
    public PlayerState _playerState = PlayerState.Alive;
    public Collider _playerCollider;

    HitData lastHit;

    public float _activationRadius = 1f;
    public float _deathDepth = -10f;
    [SerializeField]
    private List<Collider> _nearestActivatables;

    private GameObject _targetActivateableObject;

    private bool _isActivatePressed;

    [SerializeField]
    private DropMarkerManager markerPrefab;

    private InputContext currentContext;
    private DropMarkerManager marker;
    private DropManager.DropJob markerJob;

    public PlayerMovement playerMovement = new PlayerMovement();
    public PlayerAiming playerAiming = new PlayerAiming();
    public PlayerShooting playerShooting = new PlayerShooting();

    // Use this for initialization
    public void Start ()
    {
        GetPlayerController(playerIndex);
        
		playerMovement.Initalise(this, controller);
        playerAiming.Initalise(transform, controller);
        playerShooting.Initalise(playerAiming);
    }

    

    // Update is called once per frame
    void Update()
    {
        playerUI.SetPlayerColour(playerColour);

        playerUI.SetPlayerHealth(GetPlayerHealthPercent());
        playerUI.SetPlayerAmmo(playerShooting.GetAmmoClipPercent());
        playerUI.SetPlayerReloadTime(playerShooting.GetReloadPercent());

        AlexUIManager.SetPlayerAmmoBar((int)playerIndex, playerShooting.GetAmmoClipPercent());
        AlexUIManager.SetPlayerHealthBar((int)playerIndex, GetPlayerHealthPercent());
        AlexUIManager.SetWeaponName((int)playerIndex, playerShooting.GetWeaponName());

        GameObject targetObject = GetClosestActivateableObject();
        if (_targetActivateableObject != null && (_targetActivateableObject != targetObject || targetObject == null))
        {
            _targetActivateableObject.GetComponent<IActivatableObject>().OnDeactivate(this);
            InputContextManager.RemoveIndicator(_targetActivateableObject.transform);
        }
        else if (targetObject != null)
        {
            InputContextManager.PlaceIndicator(targetObject.transform);
        }
        
        _targetActivateableObject = targetObject;

        if (marker != null && markerJob != null)
        {
            float remainingJobTime = markerJob.GetTimePercent();
            if(remainingJobTime > 0f)
            {
                marker.SetDropTime(remainingJobTime);
            }else
            {
                Destroy(marker.gameObject);
                markerJob = null;
            }
        }

        if(transform.position.y < _deathDepth)
        {
            DamageFor(10000);
            transform.position = GameManager.GetPlayersCentre();
        }

        if (_playerState == PlayerState.Alive)
        {
            _nearestActivatables = CheckObjectsInRange(_activationRadius);
            
            playerMovement.HandleMovement();
            playerAiming.HandleRotation();
            playerShooting.HandleWeapon();
            if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
            {
                playerShooting.Shoot();
            }
            if(controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f && !_isActivatePressed && ActivateNearestActivatableObject())
            {
                _isActivatePressed = true;
            }else if(controller.GetTrigger(XboxTrigger.LeftTrigger) < 0.1f)
            {
                _isActivatePressed = false;
            }else if (controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f && !_isActivatePressed)
            {
                _isActivatePressed = true;
                InputContextManager.CreateNewManagerInputContext(transform, GameManager.GetCamera().camera, controller, this);
            }
        }else if(_playerState == PlayerState.Context)
        {
            if (controller.GetStickVector(XboxControlStick.LeftStick).magnitude > 0.1f)
            {
                BreakContext();
            }
        }
        else
        {
            
        }

    }

    public void SetPlayerActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetWeapon(Weapon weapon)
    {
        playerShooting.SetWeapon(weapon);
    }

    public void EnterContext(InputContext context)
    {
        _playerState = PlayerState.Context;
        currentContext = context;
    }

    public void BreakContext()
    {
        if (currentContext != null)
        {
            _playerState = PlayerState.Alive;
            currentContext.Break();
            currentContext = null;
        }
    }

    public void EnterDropPlace(MarkerCallback callback)
    {
        marker = Instantiate<DropMarkerManager>(markerPrefab);
        marker.transform.position = transform.position;
        StartCoroutine(DropMarkerPlace(marker, callback));
    }

    IEnumerator DropMarkerPlace(DropMarkerManager marker, MarkerCallback callback)
    {
        while (true)
        {
            _playerState = PlayerState.PlaceMarker;
            if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f || controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f)
            {
                markerJob = callback(marker.transform.position);
                _playerState = PlayerState.Alive;
                yield break;
            }

            Vector2 stickVector = controller.GetStickVector(XboxControlStick.RightStick);
            if(stickVector.magnitude > 0.1f)
            {
                marker.transform.position += Quaternion.AngleAxis(45, Vector3.up) * new Vector3(stickVector.x, 0, stickVector.y).normalized * 20 * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void HidePlayer()
    {
        foreach(Transform t in transform)
        {
            if(t == transform)
            {
                continue;
            }
            t.gameObject.SetActive(false);
        }
        playerMovement.FreezePlayer(true);
        _playerCollider.isTrigger = true;
        _playerState = PlayerState.Hiding;
    }

    public void UnhidePlayer()
    {
        foreach (Transform t in transform)
        {
            if (t == transform)
            {
                continue;
            }
            t.gameObject.SetActive(true);
        }
        playerMovement.FreezePlayer(false);
        _playerCollider.isTrigger = false;
        _playerState = PlayerState.Alive;
    }

    public float GetPlayerHealthPercent()
    {
        return (currentHealth / maxHealth);
    }

    public void GiveAmmoPack()
    {
        playerShooting.GiveAmmoPack();
    }

    public override void OnDeath()
    {
        BreakContext();
        _playerState = PlayerState.Dead;
        playerMovement.OnKill(lastHit);
        StartCoroutine(DisableRotation());
        base.OnDeath();
    }

    public override void OnResurrect()
    {
        _playerState = PlayerState.Alive;
        playerMovement.OnResurrect();
        base.OnResurrect();
    }

    public override void OnObjectHit(HitData hit)
    {
        if (!isDead)
        {
            lastHit = hit;
        }
      
        base.OnObjectHit(hit);
    }

    IEnumerator DisableRotation()
    {
        yield return new WaitForSeconds(1.0f);
        playerMovement.DisableRotation();
    }


    GameObject GetClosestActivateableObject()
    {
        if (_nearestActivatables.Count > 0)
        {
            _nearestActivatables = _nearestActivatables.Where(o=> o != null).OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).ToList();
            foreach (Collider activatable in _nearestActivatables)
            {
                IActivatableObject act = activatable.GetComponent<IActivatableObject>();
                if (act != null)
                {
                    if (act.ActivateCheck(this))
                    {
                        return activatable.gameObject;
                    }
                }
            }
        }
        return null;
    }

    bool ActivateNearestActivatableObject()
    {
        if (_targetActivateableObject != null)
        {
            IActivatableObject act = _targetActivateableObject.GetComponent<IActivatableObject>();
            if (act != null)
            {
                act.OnActivate(this);
                return true;
            }
        }
        return false;
    }

    List<Collider> CheckObjectsInRange(float range)
    {
        return Physics.OverlapSphere(transform.position, range).ToList();
    }

    public void OnActivate(PlayerManager player)
    {
        InputContextManager.CreateNewRandomInputContext(7, true, transform, GameManager.GetCamera().camera, player.controller, player, HealToHalf );///
    }

    public void HealToHalf()
    {
        Debug.Log("heal");
        HealToPercent(0.5f);
    }

    public void OnDeactivate(PlayerManager player)
    {
        Debug.Log("Oh nooooo");
    }

    public bool ActivateCheck(PlayerManager player)
    {
        return (_playerState == PlayerState.Dead);
    }

}
