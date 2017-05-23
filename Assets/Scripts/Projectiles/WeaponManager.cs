using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : ProjectileManager {

    public enum WeaponAimingMode
    {
        Angle,
        Vector,
        Transform
    }

    public Transform weaponFirePoint;
    public AudioSource audioSource;

    public Weapon _currentWeapon
    {
        get { return currentWeapon; }
        set { SetWeapon(value); }
    }
    [SerializeField]
    private Weapon currentWeapon;

    private WeaponAimingMode currentWeaponAimingMode = WeaponAimingMode.Angle;
    [SerializeField]
    private float currentAngleTarget;
    private Vector3 currentVectorTarget;
    private Transform currentTransformTarget;

    public void Start()
    {
        
        SetWeapon(currentWeapon);
    }

    public void Update()
    {
        currentAngleTarget = GetFiringAngle();
        UpdateWeapon();
    }

    public void LateUpdate()
    {
        currentWeapon.LateUpdate();
    }

    private void UpdateWeapon()
    {
        offset = weaponFirePoint.transform.position;
        Vector3 direction = Quaternion.AngleAxis(currentAngleTarget, Vector3.up) * Vector3.forward;
        currentWeapon.UpdateWeapon(direction);
    }

    public void AimWeaponAt(float angle)
    {
        currentWeaponAimingMode = WeaponAimingMode.Angle;
        currentAngleTarget = angle;
    }

    public void AimWeaponAt(Vector3 vector)
    {
        currentWeaponAimingMode = WeaponAimingMode.Vector;
        currentVectorTarget = vector;
    }

    public void AimWeaponAt(Transform t)
    {
        currentWeaponAimingMode = WeaponAimingMode.Transform;
        currentTransformTarget = t;
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

    public void FireWeapon()
    {
        offset = weaponFirePoint.transform.position;
        Vector3 direction = Quaternion.AngleAxis(GetFiringAngle(), Vector3.up) * Vector3.forward;
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

    public void GiveAmmoPack()
    {
        currentWeapon.OnAmmoPack();
    }

    public Vector3 GetFiringPoint()
    {
        offset = weaponFirePoint.transform.position;
        return offset;
    }

    private float GetFiringAngle()
    {
        if(currentWeaponAimingMode == WeaponAimingMode.Angle)
        {
            return currentAngleTarget;
        }else if(currentWeaponAimingMode == WeaponAimingMode.Vector)
        {
            Vector3 targetDirection = currentVectorTarget - offset;
            return Angle360(Vector3.forward, targetDirection, Vector3.right);
        }else
        {
            Vector3 targetDirection = currentTransformTarget.position - offset;
            return Angle360(Vector3.forward, targetDirection, Vector3.right);
        }
    }

    public string GetWeaponName()
    {
        return currentWeapon.weaponName;
    }

    float Angle360(Vector3 from, Vector3 to, Vector3 right)
    {
        float angle = Vector3.Angle(from, to);
        return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
    }

}
