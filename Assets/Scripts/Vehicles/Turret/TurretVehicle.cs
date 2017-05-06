using UnityEngine;
using System.Collections;

public class TurretVehicle : MonoBehaviour {

    public EntryPoint _userEntryPoint;
    public WeaponManager _weapon;
    public float _isoOffset = 45;

    private float aimAngle;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        PlayerControlManager.PlayerController controller = _userEntryPoint.GetPlayerController();
        if(controller != null)
        {
            Vector2 lookDirection = controller.GetStickVector(XboxControlStick.RightStick);
            AimTurret(lookDirection);
            _weapon.offset = _weapon.weaponHolder.transform.position;
            if (controller.GetTrigger(XboxTrigger.RightTrigger) > 0.1f)
            {
                _weapon.FireWeapon();
            }
        }
	}

    void AimTurret(Vector2 direction)
    {
        if (direction.magnitude > 0.3f)
        {
            aimAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, -direction.x) - _isoOffset;
            _weapon.currentFiringAngle = aimAngle;
            transform.rotation = Quaternion.AngleAxis(_isoOffset, Vector3.up) * Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
        }
    }
}
