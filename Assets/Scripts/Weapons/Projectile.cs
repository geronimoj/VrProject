using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    public float projectileSpeed;
    public float damage;

    public float lifetime;

    void Start()
    {
        StartCoroutine(DestroyAfter());
    }

    private IEnumerator DestroyAfter()
    {
        yield return new WaitForSecondsRealtime(lifetime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Health>().DoDamage(damage);
            Destroy(gameObject);
        }
    }
}
