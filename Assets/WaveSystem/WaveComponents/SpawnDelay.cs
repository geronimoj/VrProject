using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDelay : MonoBehaviour
{
    public float delay = 0.0f;

    private void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<MeshCollider>().enabled = false;
        gameObject.GetComponent<BezRunner>().enabled = false;
    }
    private void Update()
    {
        delay -= Time.deltaTime;
        if (delay < 0)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshCollider>().enabled = true;
            gameObject.GetComponent<BezRunner>().enabled = true;
            gameObject.AddComponent<DeleteComponent>().componentReference = this;
        }
    }
}
