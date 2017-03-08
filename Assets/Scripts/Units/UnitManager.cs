using UnityEngine;
using System.Collections;
using System;
using FSM;

[System.Serializable]
public abstract class UnitManager : DamageableObject
{
    
    public void MoveAlongVector(Vector3 vector, float amount)
    {
        transform.position += vector.normalized * amount;
    }

}
