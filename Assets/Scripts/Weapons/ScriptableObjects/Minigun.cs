using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Minigun", menuName = "Weapon/Ballistics/Minigun", order = 1)]
public class Minigun : Weapon
{
    public override void Fire(Transform gun)
    {
        GameObject b = Instantiate(spawnable, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;
        p.lifetime = projectileLifetime;
        p.damageType = weaponType;
        p.seeking = seeking;
        p.homingAngle = turnAngle;

        base.Fire(gun);
    }
}
