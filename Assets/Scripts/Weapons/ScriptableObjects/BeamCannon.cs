using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeamCannon", menuName = "Weapon/Energy/Beam Cannon", order = 1)]
public class BeamCannon : Weapon
{
    public LineRenderer lr;
    public Health health;
    private GameObject g;
    private int layerMask = 0;

    public override void Fire(List<Transform> guns)
    {
        // Spawn a beam from gun 2 (Center Gun)
        if(!g)
            g = Instantiate(spawnable, guns[2].position, guns[2].rotation);

        if (!lr)
            lr = g.GetComponent<LineRenderer>();

        if(layerMask == 0)
        {
            layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");
        }
        

        if (Physics.Raycast(g.transform.position, g.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            lr.SetPosition(0, g.transform.position);
            lr.SetPosition(1, hit.point);


            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                if (!health || hit.collider.gameObject != health.gameObject)
                    health = hit.collider.gameObject.GetComponent<Health>();

                health.DoDamage(damage);
            }
        }


        base.Fire(guns);
    }
}
