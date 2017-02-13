using UnityEngine;
using System.Collections;

public class HitData
{
    public enum DamageTypes
    {
        Normal,
        Fire,
        Ice,
        Posion,
        Explosive
    }

    public float damage;
    public Vector3 knockback;
    public GameObject owner;

    HitData(GameObject _owner,float _damage, Vector3 _knockback)
    {
        damage = _damage;
        knockback = _knockback;
        owner = _owner;
    }

}

public interface IHitTarget {

    void OnDamageHit(HitData hit);

}
