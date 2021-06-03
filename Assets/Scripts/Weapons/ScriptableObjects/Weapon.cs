using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    // Ballistic weapons have lower damage and higher speed, effective vs small, fast enemies.
    Ballistic = 0,
    // Explosive weapons have high damage and low speed, effective vs large armoured enemies.
    Explosive,
    // Energy weapons are unique and effective vs enemies with energy shields.
    Energy
}

public enum FireType
{
    // Hold trigger to continuously fire the weapon.
    Automatic = 0,
    // Press trigger to fire the weapon.
    SemiAutomatic,
    // Hold trigger to charge weapon, release to fire.
    Charge,
    // Hold trigger to lock on to enemies, release to fire at all locks.
    LockOn,

}

public enum ProjectileType
{
    // Projectile will travel at a specified speed.
    Projectile = 0,
    // Projectile is an instant line from point A to point B.
    Beam,
}

[System.Flags]
public enum Guns
{
    FarLeft =1,
    Left =2,
    Center =4,
    Right =8,
    FarRight = 16,
}


public class Weapon : ScriptableObject
{

    public Guns gunParts;

    /// <summary>
    /// The icon for the weapon.
    /// </summary>
    public Sprite weaponIcon = null;
    // The type of weapon, for the purposes of enemy types and resistances.
    public WeaponType weaponType;

    // Behaviour for the weapon when the trigger is pressed or held.
    public FireType fireType;

    // Behaviour for the weapon after being fired.
    public ProjectileType projectileType;

    // The GameObject to be spawned by the scriptable object.
    public GameObject spawnable;

    // How fast the projectile should move. Ignored for Beam weapons.
    public float projectileSpeed;

    // How much damage the weapon does. Applied on collision for projectiles, or every frame for beams.
    public float damage;

    // How long the player must wait before being able to fire the weapon again.
    public float refireSpeed;

    public bool uniqueWeapon;

    // The current cooldown based on refireSpeed, so that each weapon can track it's own refire.
    [HideInInspector]
    public float currentRefire = 0;
    /// <summary>
    /// Returns true if the currentRefire is 0 or less
    /// </summary>
    public bool CanFire => currentRefire <= 0;

    public float explosionRadius;

    public virtual void Fire(Transform gun) { 
    }

    public virtual void Fire(List<Transform> guns)
    {   //Set the guns cooldown between shots
        currentRefire = refireSpeed;

        if (!uniqueWeapon)
        {
            var parts = System.Enum.GetValues(typeof(Guns));
            int i = 0;
            foreach (Guns part in parts)
            {
                if ((gunParts & part) != 0)
                {
                    Fire(guns[i]);
                }
                i++;
            }
        }
        else
        {
            UniqueFire(guns);
        }
        
    }

    public virtual void UniqueFire(List<Transform> guns) { }

    public virtual void WeaponUpdate()
    {   //Reduce the cooldown between shots
        currentRefire -= Time.deltaTime;


        
    }
    /// <summary>
    /// Called when the weapon is equipped
    /// </summary>
    public virtual void OnEquip() { }
    /// <summary>
    /// Called when the weapon is unequipped
    /// </summary>
    public virtual void OnUnEquip() { }

}
