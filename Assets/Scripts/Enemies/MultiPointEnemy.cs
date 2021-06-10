using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An enemy that instead has multiple points that must be destroyed to kill it
/// </summary>
public class MultiPointEnemy : Enemy
{
    /// <summary>
    /// The points that have to be destroyed.
    /// </summary>
    [Tooltip("The points that have to be destroyed for this enemy to die. This can be assigned Weakpoints, Health or SeparateHealth scripts.")]
    public Health[] m_pointsToDestroy = new Health[0];
    /// <summary>
    /// Dissables Destroy on death
    /// </summary>
    protected override void Start()
    {
        base.Start();
        //We don't want to destroy this when its HP hits 0
        destroyOnDeath = false;
        //We don't want the points destroying themself on death otherwise it causes reference errors
        //We could design a system around this.
        //Yea I'm going to do that now
        /*
        foreach (Health h in m_pointsToDestroy)
            h.destroyOnDeath = false;*/
    }
    /// <summary>
    /// Checks if each of the points are dead
    /// </summary>
    protected override void Update()
    {
        base.Update();
        //Check if all of the pointsToDestroy are dead
        int alive = 0;
        for (int i = 0; i < m_pointsToDestroy.Length; i++)
        {   //Null catch. If they are destroyed or the reference is lost, they will be considered dead
            if (m_pointsToDestroy[i])
                //Check if its dead
                if (!m_pointsToDestroy[i].IsDead)
                    //If its alive, increment alive
                    alive++;
        }
        //Check that everything is dead
        if (alive == 0)
        {   //Call OnDeath
            OnDeath.Invoke();
            //Destroy this gameObject
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Removes all code from DoDamage. You cannot deal damage to this enemy directly
    /// </summary>
    /// <param name="damage">The damage to deal</param>
    public override bool DoDamage(float damage)
    {
        return true;
    }
}
