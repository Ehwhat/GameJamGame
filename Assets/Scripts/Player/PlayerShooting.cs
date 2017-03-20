using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerShooting  {

    public Transform firePoint;
    public WeaponManager weaponManager;

    private PlayerAiming aiming;

	public void Initalise(PlayerAiming aim)
    {
        aiming = aim;
    }

    public void Shoot()
    {
        if(weaponManager != null)
        {
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

}
