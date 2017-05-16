using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitMovement{

    public bool useRigidbody = false;

    protected Transform t;
    protected Rigidbody rb;

    public void Initalise(Transform transform)
    {
        t = transform;
        rb = transform.GetComponent<Rigidbody>();
    }

    public void MoveAlongDirection(Vector3 vector, float amount, float isoOffset = 0)
    {
        if (useRigidbody)
        {
            rb.MovePosition(rb.position + Quaternion.AngleAxis(isoOffset, Vector3.up) * vector.normalized * amount);
        }
        else
        {
            t.position += Quaternion.AngleAxis(isoOffset, Vector3.up) * vector.normalized * amount;
        }
    }

    public void MoveAlongVector(Vector3 vector, float isoOffset = 0)
    {
        if (useRigidbody)
        {
            rb.MovePosition(rb.position + Quaternion.AngleAxis(isoOffset, Vector3.up) * vector);
        }
        else
        {
            t.position += Quaternion.AngleAxis(isoOffset, Vector3.up) * vector;
        }
    }

}
