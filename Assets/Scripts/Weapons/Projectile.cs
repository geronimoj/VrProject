﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   /// <summary>
    /// Determines if it damages enemies or player
    /// </summary>
    public enum Target
    {
        Enemy,
        Player
    }
    // Start is called before the first frame update
    public float projectileSpeed;
    public float damage;

    public float lifetime;

    // Weapon will tell the projectile if it should explode, explosion prefab to be set in editor
    public bool explode;
    public float explosionRadius;
    public GameObject explosion;
    /// <summary>
    /// Should the projectile be destroyed if it collides with an enemy projectile
    /// </summary>
    [Tooltip("Should the projectile be destroyed if it collides with an enemy projectile")]
    public bool destroyOnContactWithProjectile = false;

    private bool flaggedForDestruction = false;
    /// <summary>
    /// The team who takes damage.
    /// </summary>
    [Tooltip("The team that takes the damage")]
    public Target target;

    void Start()
    {
        StartCoroutine(DestroyAfter(gameObject));
    }

    private IEnumerator DestroyAfter(GameObject o)
    {
        if (o == gameObject)
            flaggedForDestruction = true;
        yield return new WaitForSecondsRealtime(lifetime);
        Destroy(o);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!flaggedForDestruction)
            Debug.Log("Hit Detected! Tag: " + col.gameObject.tag);
        //Determine which types of collisions we need to look for
        switch(target)
        {   //We are targeting enemies
            case Target.Enemy:
                //If they are a standard enemy, deal damage
                if (col.gameObject.CompareTag("Enemy"))
                {
                    if (explode)
                    {
                        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.forward);
                        if (hits.Length > 0)
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].collider.gameObject.CompareTag("Enemy"))
                                {
                                    hits[i].collider.gameObject.GetComponent<Health>().DoDamage(damage);
                                }
                            }
                        }
                        GameObject g = Instantiate(explosion, transform.position, Quaternion.identity);
                        StartCoroutine(DestroyAfter(g));
                        Destroy(gameObject);
                    }
                    else
                    {
                        col.gameObject.GetComponent<Health>().DoDamage(damage);
                        Destroy(gameObject);
                    }
                }
                //If they are an enemies projectile, destroy it
                if (col.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
                {   //Destroy the projectile
                    Destroy(col.gameObject);
                    //Are we supposed to destroy this projectile
                    if (destroyOnContactWithProjectile)
                        //Destroy this
                        Destroy(gameObject);
                }
                break;
            //We are targeting the player
            case Target.Player:
                //We do a similar thing with the player
                break;
        }
        
    }
}
