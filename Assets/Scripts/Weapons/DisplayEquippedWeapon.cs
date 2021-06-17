using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DisplayEquippedWeapon : MonoBehaviour
{
    public WeaponSystem m_system = null;

    private Image _image = null;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (!m_system)
            return;

        if (m_system.EquippedWeapon)
            _image.sprite = m_system.EquippedWeapon.weaponIcon;
    }
}
