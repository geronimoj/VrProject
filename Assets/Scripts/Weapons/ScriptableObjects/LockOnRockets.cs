using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LockOnRockets", menuName = "Weapon/Explosives/Lock On Rockets")]
public class LockOnRockets : Weapon
{
    public float lockOnTime;
    public int maxTargets;

    public int rocketsPerTarget;

    private float lockOnTimer;

    private List<Transform> targets = new List<Transform>();
    private List<Transform> guns = new List<Transform>();
    /// <summary>
    /// The reticle system for aiming
    /// </summary>
    private EnemyReticleSystem _defaultReticleSystem = null;
    /// <summary>
    /// The reticle system for displaying targeted enemies
    /// </summary>
    private EnemyReticleSystem _mainReticleSystem = null;

    private void Setup()
    {
        GameObject go;
        if (!_defaultReticleSystem)
        {
            go = GameObject.FindGameObjectWithTag("EnemyReticle");

            if (go)
                _defaultReticleSystem = go.GetComponent<EnemyReticleSystem>();
        }

        if (!_mainReticleSystem)
        {
            go = GameObject.FindGameObjectWithTag("TargetReticle");

            if (go)
                _mainReticleSystem = go.GetComponent<EnemyReticleSystem>();
        }
    }

    public override void OnEquip()
    {
        ClearReticles();
        targets.Clear();
        lockOnTimer = lockOnTime;
        Setup();
        base.OnEquip();
    }

    public override void WeaponUpdate()
    {
        if(lockOnTimer != 0)
            lockOnTimer -= Time.deltaTime;

        if (lockOnTimer < 0)
            Release();

        CheckReticleDisplay();

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
                if (targets.Count < maxTargets && !targets.Contains(hits[i].transform))
                {
                    targets.Add(hits[i].transform);
                    _defaultReticleSystem.StopTracking(hits[i].transform);
                    _mainReticleSystem.EnterReticleView(hits[i].transform);
                }
            }
            
        }
        lockOnTimer = lockOnTime;
    }

    private void Release()
    {
        int currentGun = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            if(rocketsPerTarget < 2)
            {
                GameObject b = Instantiate(spawnable, guns[currentGun % 5].position, guns[currentGun % 5].rotation);
                Projectile p = b.GetComponent<Projectile>();
                p.damage = damage;
                p.projectileSpeed = projectileSpeed;
                p.lifetime = projectileLifetime;
                p.explode = true;
                p.explosionRadius = explosionRadius;
                p.homing = true;
                p.homingTarget = targets[i];
                p.homingAngle = turnAngle;
                p.damageType = weaponType;
                currentGun++;
            }
            else
            {
                
                for (int j = 0; j < rocketsPerTarget; j++)
                {
                    GameObject b = Instantiate(spawnable, guns[currentGun % 5].position, guns[currentGun % 5].rotation);
                    Projectile p = b.GetComponent<Projectile>();
                    p.damage = damage;
                    p.projectileSpeed = projectileSpeed;
                    p.lifetime = projectileLifetime;
                    p.explode = true;
                    p.explosionRadius = explosionRadius;
                    p.homing = true;
                    p.homingTarget = targets[i];
                    p.homingAngle = turnAngle;
                    p.damageType = weaponType;
                    currentGun++;
                }
            }
            
        }
        ClearReticles();
        targets.Clear();
        lockOnTimer = 0;
    }
    /// <summary>
    /// For getting the targets
    /// </summary>
    /// <returns>The targets as an array</returns>
    public Transform[] GetTargets() => targets.ToArray();
    /// <summary>
    /// Removes the reticles from the reticle system
    /// </summary>
    private void ClearReticles()
    {
        if (!_mainReticleSystem)
            return;
        //Remove the targets from the reticle view
        for (int i = 0; i < targets.Count; i++)
        {
            _mainReticleSystem.StopTracking(targets[i]);
            _defaultReticleSystem.TrackEnemy(targets[i]);
        }
    }
    /// <summary>
    /// Ensures only one EnemyReticleSystem is displaying a reticle for a enemy at a time
    /// </summary>
    private void CheckReticleDisplay()
    {   
        for (int i = 0; i < targets.Count; i++)
            //Make sure the enemies being targeted are not being displayed by the other reticle system
            _defaultReticleSystem.StopTracking(targets[i]);
    }
}
