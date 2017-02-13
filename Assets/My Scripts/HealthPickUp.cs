using UnityEngine;
using System.Collections;

public class HealthPickUp : pickUp
{
    protected override void OnPickup(GameObject pickupPlayer)
    {
        Debug.Log("Healt Awarded");
        pickupPlayer.GetComponent<PlayerWeaponHolder>().IncreaseHealth(20); //Passing in value to IncreaseHealth function on trigger.
    }

}

