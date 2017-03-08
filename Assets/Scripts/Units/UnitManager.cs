using UnityEngine;
using System.Collections;
using System;
using FSM;

[System.Serializable]
public abstract class UnitManager : DamageableObject
{
    
    public void MoveAlongDirection(Vector3 vector, float amount, float isoOffset = 0)
    {
        transform.position += Quaternion.AngleAxis(isoOffset, Vector3.up) * vector.normalized * amount;
    }

    public void MoveAlongVector(Vector3 vector, float isoOffset = 0)
    {
        transform.position += Quaternion.AngleAxis(isoOffset, Vector3.up) * vector;
    }

    public void LookInDirection(Vector2 direction, float isoOffset = 0)
    {
        if (direction.magnitude > 0)
        {
            direction.Normalize();
            transform.rotation = Quaternion.AngleAxis(isoOffset, Vector3.up) * Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
        }
    }

}
