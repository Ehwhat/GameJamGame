using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : ProjectileManager {

    public enum AmmoSystem
    {
        Clip,
        Clipless,
        Unlimited
    }

    public Transform weaponHolder;
    public Weapon currentWeapon;

    public float currentFiringAngle;

    public void Start()
    {
        SetWeapon(currentWeapon);
    }


    void Update()
    {

    }

    public void SetWeapon(Weapon w)
    {
        if(currentWeapon != null && currentWeapon.hideFlags != HideFlags.HideInHierarchy)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate<Weapon>(w);
        currentWeapon.transform.parent = weaponHolder;
        currentWeapon.Initalise();
    }

    public void FireWeapon()
    {
        Vector3 direction = Quaternion.AngleAxis(currentFiringAngle, Vector3.up) * Vector3.forward;
        currentWeapon.FireWeapon(this, direction);
    }

    public float GetAmmoClipPercent()
    {
        return currentWeapon.GetAmmoClipPercent();
    }

    public float GetReloadPercent()
    {
        return currentWeapon.GetReloadPercent();
    }

}
