using UnityEngine;
using System.Collections;

/// <summary>
///  The Weapon class is the controller for shooting Projectiles, we can make these into prefabs that are fed to WeaponManagers
/// 
/// </summary>

public class Weapon : MonoBehaviour {

    public enum WeaponState
    {
        Ready,
        Firing,
        Fired,
        Reloading,
        Empty
    }

    public enum WeaponFiringMode
    {
        Semi,
        Burst,
        AutomaticBursts,
        Automatic
    }

    public enum AmmoSystem
    {
        Clip,
        Clipless,
        Unlimited
    }

    [Tooltip("Weapon Name")]
    public string weaponName;
    public static string PROPERTY_WEAPON_NAME = "weaponName";

    [Tooltip("Projectile that the weapon fires")]
    public Projectile weaponProjectile;
    public static string PROPERTY_WEAPON_PROJECTILE = "weaponProjectile";

    [Tooltip("Time in seconds til reload is complete")]
    public float reloadTime;
    public static string PROPERTY_RELOAD_TIME = "reloadTime";

    [Tooltip("Weapon firing mode \n Semi means shot per fire, \n Burst means a burst per fire, \n Automatic means spray and pray ")]
    public WeaponFiringMode weaponFiringMode;
    public static string PROPERTY_WEAPON_FIRING_MODE = "weaponFiringMode";

    [Tooltip("The range of spread that the weapon fires in \n ( Y values are usually to be kept at 0)")]
    public Vector2 possibleSpread;
    public static string PROPERTY_POSSIBLE_SPREAD = "possibleSpread";

    [Tooltip("Shots fired per second \n In burst fire mode, this decides the firerate between bullets in a burst, not per burst \n Please note that this doesn't include shots per burst")]
    public float fireRatePerSecond = 1;
    public static string PROPERTY_FIRERATE_PER_SECOND = "fireRatePerSecond";

    [Tooltip("Bursts fired per second \n In BurstAutomatic mode, this decides how far apart each burst is")]
    public float burstsPerSecond = 1;
    public static string PROPERTY_BURSTS_PER_SECOND = "burstsPerSecond";

    [Tooltip("Shots fired per burst \n For example, an automatic rifle might fire 3 bursts of bullets in succession")]
    public int shotsPerBurst = 1;
    public static string PROPERTY_SHOTS_PER_BURST = "shotsPerBurst";

    [Tooltip("Bullets fired per shot \n For example, a shotgun might fire 6 bullets at once")]
    public int bulletsPerShot = 1;
    public static string PROPERTY_BULLETS_PER_SHOT = "bulletsPerShot";

    [Tooltip("Ammo system that the weapon uses \n Unlimited is what it sounds like \n Clip means it will use the clip system, so it'll reload after each clip is exausted ")]
    public AmmoSystem ammoSystem;
    public static string PROPERTY_AMMO_SYSTEM = "ammoSystem";

    [Tooltip("The maximum ammo the weapon can hold in storage")]
    public int maxStoredAmmo = 32;
    public static string PROPERTY_MAX_STORED_AMMO = "maxStoredAmmo";

    [Tooltip("Current stored ammo in storage")]
    public int storedAmmo = 16;
    public static string PROPERTY_CURRENT_STORED_AMMO = "storedAmmo";

    [Tooltip("Amount of ammo each ammo pack gives")]
    public int ammoPackAmount = 12;
    public static string PROPERTY_AMMO_PACK_AMOUNT = "ammoPackAmount";

    [Tooltip("Amount of ammo to each clip")]
    public int ammoPerClip = 10;
    public static string PROPERTY_AMMO_PER_CLIP = "ammoPerClip";

    [Tooltip("Audio clip for weapon shot")]
    public AudioClip weaponShotSound;
    public static string PROPERTY_SHOT_AUDIO_CLIP = "weaponShotSound";

    private WeaponState weaponState = WeaponState.Ready;
    private WeaponState lastWeaponState = WeaponState.Ready;
    private float currentReloadPercent = 0;
    /// <summary>
    /// This get-set variable works out exactly what should be in the clip, if the weapon is of a clip type and the ammo goes under
    /// or equal to 0, it'll reload, otherwise it'll just set as normal.
    /// </summary>
    public int currentAmmoInClip
    {
        get {   if (ammoSystem == AmmoSystem.Unlimited) {
                    return int.MaxValue;
                }
                else if(ammoSystem == AmmoSystem.Clipless)
                {
                return storedAmmo;
                }
                return m_currentAmmoInClip;
            }
        set {   if (m_currentAmmoInClip + value <= 1 && ammoSystem == AmmoSystem.Clip) {
                m_currentAmmoInClip = 0;
                Reload();
                }else if (ammoSystem == AmmoSystem.Clipless)
                {
                    storedAmmo = value;
                } else if (ammoSystem == AmmoSystem.Unlimited)
                {
                    //I do not need your puny "ammo"
                } else {
                    m_currentAmmoInClip = value;
                }
            }
    }

    //the private membr to the get-set
    private int m_currentAmmoInClip;
    private float lastFire = 0;
    private Vector3 fireDirection;
    private AudioSource audioSource;

    public void Initalise(AudioSource source)
    {
        audioSource = source;
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
    public bool FireWeapon(ProjectileManager owningManager)
    {
        if (lastWeaponState == WeaponState.Ready)
        {
            StartCoroutine(FireWeaponRoutine(owningManager));
            return true;
        }else if(lastWeaponState == WeaponState.Fired && weaponFiringMode != WeaponFiringMode.Automatic && weaponFiringMode != WeaponFiringMode.AutomaticBursts )
        {
            weaponState = WeaponState.Fired;
        }
        return false;
    }

    public void UpdateWeapon(Vector3 direction)
    {
        fireDirection = direction;
        if (weaponState == WeaponState.Fired)
        {
            weaponState = WeaponState.Ready;
        }
    }

    public void LateUpdate()
    {
        lastWeaponState = weaponState;
    }

    private IEnumerator FireWeaponRoutine(ProjectileManager owningManager)
    {
        lastFire = Time.time;
        weaponState = WeaponState.Firing;
        for (int shot = 0; shot < shotsPerBurst; shot++)
        {
            for (int bullet = 0; bullet < bulletsPerShot; bullet++)
            {
                owningManager.FireProjectile(weaponProjectile, fireDirection, possibleSpread + weaponProjectile.possibleSpreadModifier);
                PlayAudio(weaponShotSound);
            }
            currentAmmoInClip--;
            if(weaponState != WeaponState.Firing)
            {
                yield break;
            }
            yield return new WaitForSeconds(1 / fireRatePerSecond);
        }
        if(weaponFiringMode == WeaponFiringMode.AutomaticBursts)
        {
            yield return new WaitForSeconds(1 / burstsPerSecond);
        }
        weaponState = WeaponState.Fired;
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
        if(storedAmmo > 0 && weaponState == WeaponState.Empty)
        {
            Reload();
        }
    }

    IEnumerator ReloadWeaponDelay(float reloadTime)
    {
        if(storedAmmo < 1)
        {
            weaponState = WeaponState.Empty;
            currentReloadPercent = 0;
            yield break;
        }
        float startTime = Time.time;
        while(Time.time - startTime < reloadTime)
        {
            currentReloadPercent = 1-((Time.time - startTime)/reloadTime);
            yield return new WaitForEndOfFrame();
        }
        currentReloadPercent = 0;
        weaponState = WeaponState.Ready;
        FillClip();
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

    private void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
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
