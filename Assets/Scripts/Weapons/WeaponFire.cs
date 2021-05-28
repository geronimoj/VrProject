using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    //public WeaponManager wm;
    //private Weapon weapon;
    /// <summary>
    /// This block is temporary, for the very first weapon for testing
    /// </summary>
    public float damage;
    public float projectileSpeed;
    public Transform spawnPoint1, spawnPoint2;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger))
            Fire();
    }

    protected virtual void Fire()
    {
        GameObject b = Instantiate(bullet, spawnPoint1.position, gameObject.transform.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;
        if (spawnPoint2)
        {
            b = Instantiate(bullet, spawnPoint1.position, Quaternion.identity);
            p = b.GetComponent<Projectile>();
            p.damage = damage;
            p.projectileSpeed = projectileSpeed;
        }
    }

}
