using UnityEngine;
using System.Collections;
using System;

public class TestTarget : MonoBehaviour, IHitTarget {
    public void OnDamageHit(HitData hit)
    {
        Debug.Log("TARGET");
        GetComponent<Rigidbody>().AddForce(hit.knockback);
    }

}
