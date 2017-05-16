using UnityEngine;
using System.Collections;

public class HitData
{

    public float damage;
    public Vector3 knockback;
    public ProjectileManager owner;
    public Projectile hitProjectile;

    public Projectile.DamageTypes damageType;
    public Projectile.DamageInflictionType damageInfliction;

    public RaycastHit rayHit;

    public HitData(ProjectileManager _owner,Projectile _hitProjectile ,float _damage, Vector3 _knockback, Projectile.DamageTypes _damageType, Projectile.DamageInflictionType _damageInfliction, RaycastHit _rayHit)
    {
        damage = _damage;
        knockback = _knockback;
        owner = _owner;
        hitProjectile = _hitProjectile;
        damageType = _damageType;
        damageInfliction = _damageInfliction;
        rayHit = _rayHit;
    }

    public override string ToString()
    {
        return "Owner: " + owner + "\nDamage: " + damage + "\nDamageType: " + damageType + "\nKnockback : " + knockback;
    }

}

public interface IHitTarget {

    void OnDamageHit(HitData hit);

}