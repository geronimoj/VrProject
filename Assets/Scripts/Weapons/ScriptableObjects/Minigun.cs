using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Minigun", menuName = "Weapon/Ballistics/Minigun", order = 1)]
public class Minigun : Weapon
{
    public override void Fire(Transform guns)
    {
       GameObject b = Instantiate(spawnable, guns.position, guns.rotation);
       Projectile p = b.GetComponent<Projectile>();
       p.damage = damage;
       p.projectileSpeed = projectileSpeed;

       base.Fire(guns);
    }
}
