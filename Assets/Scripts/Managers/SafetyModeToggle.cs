using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyModeToggle : MonoBehaviour
{
    public bool safetyModeValue = true;

    private void Start()
    {
        WeaponSystem.safetyMode = safetyModeValue;
    }
}
