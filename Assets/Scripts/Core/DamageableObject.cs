using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class DamageableObject : HealthObject, IHitTarget {

    [System.Serializable]
    public class DamageTypeModifier
    {
        public Projectile.DamageTypes type;
        public float percentActuallyAfflicted = 1;
    }

    public bool applyGeneralModifierFirst = true;
    public float generalDamageModifier = 1;
    public List<DamageTypeModifier> damageTypeModifiers;


    public void OnDamageHit(HitData hit)
    {
        OnObjectHit(hit);
        float resultDamage = hit.damage * ((applyGeneralModifierFirst)? generalDamageModifier : 1);
        foreach (DamageTypeModifier dtm in damageTypeModifiers)
        {
            if(dtm.type == hit.damageType)
            {
                resultDamage *= dtm.percentActuallyAfflicted;
            }
        }
        resultDamage *= ((!applyGeneralModifierFirst) ? generalDamageModifier : 1);
        DamageFor(resultDamage);
        if (isDebug) { Debug.Log(name + " was hit for " + resultDamage); }
    }

    public virtual void OnObjectHit(HitData hit) { }

    public void DamageFor(float amount)
    {
        currentHealth -= amount;
    }

    public void HealFor(float amount)
    {
        currentHealth += amount;
    }

    public new void RefillHealth()
    {
        base.RefillHealth();
    }

}
