using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Redirects damage taken to a different health script
/// </summary>
public class SeparateHealth : Weakpoint
{
    /// <summary>
    /// The target to redirect damage to
    /// </summary>
    [Tooltip("Redirects any damage taken by this health to a target")]
    public Health m_damageDestination = null;
    /// <summary>
    /// Should damage be dealt also to this
    /// </summary>
    public bool m_alsoDealDamageToThis = false;
    /// <summary>
    /// Deals damage to the target and also this if m_alsoDealDamageToThis is true
    /// </summary>
    /// <param name="damage">The damage to deal</param>
    public override bool DoDamage(float damage)
    {   //Check if we should deal damage to this
        if (m_alsoDealDamageToThis)
            //Call the base
            return base.DoDamage(damage);
        //Modify the damage
        damage *= m_weakpointModifier;
        //Redirect the damage to the target
        m_damageDestination.DoDamage(damage);
        return true;
    }
}
