using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpreadMinigun", menuName = "Weapon/Ballistics/Spread Minigun", order = 1)]
public class SpreadMinigun : Minigun
{
    [Tooltip("The angle for additional spread bullets to fire at, keep lower for tighter spread.")]
    public Vector2 deviation;

    private Vector2 currentDeviation;

    [Tooltip("Should the bullets converge over time?")]
    public bool converge;

    [Tooltip("The maximum and minimum range for the deviation of the bullets.")]
    public Vector2 convergenceRange;

    [Tooltip("How long in seconds it should take for the bullets to go from max range to min range.")]
    public float convergeTime;
    private float convergeTimer;

    [Tooltip("The number of additional bullets to fire per every normal bullet. Each have different angles.")]
    public int extraBullets;

    private float fireTimer = 0.3f;

    public override void Fire(Transform gun)
    {
        for (int i = 0; i < extraBullets; i++)
        {
            float randX = Random.Range(-currentDeviation.x, currentDeviation.x);
            float randY = Random.Range(-currentDeviation.y, currentDeviation.y);
            Quaternion newRotation = gun.rotation;
            newRotation.x += randX;
            newRotation.y += randY;

            GameObject b = Instantiate(spawnable, gun.position, newRotation);
            Projectile p = b.GetComponent<Projectile>();
            p.damage = damage;
            p.projectileSpeed = projectileSpeed;
            p.lifetime = projectileLifetime;
            p.damageType = weaponType;
        }

        fireTimer = 0.3f;
        
        base.Fire(gun);
    }

    public override void WeaponUpdate()
    {
        fireTimer -= Time.deltaTime;
        convergeTimer += Time.deltaTime;

        if(converge)
        {
            if (fireTimer > 0)
            {
                
                currentDeviation.x = Mathf.Lerp(convergenceRange.x, convergenceRange.y, convergeTimer / convergeTime);
                currentDeviation.y = currentDeviation.x;

            }

            
        }
        if (fireTimer < 0)
        {
            currentDeviation = deviation;
            convergeTimer = 0;
        }

        base.WeaponUpdate();
    }
}
