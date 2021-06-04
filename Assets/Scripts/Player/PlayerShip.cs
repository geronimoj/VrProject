using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// The main player controller
/// </summary>
public class PlayerShip : Health
{
    /// <summary>
    /// An event to be called when health hits a specific value
    /// </summary>
    [System.Serializable]
    public struct HealthEvent
    {
        /// <summary>
        /// The value to call the event at
        /// </summary>
        [Tooltip("The min health value at which the event will be called")]
        public float healthValue;
        /// <summary>
        /// Should the event be called once
        /// </summary>
        [Tooltip("Should the event be called once")]
        public bool callOnce;
        /// <summary>
        /// The events to call
        /// </summary>
        public UnityEvent OnHealthReach;
        /// <summary>
        /// Has the event already been called
        /// </summary>
        [HideInInspector]
        public bool called;
    }
    /// <summary>
    /// Array of events that should occur when the health hits a specific threshold
    /// </summary>
    [Tooltip("The events that occur at the specified health values. These are called whenever the player takes damages")]
    public HealthEvent[] healthEvents = new HealthEvent[0];
    /// <summary>
    /// Sets up the health event calls
    /// </summary>
    protected override void Start()
    {
        OnTakeDamage.AddListener(CallHealthEvents);

        base.Start();
    }
    /// <summary>
    /// Calls the HealthEvents
    /// </summary>
    private void CallHealthEvents()
    {   //Loop over the health events
        for(int i =0; i < healthEvents.Length; i++)
        {   //If the health event should only be called once and has already been called, skip
            if (healthEvents[i].callOnce && healthEvents[i].called)
                continue;
            //Otherwise check if we meet the health threshold
            if (currentHealth <= healthEvents[i].healthValue)
            {   //Call the event
                healthEvents[i].OnHealthReach.Invoke();
                //State that the event has been called for callOnce
                healthEvents[i].called = true;
            }
        }
    }
    /// <summary>
    /// Resets the events so they can be called again
    /// </summary>
    public void ResetEvents()
    {   //Loop over the health events and set them to not be called anymore
        for (int i = 0; i < healthEvents.Length; i++)
            healthEvents[i].called = false;
    }
}
