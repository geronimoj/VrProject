using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriDisaster", menuName = "Weapon/Unique/Tri-Disaster", order = 1)]
public class TriDisaster : Weapon
{
    public GameObject bullet;
    public GameObject rocket;

    public bool seekingRockets = false;

    public float bulletDamage;
    public float rocketDamage;
    public float beamDamage;

    public float bulletSpeed;
    public float rocketSpeed;

    public float bulletRefire;
    public float rocketRefire;

    private float rocket1Refire = 0;
    private float rocket2Refire = 0;
    private float bullet1Refire = 0;
    private float bullet2Refire = 0;
    private float beamTimer = 0.1f;

    private LineRenderer lr;
    private int layerMask;
    private Health health;

    

    public override void WeaponUpdate()
    {
        bullet1Refire -= Time.deltaTime;
        bullet2Refire -= Time.deltaTime;
        rocket1Refire -= Time.deltaTime;
        rocket2Refire -= Time.deltaTime;
        beamTimer -= Time.deltaTime;

        if (lr && beamTimer < 0)
            lr.SetPosition(1, lr.GetPosition(0));

        if (bullet1Refire < 0)
            bullet1Refire = 0;
        if (bullet2Refire < 0)
            bullet2Refire = 0;
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
        if(bullet1Refire == 0)
            FireBullets(guns[1], ref bullet1Refire);
        if(bullet2Refire == 0)
            FireBullets(guns[3], ref bullet2Refire);
        


        FireBeam(guns[2]);

        

        base.UniqueFire(guns);
    }

    public void FireRockets(Transform gun, ref float refire)
    {
        GameObject b = Instantiate(rocket, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = rocketDamage;
        p.projectileSpeed = rocketSpeed;
        p.lifetime = projectileLifetime;
        p.explode = true;
        p.explosionRadius = explosionRadius;
        p.damageType = weaponType;
        p.seeking = seekingRockets;


        refire += rocketRefire;
    }

    public void FireBullets(Transform gun, ref float refire)
    {
        GameObject b = Instantiate(bullet, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = bulletDamage;
        p.projectileSpeed = bulletSpeed;
        p.lifetime = projectileLifetime;
        p.damageType = weaponType;

        refire += bulletRefire;
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

                //If its armoured health, use its dodamage function
                if (health as ArmouredHealth)
                    (health as ArmouredHealth).DoDamage(damage, WeaponType.Energy);
                //Otherwise just use the normal
                else
                    health.DoDamage(damage);
            }
        }
        beamTimer = 0.1f;
    }
}
