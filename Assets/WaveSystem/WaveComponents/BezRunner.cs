using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BezRunner : MonoBehaviour
{
    public static bool paused = false;
    bool loop = false;
    bool forward = true;
    float timer = 0;
    [HideInInspector]
    public BezPath path;
    Vector3 startPos;
    int currEvent = 0;
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
        if (path && !paused)
        {
            if (forward)
                timer += Time.deltaTime;
            else
                timer -= Time.deltaTime;

            transform.position = startPos+path.GetPos(timer);

            if (currEvent < path.timerList.Length && timer > path.timerList[currEvent])
            {
                path.eventList[currEvent].Invoke();
                currEvent++;
            }
        }
    }
}
