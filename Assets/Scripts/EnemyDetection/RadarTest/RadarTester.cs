using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarTester : MonoBehaviour
{
    public RadarSystem radar = null;

    public Transform[] enemies = null;

    void Start()
    {
#if UNITY_STANDALONE
        Debug.LogError("This script should not be running");
#endif
        foreach (Transform t in enemies)
            radar.TrackEnemy(t);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
