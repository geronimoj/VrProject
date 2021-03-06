using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    protected float currentHealth;
    /// <summary>
    /// Returns true when the charcater is dead
    /// </summary>
    public bool IsDead => currentHealth <= 0;

    public float CurrentHealth => currentHealth;
    /// <summary>
    /// Should the gameObject be destroyed on death
    /// </summary>
    public bool destroyOnDeath = true;

    public UnityEvent OnDeath;

    public UnityEvent OnTakeDamage;

    protected virtual void Start()
    {
        currentHealth = health;
    }

    //Put the death check in DoDamage to reduce calls

    public virtual bool DoDamage(float damage)
    {   //Don't deal damage to a dead character
        if (IsDead)
            return false;

        currentHealth -= damage;

        OnTakeDamage.Invoke();

        if (IsDead)
        {
            OnDeath.Invoke();

            if (destroyOnDeath)
                Destroy(gameObject);
        }
        return true;
    }
}
