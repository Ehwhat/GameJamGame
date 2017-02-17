using UnityEngine;
using System.Collections;

public class HitData
{

    public float damage;
    public Vector3 knockback;
    public ProjectileManager owner;

    public Projectile.DamageTypes damageType;
    public Projectile.DamageInflictionType damageInfliction;

    public HitData(ProjectileManager _owner,float _damage, Vector3 _knockback, Projectile.DamageTypes _damageType, Projectile.DamageInflictionType _damageInfliction)
    {
        damage = _damage;
        knockback = _knockback;
        owner = _owner;
        damageType = _damageType;
        damageInfliction = _damageInfliction;
    }

    public override string ToString()
    {
        return "Owner: " + owner + "\nDamage: " + damage + "\nDamageType: " + damageType + "\nKnockback : " + knockback;
    }

}

public interface IHitTarget {

    void OnDamageHit(HitData hit);

}
