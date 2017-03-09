using UnityEngine;
using System.Collections;

public class PlayerManager : ControlledUnitManager {

    public PlayerMovement playerMovement = new PlayerMovement();
    public PlayerAiming playerAiming = new PlayerAiming();
    public PlayerShooting playerShooting = new PlayerShooting();

	// Use this for initialization
	void Start () {
        GetPlayerController(playerIndex);
        playerMovement.Initalise(transform, controller);
        playerAiming.Initalise(transform, controller);
        playerShooting.Initalise(playerAiming);
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.HandleMovement();
        playerAiming.HandleRotation();
        if(controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
        {
            Debug.Log("Shooting");
            playerShooting.Shoot();
        }
    }

}
