using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuadMinigun", menuName = "Weapon/Ballistics/Quad Minigun", order = 1)]
public class QuadMinigun : Weapon
{
    public override void Fire(List<Transform> guns)
    {
        ///
        // Spawning bullets at guns 1 (Left), 3 (Right), 0 (Far Left), and 4 (Far Right)
        ///
        GameObject b = Instantiate(spawnable, guns[1].position, guns[1].rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        b = Instantiate(spawnable, guns[3].position, guns[3].rotation);
        p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        b = Instantiate(spawnable, guns[0].position, guns[0].rotation);
        p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        b = Instantiate(spawnable, guns[4].position, guns[4].rotation);
        p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;

        base.Fire(guns);
    }
}
