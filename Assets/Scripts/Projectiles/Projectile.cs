using UnityEngine;
using System.Collections;

public abstract class Projectile : PooledMonoBehaviour<Projectile> {

    public enum DamageInflictionType
    {
        Bullet,
        Splash,
        Other
    }

    public enum DamageTypes
    {
        Normal,
        Fire,
        Ice,
        Posion
    }

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
    public DamageTypes damageType;

    public float speedPerSecond;
    public float speedModifier = 1;
    public Vector2 possibleSpreadModifier;

    public ProjectileUpdateType projectileUpdateType = ProjectileUpdateType.Update;
    public float possibleTimeStep;

    public float knockbackForce;
    public Vector3 knockbackModifier;
    public DamageInflictionType damageInflictionWhenDirectHit = DamageInflictionType.Bullet;

    public float bulletRadius = 0;
    public bool useRadius;
    public bool useFalloffDamage;

    public float bulletSplashRadius = 0;
    public bool useSplash;
    public bool useFalloffSplashDamage = true;
    public DamageInflictionType damageInflictionWhenSplash = DamageInflictionType.Splash;

    [Range(0, 10)]
    public float gravityModifier = 0;
    public Vector3 startingVelocityModifier;
    public bool isLocalVelocity = true;

    public float maxLifetime = 20;
    public float maxDistance;

    public ProjectileDeathType projectileDeathType = ProjectileDeathType.Lifetime;

    public bool placeHitMark = true;
    public ProjectileHitMark hitMark;
    public float hitMarkLifetime;

    public TrailRenderer bulletTrail;

    public bool IsDebug = false;

    private ProjectileManager owner;
    private float distanceTravelled;
    private float currentLifetime;
    private Vector3 oldPos;
    
    private float timeStep
    {
        set { possibleTimeStep = value; }
        get { return ((projectileUpdateType == ProjectileUpdateType.None) ? possibleTimeStep : ((projectileUpdateType == ProjectileUpdateType.Update) ? Time.deltaTime : Time.fixedDeltaTime));  }
    }

	public void Fire(Vector3 origin, Vector3 direction, LayerMask hitLayers, ProjectileUpdateType updateType, ProjectileManager own){
        owner = own;
        position = origin;
        directionVector = direction;//+(startingVelocityModifier+((isLocalVelocity)?transform.forward:Vector3.one));
        projectileUpdateType = updateType;
        projectileHitLayers = hitLayers;
        transform.position = position;

        distanceTravelled = 0;
        currentLifetime = 0;
        oldPos = position;
        bulletTrail.Clear();
        OnBirth();

        StartCoroutine(StepBullet());

    }

    private bool Step()
    {
        oldPos = position;
        Vector3 velocity = StepProjectileVelocity();
        float velocityMag = velocity.magnitude;
        distanceTravelled += velocityMag;
        if (velocityMag > maxDistance)
        {
            velocity = velocity.normalized * maxDistance;
        }
        currentLifetime += timeStep;
        Ray bulletRay = new Ray(position, velocity);
        RaycastHit hit;
        bool didHit = (useRadius)? Physics.SphereCast(bulletRay,bulletRadius,out hit, velocityMag, projectileHitLayers, QueryTriggerInteraction.Ignore) : Physics.Raycast(bulletRay, out hit, velocityMag, projectileHitLayers, QueryTriggerInteraction.Ignore);
        if (didHit)
        {
            position = hit.point;
            transform.position = hit.point;
            OnHit();
            if (placeHitMark)
            {
                PlaceHitMark(hit);
            }
            HandleDamage(hit);
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
        return directionVector * speed + (Physics.gravity*timeStep*gravityModifier);
    }

    virtual protected void PlaceHitMark(RaycastHit hit)
    {
        if (hitMark)
        {
            ProjectileHitMark mark = hitMark.GetInstanceFromPool();
            mark.transform.position = hit.point;
            mark.transform.forward = hit.normal;
            mark.Place(hitMarkLifetime);
        }
    }

    IEnumerator StepBullet()
    {
        yield return new WaitForSeconds(timeStep);
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

    public virtual void HandleDamage(RaycastHit hit)
    {
		IHitTarget hitTarget = hit.collider.transform.root.gameObject.GetComponent<IHitTarget>();
        HitData hitData;

        Vector3 knockback = transform.forward * knockbackForce + knockbackModifier;

        if (hitTarget != null)
        {
            hitData = new HitData(owner,damage,knockback,damageType,damageInflictionWhenDirectHit, hit.point);
            hitTarget.OnDamageHit(hitData);
        }
        if (useSplash)
        {
            if (IsDebug)
            {
                Debug.DrawLine(transform.position, transform.position + Vector3.up * bulletSplashRadius / 2,Color.blue, 10);
                Debug.DrawLine(transform.position, transform.position + Vector3.down * bulletSplashRadius / 2, Color.blue, 10);
                Debug.DrawLine(transform.position, transform.position + Vector3.left * bulletSplashRadius / 2, Color.blue, 10);
                Debug.DrawLine(transform.position, transform.position + Vector3.right * bulletSplashRadius / 2, Color.blue, 10);
                Debug.DrawLine(transform.position, transform.position + Vector3.forward * bulletSplashRadius / 2, Color.blue, 10);
                Debug.DrawLine(transform.position, transform.position + Vector3.back * bulletSplashRadius / 2, Color.blue, 10);
            }
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, bulletSplashRadius, projectileHitLayers);
            foreach(Collider c in hitObjects)
            {
                IHitTarget explosionTarget = c.GetComponent<IHitTarget>();
                if(explosionTarget != null)
                {
                    float damageModifier = (useFalloffSplashDamage) ? 1 - (c.transform.position - transform.position).magnitude / bulletSplashRadius : 1;
                    knockback = (c.transform.position - transform.position) * knockbackForce * damageModifier + knockbackModifier;
                    hitData = new HitData(owner, damage*damageModifier, knockback, damageType, damageInflictionWhenSplash, hit.point);
                    explosionTarget.OnDamageHit(hitData);
                    if (IsDebug)
                    {
                        Debug.DrawLine(transform.position, c.transform.position, Color.red, 10);
                    }
                }
            }
        }
    }

    public virtual void OnStep() { }
    public virtual void OnHit() { }
    public virtual void OnBirth() { }

}
