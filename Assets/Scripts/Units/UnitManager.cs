using UnityEngine;
using System.Collections;

public abstract class UnitManager : MonoBehaviour {

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

    public void aimTowards(Vector3 position)
    {
        Vector3 diff = (position - transform.position).normalized;
        aimDirection = new Vector2(diff.x, diff.z);
    }

    public void aimTowardsLocal(Vector3 localPosition)
    {
        aimDirection = new Vector2(localPosition.x, localPosition.z);
    }

}
