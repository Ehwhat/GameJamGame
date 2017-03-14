using UnityEngine;
using System.Collections;

public class AmmoPickUp : pickUp {

    protected override void OnPickup(GameObject pickupPlayer)
    {
        Debug.Log("Ammo Awarded");
        pickupPlayer.GetComponent<Ammo>().IncreaseAmmo(20); //Passing in value to IncreaseHealth function on trigger.
    }
}

