using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DualMinigun", menuName = "Weapon/Ballistics/Dual Minigun", order = 1)]
public class DualMinigun : Weapon
{
    public override void Fire(List<Transform> guns)
    {
        GameObject b = Instantiate(spawnable, guns[1].position, guns[1].rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        b = Instantiate(spawnable, guns[3].position, guns[3].rotation);
        p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        base.Fire(guns);
    }
}
