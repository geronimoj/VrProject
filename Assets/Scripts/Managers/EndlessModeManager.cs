using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeManager : MonoBehaviour
{
    /// <summary>
    /// The waves that can be chosen for spawning
    /// </summary>
    private Wave[] waveOptions = null;
    /// <summary>
    /// Gets all the waves
    /// </summary>
    private void Start()
    {   //Get the wave manager
        GameObject obj = GameObject.Find("WaveManager");
        //If we found it, get the waves from it
        if (obj)
            waveOptions = obj.GetComponents<Wave>();
        //If we couldn't find the waves, log an error.
        else
            Debug.LogError("EndlessModeManager: Could not find waves.");
        //When we win, spawn more waves
        GameManager.s_instance.OnWin.AddListener(SpawnRandomWave);
        //Pause wave spawning
        Wave.paused = true;
    }
    /// <summary>
    /// Core spawning loop
    /// </summary>
    private void Update()
    {   //Make sure we have waves before continuing
        if (waveOptions == null)
            return;
    }

    private void SpawnRandomWave()
    {   //Make sure wave spawning is paused
        Wave.paused = true;
    }
}
