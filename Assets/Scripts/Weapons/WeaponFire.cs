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
    public Transform anchor;
    public float damage;
    public float projectileSpeed;
    public float cooldown;
    private float currentCooldown;
    public Transform spawnPoint1, spawnPoint2;
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        currentCooldown = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown >= 0)
            currentCooldown -= Time.deltaTime;
        if (OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && currentCooldown <= 0)
        {
            Fire();
            currentCooldown = cooldown;
        }
    }

    protected virtual void Fire()
    {
        int layer1 = 8;
        int layer2 = 9;
        int layer3 = 10;
        int mask1 = 1 << layer1;
        int mask2 = 1 << layer2;
        int mask3 = 1 << layer3;

        int finalMask = mask1 | mask2 | mask3;

        Vector3 desiredRotation = new Vector3(0, 0, 0);

        if (Physics.Raycast(anchor.position, anchor.forward, out RaycastHit hit, Mathf.Infinity, finalMask))
        {
            spawnPoint1.forward = hit.point - spawnPoint1.position;
            spawnPoint2.forward = hit.point - spawnPoint2.position;
        }

        GameObject b = Instantiate(bullet, spawnPoint1.position, spawnPoint1.transform.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;
        if (spawnPoint2)
        {
            b = Instantiate(bullet, spawnPoint2.position, spawnPoint2.transform.rotation);
            p = b.GetComponent<Projectile>();
            p.damage = damage;
            p.projectileSpeed = projectileSpeed;
        }
    }

}
