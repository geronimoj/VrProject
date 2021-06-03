﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Health
{
    /// <summary>
    /// The enemies main weapon
    /// </summary>
    [Tooltip("The enemies main weapon")]
    [SerializeField]
    protected Weapon _mainWeapon = null;
    /// <summary>
    /// The points from which the enemies weapon fires
    /// </summary>
    [Tooltip("The points from which the enemies weapon fires")]
    [SerializeField]
    protected List<Transform> _weaponFirePoints = new List<Transform>();
    /// <summary>
    /// The maximum time between each burst of shots
    /// </summary>
    [Tooltip("The maximum time between each burst of shots being fired")]
    [SerializeField]
    protected float _maxTimeBetweenShots = 10;
    /// <summary>
    /// The minimum time between each burst of shots being fired
    /// </summary>
    [Tooltip("The minimum time between each burst of shots being fired")]
    [SerializeField]
    protected float _minTimeBetweenShots = 5;
    /// <summary>
    /// The timer for each shot
    /// </summary>
    private float t_shotTimer = 0;
    /// <summary>
    /// The time until the next shot
    /// </summary>
    private float _shotTime = 0;
    /// <summary>
    /// The time a burst occurs for
    /// </summary>
    private float t_burstTimer = 0;
    /// <summary>
    /// The duration of a burst from the enemies main weapon
    /// </summary>
    [Tooltip("The duration of a burst from the enemies main weapon")]
    [SerializeField]
    protected float _burstDuration = 1;
    /// <summary>
    /// Called every time a shot is fired
    /// </summary>
    [Tooltip("Called every time a shot is fired. Called once when the main weapon is fired")]
    public UnityEvent OnFire;
    /// <summary>
    /// Have the targets been setup
    /// </summary>
    private static bool s_setupTargets = false;
    /// <summary>
    /// The targets the enemies can shoot at
    /// </summary>
    private List<Transform> s_targets = new List<Transform>();

    private void Awake()
    {   //If the targets have not been setup, setup the targets
        if (!s_setupTargets)
        {
            s_setupTargets = true;
            //Get the targets for the enemy to shoot at
            GameObject[] objects = GameObject.FindGameObjectsWithTag("PlayerShipTarget");
            //Store those targets for all the enemies.
            foreach (GameObject obj in objects)
                s_targets.Add(obj.transform);
        }
    }
    /// <summary>
    /// Creates a local instance of the main weapon to avoid multiple enemies using the same Object
    /// </summary>
    private void Start()
    {   //Create our own instance of the main weapon
        _mainWeapon = Instantiate(_mainWeapon);
        _mainWeapon.OnEquip();
        //Set this enemy to be tracked by the radar and reticle system
        RadarSystem.s_instance.TrackEnemy(transform);
        EnemyReticleSystem.s_instance.TrackEnemy(transform);
    }
    /// <summary>
    /// Destroy our main weapon to avoid memory leak because I think they can be retained.
    /// </summary>
    private void OnDestroy()
    {   //Destory our instance of our main weapon
        Destroy(_mainWeapon);
    }
    /// <summary>
    /// Fires the main weapon when the time is met
    /// </summary>
    private void FixedUpdate()
    {   //Increment the timers
        t_shotTimer += Time.fixedDeltaTime;
        t_burstTimer += Time.fixedDeltaTime;
        //Check if we can fire another burst
        if (t_shotTimer >= _shotTime)
        {   //Reset the timer
            t_shotTimer = 0;
            //Randomize the the firing
            _shotTime = Random.Range(_minTimeBetweenShots, _maxTimeBetweenShots);
            //Reset the burst timer
            t_burstTimer = 0;
            //If we have targets and weapons, point them at a random target
            if (s_targets.Count > 0 && _weaponFirePoints.Count > 0)
            {
                //Get the target to shoot at
                int target = Random.Range(0, s_targets.Count);

                Vector3 toTarget;
                //Loop over our weapons
                foreach (Transform gun in _weaponFirePoints)
                {   //Get vector to the target
                    toTarget = s_targets[target].position - gun.position;
                    //Rotate the gun to look at the target
                    gun.forward = toTarget.normalized;
                }
            }
        }
        //If the burst timer is still active and we can fire again, FIRE THE MAIN LAZER!!!!
        if (t_burstTimer <= _burstDuration && _mainWeapon.CanFire)
        {
            _mainWeapon.Fire(_weaponFirePoints);
            OnFire.Invoke();
        }
    }
    /// <summary>
    /// Updates the main weapon and other stuff
    /// </summary>
    private void Update()
    {   //Update the main weapon
        _mainWeapon.WeaponUpdate();
    }
}