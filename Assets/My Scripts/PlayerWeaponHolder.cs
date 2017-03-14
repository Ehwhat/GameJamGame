using UnityEngine;
using System.Collections;

public enum WeaponTypes
{
    Pistol,
    Shotgun,
    LMG,
    Duck,
    RocketLauncher
}

public class PlayerWeaponHolder : MonoBehaviour {

    public WeaponTypes currentWeapon;
    public PlayerHealth healthbar;

    private float maxHealth = 100;
    public float health = 100;

    public void IncreaseHealth(float amount)
    {
        health += amount;
        if(health>maxHealth) //Cheap code to keep health bar from overextending
        {
            health = 100;
        }
    }

    public void DecreaseHealth(float amount)
    {
        health -= amount;
    }

    void Update()
    {
        //Drain Health over time
        //health -= 2 * Time.deltaTime; 
        healthbar.setHealthBar(health/100);
    }

}




