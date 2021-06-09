using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Health with a damage modifier
/// </summary>
public class Weakpoint : ArmouredHealth
{
    /// <summary>
    /// The damage modifier
    /// </summary>
    [Tooltip("The damage modifier for damaging this Health")]
    public float m_weakpointModifier = 1;
    /// <summary>
    /// Multiplies the damage by m_weakpointModifier before dealing damage
    /// </summary>
    /// <param name="damage">The damage to deal</param>
    public override void DoDamage(float damage)
    {   //Modify the damage
        damage *= m_weakpointModifier;
        //Deal the damage
        base.DoDamage(damage);
    }
}
