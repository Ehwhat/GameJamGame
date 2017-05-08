using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerAiming : UnitAiming {

    public float lookDeadZone = 0.3f;
    public GameObject playerTorso;
    public GameObject playerMesh;

    public bool isAiming { private set; get; }
    public float aimingAngle { private set; get; }


    private PlayerControlManager.PlayerController controller;

    public void Initalise(Transform transform, PlayerControlManager.PlayerController c)
    {
        controller = c;
        base.Initalise(transform);
    }

    public void HandleRotation()
    {
        Vector2 lookDirection = GetLookDirection(45);
        playerTorso.transform.localRotation = Quaternion.Euler(playerMesh.transform.rotation.eulerAngles.y - aimingAngle + 180, 0,0);
        //LookInDirection(lookDirection,45);
    }

    Vector2 GetLookDirection(float isoOffset = 0)
    {
        Vector2 lookDirection = controller.GetStickVector(XboxControlStick.RightStick);
        
        if (lookDirection.magnitude > lookDeadZone)
        {
            aimingAngle = Mathf.Rad2Deg * Mathf.Atan2(lookDirection.y, -lookDirection.x) - isoOffset;
            isAiming = true;
            return lookDirection;
        }
        isAiming = false;
        return Vector2.zero;
    }

}
