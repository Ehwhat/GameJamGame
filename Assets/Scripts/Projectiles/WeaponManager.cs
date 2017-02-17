using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : ProjectileManager {

    public enum AmmoTypes
    {
        Bullet,
        Missiles,
        Batteries
    }

    public enum AmmoSystem
    {
        Clip,
        Clipless,
        Unlimited
    }

    public Weapon currentWeapon;

    public float currentFiringAngle;

    private Dictionary<AmmoTypes, int> ammoCountDictonary = new Dictionary<AmmoTypes, int>();
    private bool isReloading = false;
    private float lastFire = 0;

    public void Awake()
    {
        foreach (AmmoTypes type in Enum.GetValues(typeof(AmmoTypes)))
        {
            ammoCountDictonary.Add(type, 1000);
        }
    }

    public void Start()
    {
        SetWeapon(currentWeapon);
        lastFire = -(1/currentWeapon.fireRatePerSecond);
    }


    void Update()
    {
        if (currentWeapon.ammoSystem == AmmoSystem.Clipless)
        {
            currentWeapon.currentAmmoInClip = ammoCountDictonary[currentWeapon.ammoType];
        }
        else if (currentWeapon.ammoSystem == AmmoSystem.Unlimited)
        {
            currentWeapon.currentAmmoInClip = int.MaxValue;
        }

    }

    public void SetWeapon(Weapon w)
    {
        currentWeapon = w;
        currentWeapon.currentAmmoInClip = currentWeapon.ammoPerClip;
    }

    public void FireWeapon()
    {
        if (Time.time - lastFire > 1 / currentWeapon.fireRatePerSecond)
        {
            if (currentWeapon.tryFire(currentWeapon.bulletsPerShot))
            {
                lastFire = Time.time;
                for (int i = 0; i < currentWeapon.bulletsPerShot; i++)
                {
                    Vector3 direction = Quaternion.AngleAxis(currentFiringAngle, Vector3.up) * Vector3.forward;
                    FireProjectile(currentWeapon.weaponProjectile, direction, currentWeapon.possibleSpread + currentWeapon.weaponProjectile.possibleSpreadModifier);
                    if (currentWeapon.ammoSystem == AmmoSystem.Clipless)
                    {
                        ammoCountDictonary[currentWeapon.ammoType] -= 1;
                    }
                }
            }
            else if (!isReloading)
            {
                ReloadWeapon();
            }
        }
    }

    public void ReloadWeapon()
    {
        if (currentWeapon.ammoSystem == AmmoSystem.Clip) {
            isReloading = true;
            StartCoroutine(ReloadWeaponDelay(currentWeapon.reloadTime));
        }
    }

    IEnumerator ReloadWeaponDelay(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
        if (ammoCountDictonary[currentWeapon.ammoType] >= currentWeapon.ammoPerClip)
        {
            currentWeapon.currentAmmoInClip = currentWeapon.ammoPerClip;
            ammoCountDictonary[currentWeapon.ammoType] -= currentWeapon.ammoPerClip;
            currentWeapon.needsReloading = false;
            isReloading = false;
        }else if(ammoCountDictonary[currentWeapon.ammoType] > 0){
            currentWeapon.currentAmmoInClip = ammoCountDictonary[currentWeapon.ammoType];
            ammoCountDictonary[currentWeapon.ammoType] = 0;
            currentWeapon.needsReloading = false;
            isReloading = false;
        }

    }
    

}
