using UnityEngine;
using System.Collections;
<<<<<<< HEAD
using System;

public abstract class UnitManager : MonoBehaviour, IHitTarget {
=======

public abstract class UnitManager : MonoBehaviour {
>>>>>>> TomsBranch

    public float health;
    public float maxHealth;
    public float movementSpeed = 5;

    public float isoAngleOffset = 45;

<<<<<<< HEAD
    protected Vector3 aimDirection = new Vector3(0,0,0);
=======
    protected Vector2 aimDirection = new Vector2(0,0);
>>>>>>> TomsBranch

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

<<<<<<< HEAD
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

=======
    public void aimTowards(Vector3 position)
    {
        Vector3 diff = (position - transform.position).normalized;
        aimDirection = new Vector2(diff.x, diff.z);
    }

    public void aimTowardsLocal(Vector3 localPosition)
    {
        aimDirection = new Vector2(localPosition.x, localPosition.z);
    }

>>>>>>> TomsBranch
}
