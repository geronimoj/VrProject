using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WeaponSelector : MonoBehaviour
{
    public class SelectorEvent : UnityEvent<Weapon> { }
    /// <summary>
    /// Called every time the weapon changes via the WeaponSelector
    /// </summary>
    public static SelectorEvent OnChangeWeapon = new SelectorEvent();
    /// <summary>
    /// How long it takes for the selector to open
    /// </summary>
    [Tooltip("How long it takes for the selector to open")]
    [SerializeField]
    protected float _openTime = 0.5f;
    /// <summary>
    /// The timer for openTime
    /// </summary>
    private float t_openTimer = 0;
    /// <summary>
    /// Stores the inital scale of the weapon UI
    /// </summary>
    [Tooltip("Stores the inital scale of the weapon UI. Available for debugging")]
    [SerializeField]
    private Vector3 _openScale = new Vector3();
    /// <summary>
    /// Returns true when the timer reaches openTime
    /// </summary>
    public bool IsOpen => t_openTimer == _openTime;
    /// <summary>
    /// The deadzone around the center of the touchpad that will not cause the weapon selector to select a weapon
    /// </summary>
    [Tooltip("The deadzone around the center of the touchpad that will not cause the weapon selector to select a weapon")]
    [Range(0, 1)]
    public float m_selectDeadZone = 0.3f;
    /// <summary>
    /// The index of the selected weapon
    /// </summary>
    private int _selectedWeaponIndex = 0;
    /// <summary>
    /// THESE ARE NOT MEANT TO BE GAMEOBJECTS. THIS WAS DONE SO I CAN PROGRAM THE REST OF IT.
    /// </summary>
    [HideInInspector]
    public Weapon[] m_weapons = new Weapon[0];
    /// <summary>
    /// Stores the scale of the UI
    /// </summary>
    private void Start()
    {
        _openScale = transform.localScale;
        //The first update cycle will set the scale to 0
    }
    /// <summary>
    /// Check for inputs
    /// </summary>
    private void Update()
    {   //If we get a touch, open the selector
        if (OGInputGetter.Get(OGInputGetter.OculusInputs.FaceTouch))
            OpenSelector();
        //If the user is no longer touching the touchPad, close the selector
        else
            CloseSelector();
        //If the selector is open, select a weapon
        if (IsOpen)
            SetSelectWeapon();
    }
    /// <summary>
    /// Called when the selector is opening
    /// </summary>
    public void OpenSelector()
    {
        //Increment the scale timer
        t_openTimer += Time.deltaTime;
        //Clamp the time to never exceed openTime
        if (t_openTimer > _openTime)
            t_openTimer = _openTime;
        //Scale the UI accordingly
        transform.localScale = Vector3.Lerp(Vector3.zero, _openScale, t_openTimer / _openTime);
    }
    /// <summary>
    /// Called when the selector is closed
    /// </summary>
    public void CloseSelector()
    {   //If the UI is open and the selected weapon is valid
        //This will only be able to be called the first time CloseSelector is called after the Selector fully opens
        if (IsOpen && _selectedWeaponIndex < m_weapons.Length)
            //Call the OnChangeWeapon event
            OnChangeWeapon.Invoke(m_weapons[_selectedWeaponIndex]);
        //Decrement the scale timer
        t_openTimer = Mathf.Clamp(t_openTimer - Time.deltaTime, 0, _openTime);
        //Clamp the timer to never exceed 0
        if (t_openTimer < 0)
            t_openTimer = 0;
        //Scale the UI accordingly
        transform.localScale = Vector3.Lerp(Vector3.zero, _openScale, t_openTimer / _openTime);
    }
    /// <summary>
    /// Sets _selectedWeapon to the weapon that is being selected by the touchPad
    /// </summary>
    public void SetSelectWeapon()
    {   //If there are no weapons, return
        if (m_weapons.Length == 0)
            return;
        //When we get a touch from the touch pad, we convert it to an angle
        Vector2 touch = OGInputGetter.GetTouchPad();
        //Make sure we meet the deadZone requirements
        if (touch.magnitude < m_selectDeadZone)
            return;
        //Each weapon is assigned an angle range
        //equal to 360 / number of weapons
        float weapAng = 360 / m_weapons.Length;
        //We also rotate everything such that up on the touchpad is 0 degrees
        float touchAng = Mathf.Atan2(touch.x, touch.y) * Mathf.Rad2Deg;
        //Then calculate which weapon the angle fits within.
        float start = -weapAng / 2;
        //Check if the touchAngle is within the range of the first weapon
        if (touchAng > 360 + start || touchAng < -start)
        {
            _selectedWeaponIndex = 0;
            return;
        }
        //Increment the angle
        start += weapAng;
        //We start at 1 because we have to do a special check for the first weapon
        for (int i = 1; i < m_weapons.Length; i++)
        {   //Check if the touchAngle is within the range for this weapon
            if (touchAng > start && touchAng <= start + weapAng)
            {   //We have found the selected weapon so break
                _selectedWeaponIndex = i;
                break;
            }
            //Increment the angle
            start += weapAng;
        }
    }
}
