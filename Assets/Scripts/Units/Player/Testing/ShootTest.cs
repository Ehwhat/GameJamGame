using UnityEngine;
using System.Collections;

public class ShootTest : MonoBehaviour {

    public float shootAngle = 0;
    public WeaponManager weapon;

	// Update is called once per frame
	void Update () {
	    if(weapon != null)
        {
            shootAngle += Time.deltaTime*5;
            weapon.AimWeaponAt(shootAngle);
            weapon.FireWeapon();
        }
	}
}
