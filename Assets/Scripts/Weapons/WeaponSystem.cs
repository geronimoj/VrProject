using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    //public WeaponManager wm;
    private Weapon weapon;

    public List<Weapon> weapons;
    /// <summary>
    /// This block is temporary, for the very first weapon for testing
    /// </summary>
    public Transform anchor;

    public List<Transform> guns;

    // Start is called before the first frame update
    void Start()
    {
        if (!weapon)
        {
            weapon = weapons[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].WeaponUpdate();

            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Error in for loop in WeaponSystem! Unable to call WeaponUpdate!");
            throw;
        }

        if (OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && weapon.currentRefire <= 0)
        {
            Fire();
        }
    }

    public void Fire()
    {
        // Layermask will only test against 8 (Enemy), 9 (Wall) and 10 (EnemyProjectile)
        int layer1 = 8;
        int layer2 = 9;
        int layer3 = 10;
        // Bitshifting to get the correct layer
        int mask1 = 1 << layer1;
        int mask2 = 1 << layer2;
        int mask3 = 1 << layer3;

        // Adding the masks together because that works for some reason
        int finalMask = mask1 | mask2 | mask3;

        if (Physics.Raycast(anchor.position, anchor.forward, out RaycastHit hit, Mathf.Infinity, finalMask))
        {
            try
            {
                for (int i = 0; i < guns.Count; i++)
                {
                    guns[i].forward = hit.point - guns[i].position;

                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Error in for loop in WeaponSystem! Unable to fire weapon!");
                throw;
            }

            weapon.Fire(guns);
        }
    }

    public void ChangeWeapon(int weaponIndex)
    {
        weapon = weapons[weaponIndex];

    }

}
