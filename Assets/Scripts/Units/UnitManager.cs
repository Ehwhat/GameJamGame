using UnityEngine;
using System.Collections;
using System;

public abstract class UnitManager : MonoBehaviour, IHitTarget {

    public float health;
    public float maxHealth;
    public float movementSpeed = 5;

    public float isoAngleOffset = 45;

    protected Vector2 aimDirection = new Vector2(0,0);

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

    public void AimTowards(Vector3 position)
    {
        Vector3 diff = (position - transform.position).normalized;
        aimDirection = new Vector2(diff.x, diff.z);
    }

    public void AimTowardsLocal(Vector3 localPosition)
    {
        aimDirection = new Vector2(localPosition.x, localPosition.z);
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
