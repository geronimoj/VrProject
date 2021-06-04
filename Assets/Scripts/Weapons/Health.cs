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

    public virtual void DoDamage(float damage)
    {   //Don't deal damage to a dead character
        if (currentHealth < 0)
            return;

        currentHealth -= damage;

        OnTakeDamage.Invoke();

        if (currentHealth <= 0)
        {
            OnDeath.Invoke();

            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
