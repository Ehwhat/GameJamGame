using UnityEngine;
using System.Collections;
using System;

public class ShieldSphere : MonoBehaviour, IHitTarget {
    public void OnDamageHit(HitData hit)
    {
        DeflectProjectile(hit);
    }
    
    void DeflectProjectile(HitData data)
    {
        Projectile proj = data.hitProjectile.GetInstanceFromPool();
        proj.transform.position = data.rayHit.point;
        Vector3 normal = new Vector3(data.rayHit.normal.x, 0, data.rayHit.normal.z);

        proj.Fire(
            data.rayHit.point,
            normal,
            proj.projectileHitLayers,
            proj.projectileUpdateType,
            proj.owner
            );

    }

}
