using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    private float currentHealth;

    void Start()
    {
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    public void DoDamage(float damage)
    {
        currentHealth -= damage;
    }
}
