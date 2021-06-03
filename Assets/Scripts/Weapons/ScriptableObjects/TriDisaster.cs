using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriDisaster", menuName = "Weapon/Unique/Tri-Disaster", order = 1)]
public class TriDisaster : Weapon
{
    public GameObject laser;
    public GameObject rocket;

    public float laserDamage;
    public float rocketDamage;
    public float beamDamage;

    public float laserSpeed;
    public float rocketSpeed;

    private LineRenderer lr;
    private int layerMask;
    private Health health;

    public override void UniqueFire(List<Transform> guns)
    {
        FireRockets(guns[0]);
        FireRockets(guns[4]);
        FireLasers(guns[1]);
        FireLasers(guns[3]);
        FireBeam(guns[2]);

        base.UniqueFire(guns);
    }

    public void FireRockets(Transform gun)
    {
        GameObject b = Instantiate(rocket, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = rocketDamage;
        p.projectileSpeed = rocketSpeed;
        p.explode = true;
        p.explosionRadius = explosionRadius;
    }

    public void FireLasers(Transform gun)
    {
        GameObject b = Instantiate(laser, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = laserDamage;
        p.projectileSpeed = laserSpeed;
    }

    public void FireBeam(Transform gun)
    {
        if(layerMask == 0)
            layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");
        if (!lr)
            lr = gun.gameObject.GetComponent<LineRenderer>();

        if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            lr.SetPosition(1, hit.point);

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                if (!health || hit.collider.gameObject != health.gameObject)
                    health = hit.collider.gameObject.GetComponent<Health>();

                health.DoDamage(damage);
            }
        }

    }
}
