using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    public string formation = "";

    [SerializeField]
    public List<Vector3> spawns = new List<Vector3>();

    [SerializeField]
    public GameObject spawnObject;

    [SerializeField]
    public bool visible = true;
    public List<GameObject> Spawn(Vector3 wavePos)
    {
        List<GameObject> spawnList = new List<GameObject>();
        foreach (Vector3 pos in spawns)
        {
            GameObject newInstance = Instantiate(spawnObject, null);
            newInstance.transform.position = wavePos + pos;
            spawnList.Add(newInstance);
        }

        return spawnList;
    }
}
