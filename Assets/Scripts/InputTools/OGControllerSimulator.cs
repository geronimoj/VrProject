using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGControllerSimulator : MonoBehaviour
{
    [SerializeField]
    private Transform _pointer = null;

    [Tooltip("The rotation speed in degrees")]
    [SerializeField]
    private float _rotateSpeed = 0;

    public float _startRotate = 0.05f;

    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(this);
        return;
#else
        if (!_pointer)
            Debug.LogError("Pointer for Controller Simulator not assigned. Cannot simulate controller with mouse.");
        else
            Cursor.lockState = CursorLockMode.Confined;
#endif
    }

    void Update()
    {   //We don't need to worry about this running on Oculus because the awake function covers it
        if (!_pointer)
            return;

        Ray screen = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Rotate this object if the pointer moves to the edge of the screen
        //If we are on the left side, rotate left
        if (Input.mousePosition.x < Screen.width * _startRotate)
        {
            Vector3 angle = transform.eulerAngles;
            angle.y -= _rotateSpeed * Time.deltaTime;
            transform.eulerAngles = angle;
        }
        //If we are on the right side, rotate right
        else if (Input.mousePosition.x > Screen.width * (1 - _startRotate))
        {
            Vector3 angle = transform.eulerAngles;
            angle.y += _rotateSpeed * Time.deltaTime;
            transform.eulerAngles = angle;
        }
        //Repeat for y
        if (Input.mousePosition.y < Screen.height * _startRotate)
        {
            Vector3 angle = transform.eulerAngles;
            angle.x += _rotateSpeed * Time.deltaTime;
            transform.eulerAngles = angle;
        }
        else if (Input.mousePosition.y > Screen.height * (1 - _startRotate))
        {
            Vector3 angle = transform.eulerAngles;
            angle.x -= _rotateSpeed * Time.deltaTime;
            transform.eulerAngles = angle;
        }

        //If we hit nothing, return
        if (!Physics.Raycast(screen, out RaycastHit hit))
            return;
        //Rotate the pointer to look at the target location
        _pointer.rotation = Quaternion.LookRotation(hit.point - _pointer.position, Vector3.up);
    }
}
