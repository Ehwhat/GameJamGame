using UnityEngine;
using System.Collections;

public class Projectile {

    public enum ProjectileUpdateType
    {
        None,
        Update,
        Fixed
    }

    public LayerMask projectileHitLayers;

    public Vector3 position;
    public Vector3 directionVector;

    public float speedPerSecond;
    public float speedModifier = 1;

    private float m_timeStep;
    public float timeStep
    {
        private set { m_timeStep = value; }
        get { return (projectileUpdateType == ProjectileUpdateType.None) ? m_timeStep : (projectileUpdateType == ProjectileUpdateType.Update) ? Time.deltaTime : Time.fixedDeltaTime;  }
    }

    private ProjectileUpdateType projectileUpdateType = ProjectileUpdateType.None;

	public Projectile(Vector3 origin, Vector3 direction, LayerMask hitLayers, ProjectileUpdateType updateType){
        position = origin;
        directionVector = direction;
        projectileUpdateType = updateType;
        projectileHitLayers = hitLayers;

        OnBirth();
    }

    public Projectile(Vector3 origin, Vector3 direction, float updateTime)
    {
        position = origin;
        directionVector = direction;
        timeStep = updateTime;
    }

    private bool Step()
    {
        float speed = (speedPerSecond * speedModifier * timeStep);
        Vector3 direction = directionVector * speed;
        Ray bulletRay = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(bulletRay, out hit, speed, projectileHitLayers))
        {
            position = hit.point;
            OnHit();
            Death();
            return true;
        }
        position += direction;
        return false;
    }

    private void Death()
    {

    }

    public void OnStep() { }
    public void OnHit() { }
    public void OnBirth() { }

}
