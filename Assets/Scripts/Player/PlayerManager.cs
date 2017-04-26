using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerManager : ControlledUnitManager {

    HitData lastHit;
    int numRevivers = 0;
    float reviveTime = 0;
    float endReviveTime = 5;

    public PlayerMovement playerMovement = new PlayerMovement();
    public PlayerAiming playerAiming = new PlayerAiming();
    public PlayerShooting playerShooting = new PlayerShooting();

    //Additions for demo
    public bool isP1 = false;
    Objective ob;
    Transform obArrow;
    public Transform healthBar;
    public Transform energyBar;
    Image healbarImage;
    Image energyBarImage;

    // Use this for initialization
    public void Start ()
    {
        GetPlayerController(playerIndex);
		playerMovement.Initalise(this, controller);
        playerAiming.Initalise(transform, controller);
        playerShooting.Initalise(playerAiming);
       // healbarImage = healthBar.GetComponent<Image>();
        //energyBarImage = energyBar.GetComponent<Image>();
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

            healbarImage = GameObject.Find("P1ManaBackDrop").transform.FindChild("HealthBar").GetComponent<Image>();
            energyBarImage = GameObject.Find("P1ManaBackDrop").transform.FindChild("ManaBar").GetComponent<Image>();

            //Set Red mat.
            //transform.FindChild("Mesh/Capsule").GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            switch (playerIndex)
            {
                case PlayerIndex.Two:
                    healbarImage = GameObject.Find("P2ManaBackDrop").transform.FindChild("HealthBar").GetComponent<Image>();
                    energyBarImage = GameObject.Find("P2ManaBackDrop").transform.FindChild("ManaBar").GetComponent<Image>();
                   // transform.FindChild("Mesh/Capsule").GetComponent<MeshRenderer>().material.color = Color.blue;
                    break;
                case PlayerIndex.Three:
                    healbarImage = GameObject.Find("P3ManaBackDrop").transform.FindChild("HealthBar").GetComponent<Image>();
                    energyBarImage = GameObject.Find("P3ManaBackDrop").transform.FindChild("ManaBar").GetComponent<Image>();
                    break;
                case PlayerIndex.Four:
                    healbarImage = GameObject.Find("P4ManaBackDrop").transform.FindChild("HealthBar").GetComponent<Image>();
                    energyBarImage = GameObject.Find("P4ManaBackDrop").transform.FindChild("ManaBar").GetComponent<Image>();
                    break;
            }
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
        healbarImage.fillAmount = currentHealth / (maxHealth * 4);
        energyBarImage.fillAmount = reviveTime / (endReviveTime * 4);

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
