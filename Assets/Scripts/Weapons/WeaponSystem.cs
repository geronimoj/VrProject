using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    //public WeaponManager wm;
    public Weapon weapon;

    public List<Weapon> weapons;
    /// <summary>
    /// This block is temporary, for the very first weapon for testing
    /// </summary>
    public Transform anchor;

    public List<Transform> guns;

    /// <summary>
    /// Setups the primary gun and WeaponSelector
    /// </summary>
    void Start()
    {
        if (!weapon && weapons != null && weapons.Count > 0)
            //This could throw an exception if weapons is null or length is 0
            weapon = weapons[0];
        else
            Debug.LogError("Could not assign current weapon to weapons[0]. Weapons is either null or has a length of 0");
        //Subscribe to the weapon change event
        WeaponSelector.OnChangeWeapon.AddListener(ChangeWeapon);

        weapon.OnEquip();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {   //Make sure weapons is valid before entering the loop
            if (weapons != null)
                for (int i = 0; i < weapons.Count; i++)
                {   //Make sure weapons is not null before calling function
                    if (weapons[i])
                        weapons[i].WeaponUpdate();
                }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in for loop in WeaponSystem! Unable to call WeaponUpdate! \n" + e);
            //We don't need to throw, we can just go to the GetInput because it probably still is fine
            //Besides, the only errors that could appear are weapons containing a null or weapons iteself being null
            //throw;
        }
        //Added a null pointer check for the current weapon.
        if (weapon && OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && weapon.CanFire)
        {
            Fire();
        }
        else if(weapon && OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && weapon.uniqueWeapon)
        {
            Fire();
        }
    }

    public void Fire()
    {   /*
        // Layermask will only test against 8 (Enemy), 9 (Wall) and 10 (EnemyProjectile)
        int layer1 = 8;
        int layer2 = 9;
        int layer3 = 10;
        // Bitshifting to get the correct layer
        int mask1 = 1 << layer1;
        int mask2 = 1 << layer2;
        int mask3 = 1 << layer3;

        // Adding the masks together because that works for some reason
        int finalMask = mask1 | mask2 | mask3; */
        //Use Unity's GetMask function to get the masks instead of bit shifting operations that could break if the Masks are moved
        int finalMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");

        if (Physics.Raycast(anchor.position, anchor.forward, out RaycastHit hit, Mathf.Infinity, finalMask))
        {
            try
            {   //Null reference catch
                if (guns != null)
                    for (int i = 0; i < guns.Count; i++)
                    {   //Null reference catch
                        if (guns[i])
                            guns[i].forward = hit.point - guns[i].position;

                    }
            }
            catch (System.Exception e)
            {   //Also log the expection
                Debug.LogError("Error in for loop in WeaponSystem! Unable to fire weapon! \n" + e);
                throw;
            }

            weapon.Fire(guns);
        }
    }
    /// <summary>
    /// Swaps the current usable weapon
    /// </summary>
    /// <param name="newWeapon">The new weapon to use</param>
    public void ChangeWeapon(Weapon newWeapon)
    {   //Is the weapon registered as a usable weapon
        if (!weapons.Contains(newWeapon))
            return;
        //Unequip the weapon
        weapon.OnUnEquip();
        //Swap weapons
        weapon = newWeapon;
        //Equip the new weapon
        weapon.OnEquip();
    }

}
