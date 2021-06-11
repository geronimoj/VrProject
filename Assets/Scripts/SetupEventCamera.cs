using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupEventCamera : MonoBehaviour
{
    public Canvas canvas = null;

    void Start()
    {   //Make sure we have a reference to a canvas
        if (!canvas)
            canvas = GetComponent<Canvas>();

        if (canvas)
            //Set the event camera
            canvas.worldCamera = Camera.main;
    }
}
