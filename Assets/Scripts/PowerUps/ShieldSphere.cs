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
        Projectile proj = data.hitProjectile;
        proj.Fire(
            data.rayHit.point,
            data.rayHit.normal,
            proj.projectileHitLayers,
            proj.projectileUpdateType,
            proj.owner
            );

    }

}
