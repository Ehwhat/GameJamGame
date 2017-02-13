using UnityEngine;
using System.Collections;

public abstract class Projectile : PooledMonoBehaviour<Projectile> {

    public enum ProjectileUpdateType
    {
        None,
        Update,
        Fixed
    }

    public enum ProjectileDeathType
    {
        Custom,
        Distance,
        Lifetime
    }

    public LayerMask projectileHitLayers;

    protected Vector3 position;
    protected Vector3 directionVector;

    public float damage = 10;

    public float speedPerSecond;
    public float speedModifier = 1;

    public float bulletRadius = 0;
    public bool useRadius;
    public bool useFalloffDamage;

    public float maxLifetime = 20;
    public float maxDistance;

    public ProjectileDeathType projectileDeathType = ProjectileDeathType.Lifetime;

    public bool IsDebug = false;

    private float distanceTravelled;
    private float currentLifetime;
    private Vector3 oldPos;
    private float m_timeStep;
    public float timeStep
    {
        private set { m_timeStep = value; }
        get { return (projectileUpdateType == ProjectileUpdateType.None) ? m_timeStep : (projectileUpdateType == ProjectileUpdateType.Update) ? Time.deltaTime : Time.fixedDeltaTime;  }
    }

    private ProjectileUpdateType projectileUpdateType = ProjectileUpdateType.None;

	public void Fire(Vector3 origin, Vector3 direction, LayerMask hitLayers, ProjectileUpdateType updateType){
        position = origin;
        directionVector = direction;
        projectileUpdateType = updateType;
        projectileHitLayers = hitLayers;
        transform.position = position;

        distanceTravelled = 0;
        currentLifetime = 0;

        OnBirth();

        StartCoroutine(StepBullet());

    }

    public void Fire(Vector3 origin, Vector3 direction, float updateTime)
    {
        position = origin;
        directionVector = direction;
        timeStep = updateTime;
    }

    private bool Step()
    {
        oldPos = position;
        Vector3 velocity = StepProjectileVelocity();
        distanceTravelled += velocity.magnitude;
        currentLifetime += timeStep;
        Ray bulletRay = new Ray(position, velocity);
        RaycastHit hit;
        bool didHit = (useRadius)? Physics.SphereCast(bulletRay,bulletRadius,out hit, velocity.magnitude, projectileHitLayers) : Physics.Raycast(bulletRay, out hit, velocity.magnitude, projectileHitLayers);
        if (didHit)
        {
            position = hit.point;
            OnHit();
            Death(true);
            return true;
        }else
        {

            if (IsDebug)
            {
                Debug.DrawLine(position, position+velocity, Color.blue);
            }

            bool isDead = (projectileDeathType == ProjectileDeathType.Distance) ? (distanceTravelled >= maxDistance) : (projectileDeathType == ProjectileDeathType.Lifetime) ? (currentLifetime >= maxLifetime) : false;
            if (isDead)
            {
                Death(false);
                return true;
            }
        }
        position += velocity;
        transform.position = position;
        if (IsDebug)
        {
            Debug.DrawLine(oldPos, position, Color.yellow, 10);
        }
        return false;
    }

    virtual protected Vector3 StepProjectileVelocity() {
        float speed = (speedPerSecond * speedModifier * timeStep);
        return directionVector * speed;
    }

    IEnumerator StepBullet()
    {
        while (!Step())
        {
            yield return new WaitForSeconds(timeStep);
        }
    }

    public override void OnDespawn()
    {
        
    }

    private void Death(bool wasHit)
    {
        ReturnToPool(this);
    }

    public virtual void OnStep() { }
    public virtual void OnHit() { }
    public virtual void OnBirth() { }

}
