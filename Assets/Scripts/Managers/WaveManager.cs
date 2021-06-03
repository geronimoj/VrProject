using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages when waves should spawn
/// </summary>
public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// A wave
    /// </summary>
    [System.Serializable]
    internal struct Wave
    {
        /// <summary>
        /// The prefab for the wave
        /// </summary>
        public GameObject wavePrefab;
        /// <summary>
        /// The time at which the wave should spawn
        /// </summary>
        [Tooltip("The time at which the wave should spawn")]
        public float spawnTime;
        /// <summary>
        /// The event called when the wave starts
        /// </summary>
        public UnityEngine.Events.UnityEvent OnWaveStart;
        /// <summary>
        /// Has the wave already been spawned
        /// </summary>
        [HideInInspector]
        public bool spawed;
    }
    /// <summary>
    /// A global timer, this aint ever going to end, don't worry.
    /// </summary>
    private float globalTime = 0;
    /// <summary>
    /// The waves that should be spawned
    /// </summary>
    [SerializeField]
    private WaveManager.Wave[] waves = new Wave[0];
    /// <summary>
    /// Spawns the waves when their spawn time is met
    /// </summary>
    private void Update()
    {   //Increment the global timer
        globalTime += Time.deltaTime;
        //Loop over the waves
        for (int i = 0; i < waves.Length; i++)
            //If they meet the timer and the wave has not spawned, spawn it
            if (!waves[i].spawed && globalTime > waves[i].spawnTime)
            {   //Spawn the wave at 0, 0, 0
                Instantiate(waves[i].wavePrefab, Vector3.zero, Quaternion.identity);
                //Start the event for the wave starting
                waves[i].OnWaveStart.Invoke();
                //Set the wave to have spawned
                waves[i].spawed = true;
            }
    }
}
