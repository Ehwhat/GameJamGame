using UnityEngine;
using System.Collections;

[System.Serializable]
public class Weapon {

    public Projectile weaponProjectile;
    public Vector2 possibleSpread;
    public float fireRatePerSecond;

    public WeaponManager.AmmoSystem ammoSystem;
    public WeaponManager.AmmoTypes ammoType;
    public int ammoPerClip = 10;
    public int currentAmmoInClip
    {
        get { return m_currentAmmoInClip; }
        set { if (m_currentAmmoInClip + value <= 0 && ammoSystem == WeaponManager.AmmoSystem.Clip) { Reload(); } else { m_currentAmmoInClip = value; } }
    }
    public float reloadTime;

    public bool needsReloading;

    private int m_currentAmmoInClip;

    public Weapon(Projectile weaponProjectile)
    {

    }

    public void Reload()
    {
        needsReloading = true;
    }

    public bool tryFire()
    {
        return !needsReloading;
    }

}
