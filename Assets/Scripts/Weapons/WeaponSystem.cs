﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    // Reference to the WeaponSelector to directly access it's weapon array
    public WeaponSelector ws;
    
    public Weapon weapon;

    public List<Weapon> weapons;

    public Weapon triDisaster;
    /// <summary>
    /// This block is temporary, for the very first weapon for testing
    /// </summary>
    public Transform anchor;

    public List<Transform> guns;
    /// <summary>
    /// Is the safetyMode on
    /// </summary>
    public static bool safetyMode = false;
    /// <summary>
    /// The cooldown of triDisaster
    /// </summary>
    public float triDistasterCooldown = 50;
    /// <summary>
    /// The duration of triDisaster
    /// </summary>
    public float triDisasterDuration = 5;

    private float t_durationTimer = 0;
    private float t_cooldownTimer = 0;

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

        GameManager.s_instance.OnWin.AddListener(UpgradeWeapon);

        // Initialize the WeaponSelector array from here using the first three weapons in our list
        InitializeSelector();

        t_cooldownTimer = triDistasterCooldown;
        t_durationTimer = triDisasterDuration;

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
        //If safetyMode is on, no shoot shoot
        if (safetyMode)
            return;
        //Timers
        if (t_durationTimer < triDisasterDuration)
            t_durationTimer += Time.deltaTime;
        //Only reduce the cooldown of tridisaster if its not being used
        else
        {   //If the duration timer just completed, equip the main weapon
            if (t_cooldownTimer == 0)
                weapon = weapons[0];
            //Start the cooldown
            t_cooldownTimer += Time.deltaTime;
        }
        //Check if they want to equip triDisaster
        if (triDisaster && t_cooldownTimer >= triDistasterCooldown && OGInputGetter.GetDown(OGInputGetter.OculusInputs.FaceButton))
        {   //Equip the weapon
            ChangeWeapon(triDisaster);
            //Reset the timers
            t_cooldownTimer = 0;
            t_durationTimer = 0;
        }

        //Added a null pointer check for the current weapon.                //No-longer need the else if
        if (weapon && OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && (weapon.CanFire || weapon.uniqueWeapon))
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

    public void UpgradeWeapon()
    {
        int randomUpgrade = Random.Range(3, 6);
        Weapon w = weapons[randomUpgrade];
        // Check to see if the weapon we pass in is any of the upgraded weapons
        if (w as SplitBeam)
        {
            ws.m_weapons[0] = weapon;
        }
        else if (w as LockOnRockets)
        {
            ws.m_weapons[1] = weapon;
        }
        else if (w as SpreadMinigun)
        {
            ws.m_weapons[2] = weapon;
        }
        else
        {
            Debug.LogError("No suitable upgrade for " + weapon.name);
        }
        Debug.Log("Weapon upgrade received: " + weapons[randomUpgrade].name);

        ChangeWeapon(w);
    }

    private bool InitializeSelector()
    {
        try
        {
            Weapon[] w = new Weapon[] { weapons[0], weapons[1], weapons[2] };
            ws.m_weapons = w;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to initialize WeaponSelector from WeaponSystem with exception " + e);
            return false;
        }
    }

}
