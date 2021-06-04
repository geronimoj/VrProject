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

    public float laserRefire;
    public float rocketRefire;

    private float rocket1Refire = 0;
    private float rocket2Refire = 0;
    private float laser1Refire = 0;
    private float laser2Refire = 0;
    private float beamTimer = 0.1f;

    private LineRenderer lr;
    private int layerMask;
    private Health health;

    

    public override void WeaponUpdate()
    {
        laser1Refire -= Time.deltaTime;
        laser2Refire -= Time.deltaTime;
        rocket1Refire -= Time.deltaTime;
        rocket2Refire -= Time.deltaTime;
        beamTimer -= Time.deltaTime;

        if (beamTimer < 0)
            lr.SetPosition(1, lr.GetPosition(0));

        if (laser1Refire < 0)
            laser1Refire = 0;
        if (laser2Refire < 0)
            laser2Refire = 0;
        if (rocket1Refire < 0)
            rocket1Refire = 0;
        if (rocket2Refire < 0)
            rocket2Refire = 0;

        base.WeaponUpdate();
    }

    public override void UniqueFire(List<Transform> guns)
    {
        if(rocket1Refire == 0)
            FireRockets(guns[0], ref rocket1Refire);
        if(rocket2Refire == 0)
            FireRockets(guns[4], ref rocket2Refire);
        if(laser1Refire == 0)
            FireLasers(guns[1], ref laser1Refire);
        if(laser2Refire == 0)
            FireLasers(guns[3], ref laser2Refire);
        


        FireBeam(guns[2]);

        

        base.UniqueFire(guns);
    }

    public void FireRockets(Transform gun, ref float refire)
    {
        GameObject b = Instantiate(rocket, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = rocketDamage;
        p.projectileSpeed = rocketSpeed;
        p.explode = true;
        p.explosionRadius = explosionRadius;

        refire += rocketRefire;
    }

    public void FireLasers(Transform gun, ref float refire)
    {
        GameObject b = Instantiate(laser, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = laserDamage;
        p.projectileSpeed = laserSpeed;

        refire += laserRefire;
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
        beamTimer = 0.1f;
    }
}
