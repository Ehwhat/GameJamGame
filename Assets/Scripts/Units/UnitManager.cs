using UnityEngine;
using System.Collections;
using System;

public abstract class UnitManager : MonoBehaviour, IHitTarget {

    public float health;
    public float maxHealth;
    public float movementSpeed = 5;

    public float isoAngleOffset = 45;

    protected Vector3 aimDirection = new Vector3(0,0,0);

    public enum TimeStepType
    {
        Fixed,
        Normal
    }

    public void MoveIn(Vector3 vector, TimeStepType timeType = TimeStepType.Fixed)
    {
        transform.position += Quaternion.AngleAxis(isoAngleOffset, Vector3.up) * vector.normalized * movementSpeed * ((timeType == TimeStepType.Fixed) ? Time.fixedDeltaTime : Time.deltaTime);
    }

    public void MoveIn(Vector3 vector, float timeStep)
    {
        transform.position += vector.normalized * movementSpeed * timeStep;
    }

    public void AimAtAngle(float angle)
    {
        aimDirection = Quaternion.AngleAxis(angle+isoAngleOffset, Vector3.up) * Vector3.forward;
        Debug.Log(aimDirection);
    }

    public virtual void OnDamageHit(HitData hit)
    {
        health -= hit.damage;
        OnHit(hit);
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath() { }
    public virtual void OnHit(HitData hit) { }

}
