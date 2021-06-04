﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RocketLauncher", menuName = "Weapon/Explosives/Rocket Launcher", order = 1)]
public class RocketLauncher : Weapon
{
    public override void Fire(Transform gun)
    {
        GameObject b = Instantiate(spawnable, gun.position, gun.rotation);
        Projectile p = b.GetComponent<Projectile>();
        p.damage = damage;
        p.projectileSpeed = projectileSpeed;
        p.explode = true;
        p.explosionRadius = explosionRadius;

        base.Fire(gun);
    }
}
