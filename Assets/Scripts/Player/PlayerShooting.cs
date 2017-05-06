﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerShooting  {

    public WeaponManager weaponManager;
    public LineRenderer aimRect;

    public float aimLength = 4;

    private PlayerAiming aiming;

	public void Initalise(PlayerAiming aim)
    {
        aiming = aim;
    }

    public void Shoot()
    {
        if(weaponManager != null)
        {
            weaponManager.offset = weaponManager.weaponHolder.transform.position;
            weaponManager.currentFiringAngle = aiming.aimingAngle;
            weaponManager.FireWeapon();
        }
    }

    public void SetWeapon(Weapon weapon)
    {

        if (weaponManager != null)
        {
            weaponManager.SetWeapon(weapon);
        }
    }

    public float GetAmmoClipPercent()
    {
        return weaponManager.GetAmmoClipPercent();
    }

    public float GetReloadPercent()
    {
        return weaponManager.GetReloadPercent();
    }

    public void SetupAim()
    {
        weaponManager.offset = weaponManager.weaponHolder.transform.position;
        aimRect.SetPosition(0, weaponManager.GetFiringPoint());
        aimRect.SetPosition(1, weaponManager.GetFiringPoint() + (Quaternion.AngleAxis(aiming.aimingAngle, Vector3.up) * Vector3.forward)*aimLength);
    }

}
