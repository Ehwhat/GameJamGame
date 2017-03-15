using UnityEngine;
using System.Collections;
[System.Serializable]
public class PlayerMovement : UnitMovement {

    public float movementWalkSpeed = 30;
    public float movementRunSpeed = 45;

    public float movementDeadZone = 0.1f;
    public float movementWalkZone = 0.65f;

    private PlayerControlManager.PlayerController controller;
    private PlayerManager player;

    // Use this for initialization

    public void Initalise(PlayerManager play, PlayerControlManager.PlayerController c)
    {
        controller = c;
        player = play;
        base.Initalise(play.transform);
    }

    public void HandleMovement()
    {
        Vector3 movementVector = GetMovementVector();
        MoveAlongVector(movementVector,45);
    }

    Vector3 GetMovementVector()
    {
        Vector2 movementVector = controller.GetStickVector(XboxControlStick.LeftStick);
        return new Vector3(movementVector.x, 0, movementVector.y) * GetSpeed(movementVector.magnitude) * Time.deltaTime;
    }

    

    float GetSpeed(float mag)
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
    
}
