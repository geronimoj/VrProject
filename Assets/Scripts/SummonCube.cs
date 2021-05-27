using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCube : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject prefab;

    public void SpawnCube()
    {
        Instantiate(prefab, spawnPos.position, Quaternion.identity);
    }
}
