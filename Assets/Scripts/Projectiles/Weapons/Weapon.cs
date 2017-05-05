using UnityEngine;
using System.Collections;

/// <summary>
///  The Weapon class is the controller for shooting Projectiles, we can make these into prefabs that are fed to WeaponManagers
/// 
/// </summary>

public class Weapon : MonoBehaviour {

    public enum WeaponState
    {
        Idle,
        Firing,
        Reloading,
        Empty
    }

    [Header("Projectile Stuff")]
    [Tooltip("Weapon firing Point")]
    public Transform weaponFirePoint;
    [Tooltip("Projectile that the weapon fires")]
    public Projectile weaponProjectile;
    [Tooltip("The range of spread that the weapon fires in ( Y values are usually to be kept at 0)")]
    public Vector2 possibleSpread;
    [Tooltip("Shots fired per second")]
    public float fireRatePerSecond = 1;
    [Tooltip("Bullets fired per shot")]
    public int bulletsPerShot = 1;
    [Tooltip("Time to reload")]
    public float reloadTime;
    [Space(10)]

    [Header("Ammo Stuff")]
    [Tooltip("Ammo system that the weapon uses \n Unlimited is what it sounds like \n Clip means it will use the clip system, so it'll reload after each clip is exausted ")]
    public WeaponManager.AmmoSystem ammoSystem;

    [Tooltip("The maximum ammo the weapon can hold in storage")]
    public int maxStoredAmmo = 32;
    [Tooltip("Current stored ammo in storage")]
    public int storedAmmo = 16;
    [Tooltip("Amount of ammo each ammo pack gives")]
    public int ammoPackAmount = 12;
    [Tooltip("Amount of ammo to each clip")]
    public int ammoPerClip = 10;

    private WeaponState weaponState = WeaponState.Idle;
    private float currentReloadPercent = 0;
    /// <summary>
    /// This get-set variable works out exactly what should be in the clip, if the weapon is of a clip type and the ammo goes under
    /// or equal to 0, it'll reload, otherwise it'll just set as normal.
    /// </summary>
    public int currentAmmoInClip
    {
        get {   if (ammoSystem == WeaponManager.AmmoSystem.Unlimited) {
                    return int.MaxValue;
                }
                else if(ammoSystem == WeaponManager.AmmoSystem.Clipless)
                {
                return storedAmmo;
                }
                return m_currentAmmoInClip;
            }
        set {   if (m_currentAmmoInClip + value <= 1 && ammoSystem == WeaponManager.AmmoSystem.Clip && weaponState != WeaponState.Reloading) {
                    m_currentAmmoInClip = 0;
                    Reload();
                }else if(ammoSystem == WeaponManager.AmmoSystem.Clipless)
                {
                    storedAmmo = value;
                }else if(ammoSystem == WeaponManager.AmmoSystem.Unlimited)
                {
                    //I do not need your puny "ammo"
                }
                else {
                    m_currentAmmoInClip = value;
                }
            }
    }

    //the private membr to the get-set
    private int m_currentAmmoInClip;
    private float lastFire = 0;

    public void Initalise()
    {
        lastFire = -(1 / fireRatePerSecond);
        FillClip();
    }

    // Hacky solution to getting WeaponManager to reload the weapon, I do want to change this but atm it works so...
    public void Reload()
    {
        weaponState = WeaponState.Reloading;
        StartCoroutine(ReloadWeaponDelay(reloadTime));
    }

    //checks the clip 
    public bool FireWeapon(ProjectileManager owningManager, Vector3 direction)
    {
        if (Time.time - lastFire > 1 / fireRatePerSecond)
        {
            if (weaponState != WeaponState.Reloading && weaponState != WeaponState.Empty)
            {
                lastFire = Time.time;
                for (int i = 0; i < bulletsPerShot; i++)
                {
                    owningManager.FireProjectile(weaponProjectile, direction, possibleSpread + weaponProjectile.possibleSpreadModifier);
                }
                currentAmmoInClip--;
                return true;
            }
        }
        return false;
    }

    public void OnAmmoPack()
    {
        AddAmmoToStorage(ammoPackAmount);
    }

    public void FillAmmoStorage()
    {
        storedAmmo = maxStoredAmmo;
    }

    private void AddAmmoToStorage(int amount)
    {
        storedAmmo = Mathf.Clamp(storedAmmo + amount, 0, maxStoredAmmo);
    }

    IEnumerator ReloadWeaponDelay(float reloadTime)
    {
        float startTime = Time.time;
        while(Time.time - startTime < reloadTime)
        {
            currentReloadPercent = 1-((Time.time - startTime)/reloadTime);
            yield return new WaitForEndOfFrame();
        }
        currentReloadPercent = 0;
        weaponState = WeaponState.Idle;
        FillClip();
        if(currentAmmoInClip <= 0)
        {
            weaponState = WeaponState.Empty;
        }
    }

    void FillClip()
    {
        if (storedAmmo >= ammoPerClip)
        {
            currentAmmoInClip = ammoPerClip;
            storedAmmo -= ammoPerClip;
        }
        else
        {
            currentAmmoInClip = storedAmmo;
            storedAmmo = 0;
        }
    }

    public float GetAmmoClipPercent()
    {
        return (Mathf.Clamp01(((float)currentAmmoInClip / (float)ammoPerClip)));
    }

    public float GetReloadPercent()
    {
        return currentReloadPercent;
    }

}
