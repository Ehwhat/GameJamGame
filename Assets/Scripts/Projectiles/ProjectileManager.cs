using UnityEngine;
using System.Collections;

public class ProjectileManager : MonoBehaviour {

    public Transform firePoint;

    public void FireProjectile(Projectile p, Vector3 direction, Vector2 posSpread)
    {
        Vector3 fireDirection = firePoint.transform.forward + GetSpread(posSpread);
        Projectile projectile = p.GetInstanceFromPool();
        projectile.Fire(firePoint.transform.position,fireDirection, p.projectileHitLayers, Projectile.ProjectileUpdateType.Update);
    }

    protected virtual Vector3 GetSpread(Vector2 pS)
    {
        Vector3 circle = new Vector3((Random.insideUnitCircle* pS.x).x, (Random.insideUnitCircle * pS.y).y,0);
        return circle;
    }

    void Update()
    {

    }
	
}
