using UnityEngine;
using System.Collections;


public class PlayerManager : ControlledUnitManager {

    HitData lastHit;
    int numRevivers = 0;
    float reviveTime = 0;
    float endReviveTime = 10;

    public PlayerMovement playerMovement = new PlayerMovement();
    public PlayerAiming playerAiming = new PlayerAiming();
    public PlayerShooting playerShooting = new PlayerShooting();

	// Use this for initialization
	void Start ()
    {
        GetPlayerController(playerIndex);
		playerMovement.Initalise(this, controller);
        playerAiming.Initalise(transform, controller);
        playerShooting.Initalise(playerAiming);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            playerMovement.HandleMovement();
            playerAiming.HandleRotation();
            if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
            {
                Debug.Log("Shooting");
                playerShooting.Shoot();
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

    public override void OnDeath()
    {
        
        playerMovement.OnKill(lastHit);
        gameObject.tag = "DeadPlayer";
        StartCoroutine(DisableRotation());
        base.OnDeath();
    }

    public override void OnResurrect()
    {
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
