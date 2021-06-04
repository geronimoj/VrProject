﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LockOnRockets", menuName = "Weapon/Explosives/Lock On Rockets")]
public class LockOnRockets : Weapon
{
    public float lockOnTime;
    public float homingAngle;
    public int maxTargets;
    private float lockOnTimer;

    private List<Transform> targets = new List<Transform>();
    private List<Transform> guns = new List<Transform>();

    public override void OnEquip()
    {
        targets.Clear();
        lockOnTimer = lockOnTime;
        base.OnEquip();
    }

    public override void WeaponUpdate()
    {
        if(lockOnTimer != 0)
            lockOnTimer -= Time.deltaTime;

        if (lockOnTimer < 0)
            Release();

        base.WeaponUpdate();
    }

    public override void Fire(List<Transform> gun)
    {
        guns = gun;

        int layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");

        RaycastHit[] hits;
        hits = Physics.CapsuleCastAll(guns[2].position, guns[2].position + guns[2].forward * 300, 10, guns[2].forward, Mathf.Infinity, layerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Enemy"))
            {
                if (!targets.Contains(hits[i].transform) && targets.Count < maxTargets)
                {
                    targets.Add(hits[i].transform);
                }
            }
            
        }
        lockOnTimer = lockOnTime;
    }

    private void Release()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            GameObject b = Instantiate(spawnable, guns[i % 5].position, guns[i % 5].rotation);
            Projectile p = b.GetComponent<Projectile>();
            p.damage = damage;
            p.projectileSpeed = projectileSpeed;
            p.lifetime = projectileLifetime;
            p.explode = true;
            p.explosionRadius = explosionRadius;
            p.homing = true;
            p.homingTarget = targets[i];
            p.homingAngle = homingAngle;
        }
        targets.Clear();
        lockOnTimer = 0;
    }
}
