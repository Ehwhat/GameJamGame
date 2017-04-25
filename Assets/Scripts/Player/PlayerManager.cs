﻿using UnityEngine;
using System.Collections;


public class PlayerManager : ControlledUnitManager {

    HitData lastHit;
    int numRevivers = 0;
    float reviveTime = 0;
    float endReviveTime = 10;

    public PlayerMovement playerMovement = new PlayerMovement();
    public PlayerAiming playerAiming = new PlayerAiming();
    public PlayerShooting playerShooting = new PlayerShooting();

    //Additions for demo
    public bool isP1 = false;
    Objective ob;
    Transform obArrow;
	// Use this for initialization
	public void Start ()
    {
        GetPlayerController(playerIndex);
		playerMovement.Initalise(this, controller);
        playerAiming.Initalise(transform, controller);
        playerShooting.Initalise(playerAiming);

    }

    void Awake()
    {

        if (playerIndex == PlayerIndex.One)
        {
            isP1 = true;


            //Spawn in objArrow
            ob = GameObject.FindGameObjectWithTag("Objective").GetComponent<Objective>();
            obArrow = GameObject.Find("Arrow").transform;
            obArrow.transform.parent = transform;
            obArrow.localPosition = new Vector3(0, 5.0f, 0);
        }
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


        //If P1, Calculate position of objective arrow.
            if (isP1 && !ob.complete)
            {
                Vector3 directionVector = transform.position - ob.transform.position;
                directionVector.Normalize();
                float headingAngle = Mathf.Atan2(directionVector.z, directionVector.z);
            //obArrow.LookAt(ob.transform, Vector3.up);
            obArrow.rotation = Quaternion.AngleAxis(Time.time * 2.0f, Vector3.up);
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
