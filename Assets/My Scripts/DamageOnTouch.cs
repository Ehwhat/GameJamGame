using UnityEngine;
using System.Collections;

public class DamageOnTouch : pickUp 
{
    protected override void OnPickup(GameObject pickupPlayer)
    {
        Debug.Log("Health Lost");
        pickupPlayer.GetComponent<PlayerWeaponHolder>().DecreaseHealth(20); //Passing in value to IncreaseHealth function on trigger.
    }
}