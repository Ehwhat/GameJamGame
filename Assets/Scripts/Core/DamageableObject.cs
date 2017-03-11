using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class DamageableObject : MonoBehaviour, IHitTarget
{

    [System.Serializable]
    public class DamageTypeModifier
    {
        public Projectile.DamageTypes type;
        public float percentActuallyAfflicted = 1;
    }

    public bool isDebug;

    public bool clampHealth = true;
    public float maxHealth = 100;

    private float m_health;

    public bool applyGeneralModifierFirst = true;
    public float generalDamageModifier = 1;
    public List<DamageTypeModifier> damageTypeModifiers;
    public float currentHealth { get { return m_health; } set { SetHealth(value); } }

    public bool isDead = false;

    private void Awake()
    {
        RefillHealth();
    }

    private void SetHealth(float value)
    {
        m_health = (clampHealth) ? Mathf.Clamp(value, 0, maxHealth) : value;
        if (m_health <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                OnDeath();
            }
        }
        else if (isDead)
        {
            HandleResurrection();
        }
    }

    public void RefillHealth()
    {
        currentHealth = maxHealth;
    }

    private void HandleResurrection() //and on the third day, he rose!
    {
        isDead = false;
        OnResurrect();
    }

    public virtual void OnDeath() { if (isDebug) { Debug.Log(name + " Died!"); } }
    public virtual void OnResurrect() { if (isDebug) { Debug.Log(name + " Resurrected!"); } }

    public void OnDamageHit(HitData hit)
    {
        OnObjectHit(hit);
        float resultDamage = hit.damage * ((applyGeneralModifierFirst) ? generalDamageModifier : 1);
        foreach (DamageTypeModifier dtm in damageTypeModifiers)
        {
            if (dtm.type == hit.damageType)
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

}

