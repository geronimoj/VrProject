using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BezRunner : MonoBehaviour
{
    bool loop = false;
    bool forward = true;
    float timer = 0;
    [HideInInspector]
    public BezPath path;
    Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;

        if (!path)
        {
            var pathLocal = GetComponent<BezPath>();
            var pathRef = GetComponent<BezPathByRef>();
            path = pathLocal ? pathLocal : pathRef.LookUpPath();
        }
    }
    private void Update()
    {   //Make sure there is a path and the wave is not paused
        if (path && !Wave.paused)
        {
            if (forward)
                timer += Time.deltaTime;
            else
                timer -= Time.deltaTime;

            transform.position = startPos+path.GetPos(timer);
        }
    }
}
