using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class PhysicsObjectPusher : MonoBehaviour
{
    public Transform pointer = null;
    //public float force = 100;
    //private float _force = 100;
    //private bool push = true;
    public LineRenderer lRend = null;

    private void Start()
    {
        if (!lRend)
            lRend = gameObject.AddComponent<LineRenderer>(); 
        //push = true;
    }

    void LateUpdate()
    {
        if (!pointer)
        {
            Debug.LogError("Pointer transform not set");
            return;
        }

        Ray ray = new Ray(pointer.position, pointer.forward);

        if (lRend)
            lRend.SetPosition(0, ray.origin);

        if (/*OGInputGetter.Get(OGInputGetter.OculusInputs.BackTrigger) && */Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Enemy", "EnemyProjectile")))
        {
        //    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        //    //SHUT UNITY, JUST USE NULL PROPAGATION
        //    if (rb)
        //        rb.AddForce(_force * ray.direction);

            if (lRend)
                lRend.SetPosition(1, hit.point);
        }
        //If the raycast misses, set the end point to be far away
        else
            lRend.SetPosition(1, ray.origin + 100 * ray.direction);
    }

    public void TogglePushDirection()
    {
        //push = !push;

        //_force = push ? force : -force;
    }
}
