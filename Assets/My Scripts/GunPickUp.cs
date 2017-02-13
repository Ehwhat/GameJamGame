using UnityEngine;
using System.Collections;

public class GunPickUp : pickUp
{
    public WeaponTypes gun;
    protected override void OnPickup(GameObject pickupPlayer)
    {
        Debug.Log("Weapon added");
        pickupPlayer.GetComponent<PlayerWeaponHolder>().currentWeapon = gun; //Changes player curret weapon to whatever weapon the pickup contains
    }

}
