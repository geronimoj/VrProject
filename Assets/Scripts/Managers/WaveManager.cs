using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages when waves should spawn
/// </summary>
public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// A global timer, this aint ever going to end, don't worry.
    /// </summary>
    private float globalTime = 0;
    /// <summary>
    /// The waves that should be spawned
    /// </summary>
    [Tooltip("The waves that should be spawned")]
    [SerializeField]
    private Wave[] waves = new Wave[0];
    /// <summary>
    /// The paths that can be chosen from
    /// </summary>
    private readonly List<BezPath> _paths = new List<BezPath>();
    /// <summary>
    /// The formations that can be chosen from
    /// </summary>
    private readonly List<Formation> _formations = new List<Formation>();
    /// <summary>
    /// Logs that waves are starting to be spawned
    /// </summary>
    private void Start()
    {
        Debug.Log("Starting to spawn waves");
    }
    /// <summary>
    /// Spawns the waves when their spawn time is met
    /// </summary>
    private void Update()
    {   //Increment the global timer
        globalTime += Time.deltaTime;
    }

    private void LoadWavesAndFormations()
    {
        Wave[] waves = Resources.LoadAll<Wave>("Waves");
        BezPath[] paths = Resources.LoadAll<BezPath>("Paths");
        Formation[] formations = Resources.LoadAll<Formation>("Formations");
    }
}
