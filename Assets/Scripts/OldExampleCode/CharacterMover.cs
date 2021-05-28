using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Transform VrCamera = null;

    public float speed = 3;

    void Update()
    {
        Vector3 forward = VrCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector2 touchDir = OGInputGetter.GetTouchPad();
        touchDir.Normalize();
        //Rotate the direction of forward by touchDir
        float x, y;
        x = touchDir.x;
        y = touchDir.y;

        Vector3 forwardRight = new Vector3(forward.z, 0, -forward.x);

        forward = (forward * y) + (forwardRight * x);

        transform.position += forward * speed * Time.deltaTime;
    }
}
