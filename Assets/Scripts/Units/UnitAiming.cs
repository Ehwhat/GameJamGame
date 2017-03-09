using UnityEngine;
using System.Collections;

public abstract class UnitAiming {

    private Transform t;

    protected void Initalise(Transform transform)
    {
        t = transform;
    }

    public void LookInDirection(Vector2 direction, float isoOffset = 0)
    {
        if (direction.magnitude > 0)
        {
            direction.Normalize();
            t.rotation = Quaternion.AngleAxis(isoOffset, Vector3.up) * Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
        }
    }
}
