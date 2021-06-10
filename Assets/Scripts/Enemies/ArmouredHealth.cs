using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmouredHealth : Health
{
    /// <summary>
    /// The type of armour the enemy has
    /// </summary>
    [Tooltip("The type of armour the enemy has")]
    [SerializeField]
    protected ArmourType _armour = ArmourType.Light;
    /// <summary>
    /// The shields or damage bonus
    /// </summary>
    [Tooltip("The damage bonus or shield strength when the armour type is armoured or shielded respectively")]
    [SerializeField]
    protected float _armourAndSheildValue = 0;
    /// <summary>
    /// Called when _armourAndShieldValue reach 0
    /// </summary>
    public UnityEngine.Events.UnityEvent OnBreakShield;
    /// <summary>
    /// Does damage to the enemy given a unique damage type
    /// </summary>
    /// <param name="damage">The damage to deal</param>
    /// <param name="damageType">The type of damage being dealt</param>
    public bool DoDamage(float damage, WeaponType damageType)
    {   //Deal damage the correct way
        switch (damageType)
        {   //Anti Light armour
            case WeaponType.Ballistic:
                //If the damage type is sheilds, don't do anything
                if (_armour == ArmourType.Shielded)
                    return false;
                break;
            case WeaponType.Energy:
                //Deal damage to sheilds first
                if (_armour == ArmourType.Shielded)
                {   //Deal damage to the sheilds
                    _armourAndSheildValue -= damage;
                    //If the shields are gone, make them light armour
                    if (_armourAndSheildValue <= 0)
                    {
                        OnBreakShield.Invoke();
                        _armour = ArmourType.Light;
                        //Do Overkill damage to the heath bar
                        currentHealth -= _armourAndSheildValue;
                    }
                    //Don't deal damage to the ship yet
                    return false;
                }
                break;
            case WeaponType.Explosive:
                //Deal no damage to shielded targets
                if (_armour == ArmourType.Shielded)
                    return false;
                //If the target is armoured, increase the damage of explosive weaponry
                if (_armour == ArmourType.Armoured)
                    damage *= _armourAndSheildValue;
                break;
        }
        //Do damage direct to the health
        return DoDamage(damage);
    }
    /// <summary>
    /// The type of armour the enemy has
    /// </summary>
    public enum ArmourType
    {
        //Normal
        Light = 0,
        //Has armour
        Armoured = 1,
        //Has a shield
        Shielded = 2
    }
}
