using UnityEngine;
using System.Collections;

public class ProjectileManager : MonoBehaviour {

    public Vector3 offset;

    public void FireProjectile(Projectile p, Vector3 direction, Vector2 posSpread)
    {
        Vector3 fireDirection = direction + Quaternion.FromToRotation(Vector3.forward, direction) * GetSpread(posSpread);
        Vector3 fireOrigin = offset;
        Projectile projectile = p.GetInstanceFromPool();
        projectile.Fire(fireOrigin, fireDirection, p.projectileHitLayers, p.projectileUpdateType, this);
    }

    protected virtual Vector3 GetSpread(Vector2 pS)
    {
        Vector3 circle = new Vector3((Random.insideUnitCircle * pS.x).x, (Random.insideUnitCircle * pS.y).y, 0);
        return circle;
    }
	
}
