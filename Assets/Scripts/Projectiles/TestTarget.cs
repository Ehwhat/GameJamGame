using UnityEngine;
using System.Collections;
using System;

public class TestTarget : MonoBehaviour, IHitTarget {
    public void OnDamageHit(HitData hit)
    {
        Debug.Log(hit);
        GetComponent<Rigidbody>().AddForce(hit.knockback);
    }

}
