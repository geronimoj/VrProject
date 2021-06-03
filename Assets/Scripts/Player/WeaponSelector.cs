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
    /// The weapons that can be chosen from using the choose wheel
    /// </summary>
    public Weapon[] m_weapons = new Weapon[0];
    /// <summary>
    /// The radius of the imaginary circle of which the icons are laid on
    /// </summary>
    [Tooltip("The distance from the center each icon is positioned")]
    [SerializeField]
    protected float _iconRadius = 1;
    /// <summary>
    /// Is the selector currently open
    /// </summary>
    private bool _isOpen = false;
    /// <summary>
    /// The images for each weapon
    /// </summary>
    private readonly List<GameObject> _weaponImageUI = new List<GameObject>();
    /// <summary>
    /// The prefab the weapons image is put on
    /// </summary>
    [Tooltip("The prefab the weapons image is put on")]
    [SerializeField]
    private GameObject _weaponImagePrefab = null;
    /// <summary>
    /// The canvas the weapon images are put on
    /// </summary>
    [Tooltip("The canvas the weapon images are put on")]
    [SerializeField]
    public Transform _canvas = null;
    /// <summary>
    /// The object the is used to point out the direction the user is pointing on the track pad
    /// </summary>
    [Tooltip("The object the is used to point out the direction the user is pointing on the track pad")]
    public Transform _circlePointer = null;
    /// <summary>
    /// Stores the scale of the UI
    /// </summary>
    private void Start()
    {
        if (!_weaponImagePrefab)
        {
            Debug.LogError("Weapon Image prefab not set on WeaponSelector.");
            Debug.Break();
        }
        if (!_canvas)
        {
            Debug.LogError("Canvas not set on WeaponSelector.");
            Debug.Break();
        }
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
        if (!_isOpen)
        {
            float weapAng = 360 / m_weapons.Length;
            float curAngle = 90;
            //Loop over the weapons and create their UI
            for (int i = 0; i < m_weapons.Length; i++)
            {
                GameObject ui = Instantiate(_weaponImagePrefab, _canvas);
                Image image = ui.GetComponent<Image>();
                //Set the sprite of the weapon
                if (image)
                    image.sprite = m_weapons[i].weaponIcon;
                else
                    Debug.LogError("Weapon Image Prefab on Weapon Selector does not contain an Image");
                //Get the rect transform for position manipulation
                RectTransform rt = image.rectTransform;
                //Set the image to be anchored in the center of the canvas
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                //Set the position of the UI
                Vector2 pos = new Vector2(Mathf.Cos(curAngle * Mathf.Deg2Rad), Mathf.Sin(curAngle * Mathf.Deg2Rad));
                rt.anchoredPosition = pos.normalized * _iconRadius;
                //Store the weapon image
                _weaponImageUI.Add(ui);

                curAngle += weapAng;
            }
            //Spawn the weapon UI
            _isOpen = true;
        }
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
    {   //Once the UI has finished dissapearing
        if (t_openTimer == 0)
        {
            //Destroy the weapon UI
            while (_weaponImageUI.Count > 0)
            {
                Destroy(_weaponImageUI[0]);
                _weaponImageUI.RemoveAt(0);
            }
            _isOpen = false;
        }
        //If the UI is open and the selected weapon is valid
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

        if (_circlePointer)
        {
            Vector3 euler = _circlePointer.eulerAngles;
            euler.z = touchAng;
            _circlePointer.eulerAngles = euler;
        }
        //Its currently in range of -180 to 180. Its nicer for it to be in 0 - 360
        if (touchAng < 0)
            touchAng += 360;
        //Then calculate which weapon the angle fits within.
        float start = -weapAng / 2;
        //Check if the touchAngle is within the range of the first weapon
        if (touchAng > 360 + start || touchAng < -start)
        {
            _selectedWeaponIndex = 0;
            Debug.Log("First Weapon");
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
