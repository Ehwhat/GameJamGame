using UnityEngine;
using System.Collections;

public class PlayerMovement : ControlledUnitManager {

    public float movementWalkSpeed = 30;
    public float movementRunSpeed = 45;

    public float movementDeadZone = 0.1f;
    public float movementWalkZone = 0.65f;
    public float lookDeadZone = 0.3f;

    // Use this for initialization
    void Start () {
        GetPlayerController(playerIndex);
    }
	
	// Update is called once per frame
	void Update () {
        HandleRotation();
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 movementVector = getMovementVector();
        MoveAlongVector(movementVector,45);
    }

    void HandleRotation()
    {
        Vector2 lookDirection = getLookDirection();
        LookInDirection(lookDirection,45);
    }

    Vector3 getMovementVector()
    {
        Vector2 movementVector = controller.GetStickVector(XboxControlStick.LeftStick);
        return new Vector3(movementVector.x, 0, movementVector.y) * getSpeed(movementVector.magnitude) * Time.deltaTime;
    }

    Vector2 getLookDirection()
    {
        Vector2 lookDirection = controller.GetStickVector(XboxControlStick.RightStick);
        if(lookDirection.magnitude > lookDeadZone)
        {
            return lookDirection;
        }
        return Vector2.zero;
    }

    float getSpeed(float mag)
    {
        if(mag <= movementDeadZone)
        {
            return 0;
        }else if(mag <= movementWalkZone)
        {
            return movementWalkSpeed;
        }
        return movementRunSpeed;
    }

    Quaternion getIsoModifier(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
    
}
