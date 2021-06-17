using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitBeam", menuName = "Weapon/Energy/Split Beam Cannon", order = 1)]
public class SplitBeam : Weapon
{
    public float splitRadius;
    public GameObject lineSpawner;
    public int maxLines;

    private List<Health> targets = new List<Health>();
    private List<LineRenderer> primaryLines = new List<LineRenderer>();
    private List<LineRenderer> splitLines = new List<LineRenderer>();

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
        if (primaryLines.Count == 0)
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

        if (primaryLines.Count < numGuns)
        {
            primaryLines.Add(gun.gameObject.GetComponent<LineRenderer>());
        }


        if (layerMask == 0)
        {
            layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");
        }


        if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            for (int i = 0; i < primaryLines.Count; i++)
            {
                primaryLines[i].SetPosition(1, hit.point);
            }

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Health h = hit.collider.gameObject.GetComponent<Health>();
                if (targets.Count < 1 || !targets[0] || !targets.Contains(h))
                    targets.Add(hit.collider.gameObject.GetComponent<Health>());
                //Make sure the main target is alive
                if (!targets[0])
                    return;

                RaycastHit[] hits = Physics.SphereCastAll(targets[0].transform.position, splitRadius, Vector3.up, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide);


                for (int i = 0; i < hits.Length; i++)
                {
                    h = hits[i].collider.gameObject.GetComponent<Health>();
                    if (h && !targets.Contains(h))
                    {
                        targets.Add(h);
                    }
                }

                if (targets.Count > splitLines.Count)
                {
                    int diff = targets.Count - splitLines.Count;

                    for (int i = 0; i < diff; i++)
                    {
                        
                        GameObject o = Instantiate(lineSpawner);
                        splitLines.Add(o.GetComponent<LineRenderer>());
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < splitLines.Count; i++)
                {
                    if (splitLines[i])
                    {
                        splitLines[i].SetPosition(0, Vector3.zero);
                        splitLines[i].SetPosition(1, Vector3.zero);
                    }
                }

                targets.Clear();
            }

            if (targets.Count > 0)
            {

                for (int i = 0; i < targets.Count; i++)
                {   //Make sure target is not null
                    if (!targets[i])
                        continue;

                    if(i > 0)
                    {
                        splitLines[i].SetPosition(0, primaryLines[0].GetPosition(1));
                        splitLines[i].SetPosition(1, targets[i].transform.position);
                    }


                    if (damageTickTimer >= damageTick)
                    {
                        //If its armoured health, use its dodamage function
                        if (targets[i] as ArmouredHealth)
                            (targets[i] as ArmouredHealth).DoDamage(damage, weaponType);
                        //Otherwise just use the normal
                        else
                            targets[i].DoDamage(damage);
                    }

                }

                for (int i = 0; i < targets.Count; i++)
                {
                    if (targets[i] == null)
                        targets.RemoveAt(i);
                }

                if (damageTickTimer > damageTick)
                    damageTickTimer = 0;
            }

            
        }
        else
        {
            for (int i = 0; i < primaryLines.Count; i++)
            {   //Make sure the lines are not null
                if (primaryLines[i])
                    primaryLines[i].SetPosition(1, primaryLines[i].GetPosition(0));

            }
            for (int i = 0; i < splitLines.Count; i++)
            {
                if (splitLines[i])
                {
                    splitLines[i].SetPosition(0, Vector3.zero);
                    splitLines[i].SetPosition(1, Vector3.zero);
                }
            }
        }
        

        fireTimer = 0.3f;

        base.Fire(gun);
    }

    public override void WeaponUpdate()
    {
        if (primaryLines != null)
        {
            if (currentRefire <= 0)
            {
                for (int i = 0; i < primaryLines.Count; i++)
                {   //Make sure the lines are not null
                    if (primaryLines[i])
                        primaryLines[i].SetPosition(1, primaryLines[i].GetPosition(0));
                    
                }
                for (int i = 0; i < splitLines.Count; i++)
                {
                    if (splitLines[i])
                    {
                        splitLines[i].SetPosition(0, Vector3.zero);
                        splitLines[i].SetPosition(1, Vector3.zero);
                    }
                }
            }
        }

        if(splitLines.Count >= maxLines)
        {
            int l = splitLines.Count;
            for (int i = 0; i < l; i++)
            {
                Destroy(splitLines[i].gameObject);
            }

            ClearLines();
        }


        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
            damageTickTimer += Time.deltaTime;
        }

        if (fireTimer < 0)
            targets.Clear();

        base.WeaponUpdate();
    }

    public override void OnEquip()
    {
        numGuns = 0;
        targets.Clear();
        primaryLines.Clear();
        ClearLines();
    }
    /// <summary>
    /// Removes any splitLines that are null
    /// </summary>
    private void ClearLines()
    {   //Loop over the splitLines
        for (int i = 0; i < splitLines.Count; i++)
            //If the value is null, remove its entry in the list
            if (!splitLines[i])
            {   //Remove it
                splitLines.RemoveAt(i);
                //Keep the indexer valid
                i--;
            }
    }


}
