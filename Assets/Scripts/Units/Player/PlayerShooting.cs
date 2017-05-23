using UnityEngine;
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
            weaponManager.AimWeaponAt(aiming.aimingAngle);
            weaponManager.FireWeapon();
        }
    }

    public void HandleWeapon()
    {
        SetupAim();
    }

    public void SetWeapon(Weapon weapon)
    {

        if (weaponManager != null)
        {
            weaponManager.SetWeapon(weapon);
        }
    }

    public void GiveAmmoPack()
    {
        weaponManager.GiveAmmoPack();
    }

    public float GetAmmoClipPercent()
    {
        return weaponManager.GetAmmoClipPercent();
    }

    public float GetReloadPercent()
    {
        return weaponManager.GetReloadPercent();
    }

    public string GetWeaponName()
    {
        return weaponManager.GetWeaponName();
    }

    public void SetupAim()
    {
        aimRect.SetPosition(0, weaponManager.GetFiringPoint());
        aimRect.SetPosition(1, weaponManager.GetFiringPoint() + (Quaternion.AngleAxis(aiming.aimingAngle, Vector3.up) * Vector3.forward)*aimLength);
    }

}
