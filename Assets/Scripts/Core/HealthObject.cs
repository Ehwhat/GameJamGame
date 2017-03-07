using UnityEngine;
using System.Collections;

public abstract class HealthObject : MonoBehaviour {

    public bool isDebug;

    public bool clampHealth = true;
    public float maxHealth = 100;

    private float m_health;
    public float currentHealth { get { return m_health; } set { SetHealth(value); } }

    public bool isDead = false;

    private void Awake()
    {
        RefillHealth();
    }

    private void SetHealth(float value)
    {
        m_health = (clampHealth) ? Mathf.Clamp(value,0,maxHealth) : value;
        if(m_health <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                OnDeath();
            }
        }else if (isDead)
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

}
