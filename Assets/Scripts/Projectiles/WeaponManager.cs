using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : ProjectileManager {

    public Transform weaponFirePoint;
    public AudioSource audioSource;

    public Weapon _currentWeapon
    {
        get { return currentWeapon; }
        set { SetWeapon(value); }
    }
    [SerializeField]
    private Weapon currentWeapon;

    public float currentFiringAngle;

    public void Start()
    {
        SetWeapon(currentWeapon);
    }

    public void LateUpdate()
    {
        currentWeapon.LateUpdate();
    }

    public void UpdateWeapon(float firingAngle)
    {
        offset = weaponFirePoint.transform.position;
        currentFiringAngle = firingAngle;
        Vector3 direction = Quaternion.AngleAxis(currentFiringAngle, Vector3.up) * Vector3.forward;
        currentWeapon.UpdateWeapon(direction);
    }

    public void SetWeapon(Weapon w)
    {
        if(currentWeapon != null && currentWeapon.hideFlags != HideFlags.HideInHierarchy)
        {
            Destroy(currentWeapon.gameObject);
        }
        currentWeapon = Instantiate<Weapon>(w);
        currentWeapon.transform.parent = transform;
        currentWeapon.Initalise(audioSource);
    }

    public void FireWeapon(float firingAngle)
    {
        offset = weaponFirePoint.transform.position;
        currentFiringAngle = firingAngle;
        Vector3 direction = Quaternion.AngleAxis(currentFiringAngle, Vector3.up) * Vector3.forward;
        currentWeapon.UpdateWeapon(direction);
        currentWeapon.FireWeapon(this);
    }

    public float GetAmmoClipPercent()
    {
        return currentWeapon.GetAmmoClipPercent();
    }

    public float GetReloadPercent()
    {
        return currentWeapon.GetReloadPercent();
    }

    public Vector3 GetFiringPoint()
    {
        offset = weaponFirePoint.transform.position;
        return offset;
    }

}
