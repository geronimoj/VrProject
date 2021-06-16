using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeamCannon", menuName = "Weapon/Energy/Beam Cannon", order = 1)]
public class BeamCannon : Weapon
{
    public List<LineRenderer> lines = new List<LineRenderer>(0);
    public Health health;
    private int layerMask = 0;
    private int numGuns = 0;

    private float fireTimer = 0.3f;
    private float damageTick = 0.25f;
    private float damageTickTimer = 0;

    public override void OnStartup()
    {

        base.OnStartup();
    }

    public override void Fire(Transform gun)
    {
        if (lines.Count == 0)
        {
            var parts = System.Enum.GetValues(typeof(Guns));
            foreach (Guns part in parts)
            {
                if ((gunParts & part) != 0)
                {
                    numGuns++;
                }

            }
        }

        if(lines.Count < numGuns)
        {
            lines.Add(gun.gameObject.GetComponent<LineRenderer>());
            Debug.Log("Adding Line Renderer from gun " + lines.Count);
        }
        

        if(layerMask == 0)
        {
            layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");
        }
        

        if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].SetPosition(1, hit.point);
            }

            if (hit.collider.gameObject.CompareTag("Enemy") && damageTickTimer >= damageTick)
            {
                if (!health || hit.collider.gameObject != health.gameObject)
                    health = hit.collider.gameObject.GetComponent<Health>();
                //If its armoured health, use its dodamage function
                if (health as ArmouredHealth)
                    (health as ArmouredHealth).DoDamage(damage, weaponType);
                //Otherwise just use the normal
                else
                    health.DoDamage(damage);

                damageTickTimer = 0;
            }
            //If its a projectile, destroy it
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
                Destroy(hit.collider.gameObject);
        }

        fireTimer = 0.3f;

        base.Fire(gun);
    }

    public override void WeaponUpdate()
    {
        if (lines != null)
        {
            if (currentRefire <= 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {   //Make sure the lines are not null
                    if (lines[i])
                        lines[i].SetPosition(1, lines[i].GetPosition(0));
                }
            }
        }

        fireTimer -= Time.deltaTime;
        if(fireTimer > 0)
        {
            damageTickTimer += Time.deltaTime;
        }
        
        base.WeaponUpdate();
    }

    public override void OnEquip()
    {
        numGuns = 0;
        lines.Clear();
    }
}
