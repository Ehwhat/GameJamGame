using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerManager : ControlledUnitManager {

    public PlayerUI playerUI;
    public Color playerColour = Color.red;
    public bool _isEnteringContext = false;

    HitData lastHit;
    int numRevivers = 0;
    float reviveTime = 0;
    float endReviveTime = 5;

    public float _activationRadius = 1f;
    [SerializeField]
    private IActivatableObject _nearestActivatable;
    private bool _isActivatePressed;

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
        if (!isDead && !_isEnteringContext)
        {
            CheckObjectInRange(_activationRadius, out _nearestActivatable);
            
            playerMovement.HandleMovement();
            playerAiming.HandleRotation();
            if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
            {
                playerShooting.Shoot();
            }
            if(controller.GetTrigger(XboxTrigger.LeftTrigger) > 0.1f && !_isActivatePressed && _nearestActivatable != null)
            {
                _isActivatePressed = true;
                _nearestActivatable.OnActivate(this);
            }else if(controller.GetTrigger(XboxTrigger.LeftTrigger) < 0.1f)
            {
                _isActivatePressed = false;
            }
        }
        else
        {
            if (numRevivers > 0)
            {
                reviveTime += Time.deltaTime * numRevivers;

                if(reviveTime >= endReviveTime)
                {
                    reviveTime = 0;
                   //numRevivers = 0;
                    RefillHealth();
                }
            }

            
        }

    }

    public void SetEnteringContext(bool active)
    {
        _isEnteringContext = active;
    }

    public override void OnDeath()
    {
        GetComponent<SphereCollider>().enabled = true;
        playerMovement.OnKill(lastHit);
        gameObject.tag = "DeadPlayer";
        StartCoroutine(DisableRotation());
        base.OnDeath();
    }

    public override void OnResurrect()
    {
        GetComponent<SphereCollider>().enabled = false;
        playerMovement.OnResurrect();
        gameObject.tag = "Player";
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

    bool CheckObjectInRange(float range, out IActivatableObject nearestActivatable)
    {
        IActivatableObject lastObject = _nearestActivatable;
        nearestActivatable = null;
        Collider[] foundColliders = Physics.OverlapSphere(transform.position, range);
        if (foundColliders.Length > 0) {
            IActivatableObject targetObject = null;
            float bestDistance = Mathf.Infinity;
            for(int i = 0; i < foundColliders.Length; i++)
            {
                IActivatableObject activatableObject = foundColliders[i].GetComponent<IActivatableObject>();
                if (activatableObject != null && Vector3.Distance(transform.position, foundColliders[i].transform.position) < bestDistance)
                {
                    
                    targetObject = activatableObject;
                    bestDistance = Vector3.Distance(transform.position, foundColliders[i].transform.position);
                }
            }
            nearestActivatable = targetObject;
            
            if(lastObject != null && lastObject != targetObject)
            {
                lastObject.OnDeactivate(this);
            }

            return targetObject != null;
        }else
        {
            return false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
       
        if (col.CompareTag("Player"))
        {
            
            //Debug.Log("Reviving");
            numRevivers++;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            numRevivers--;
            playerShooting.Shoot();
        }
        
    }

}
