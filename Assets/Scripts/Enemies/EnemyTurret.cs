using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    /// <summary>
    /// The player target the gun is targeting
    /// </summary>
    [Tooltip("DEBUG ONLY. DO NOT MANIPULATE.")]
    [SerializeField]
    private int target = 0;
    /// <summary>
    /// Updates the orientation of the turret
    /// </summary>
    private void LateUpdate()
    {   //Look at the target point
        Vector3 toTarget = Enemy.s_targets[target].position - transform.position;
        transform.rotation = Quaternion.LookRotation(-toTarget.normalized, transform.up);
    }
    /// <summary>
    /// Sets the target the ship is targeting
    /// </summary>
    /// <param name="t"></param>
    public void SetTarget(int t)
    {
        target = t;
        //Call fixedUpdate, this contains the code for rotating the gun
        LateUpdate();
    }
}
