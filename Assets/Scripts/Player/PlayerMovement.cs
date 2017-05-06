using UnityEngine;
using System.Collections;
[System.Serializable]
public class PlayerMovement : UnitMovement {

    public float currentSpeed = 0.0f;
    
    public float movementWalkSpeed = 7;
    public float movementRunSpeed = 10;

    public float movementDeadZone = 0.10f;
    public float movementWalkZone = 0.65f;

    private PlayerControlManager.PlayerController controller;
    private PlayerManager player;

    private Vector2 movementVector = new Vector2();
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
        movementVector = Vector2.Lerp(movementVector,controller.GetStickVector(XboxControlStick.LeftStick), Time.deltaTime * 10);
        GetSpeed(movementVector.magnitude);
        return new Vector3(movementVector.x, 0, movementVector.y) * currentSpeed * Time.deltaTime;
    }

    public void OnKill(HitData lastHit)
    {
        if (useRigidbody)
        {
            Vector3 hitPoint = lastHit.rayHit.point;
            Vector3 forceHit = (rb.position - lastHit.rayHit.point).normalized * -100;
            Debug.Log("ShotHit" + forceHit);
            //Disable Constraints
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForceAtPosition(forceHit, hitPoint);
        }
        else
        {

        }
    }

    public void OnResurrect()
    {
        if (useRigidbody)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            t.localEulerAngles = new Vector3(0,0,0);
        }
         
    }

    void GetSpeed(float mag)
    {
        if(mag <= movementDeadZone)
        {
            currentSpeed =  Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2);
        }
        else if (mag <= movementWalkZone)
        {
           currentSpeed = Mathf.Lerp(currentSpeed, movementWalkSpeed, Time.deltaTime * 4);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, movementRunSpeed, Time.deltaTime * 4);
        }
    }

    public void DisableRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    public void FreezePlayer(bool freeze)
    {
        
        rb.constraints = (freeze) ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
    }
    
}
