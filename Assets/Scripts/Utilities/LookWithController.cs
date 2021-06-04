using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Rotates the gameObject it is on to look along or at the point the pointer is looking at
/// </summary>
public class LookWithController : MonoBehaviour
{
    public LayerMask layers;
    /// <summary>
    /// The pointers
    /// </summary>
    [Tooltip("The RightHandAnchor")]
    [SerializeField]
    private Transform _pointer = null;
    /// <summary>
    /// Should it look at the point the controller is pointing at or in the same direction as the controller
    /// </summary>
    [Tooltip("Should it look at the point the controller is pointing at or in the same direction as the controller")]
    [SerializeField]
    private bool _lookAtPoint = true;

    private void Update()
    {
        if (!_pointer)
            return;

        Vector3 forward;

        if (_lookAtPoint 
            && Physics.Raycast(_pointer.position, _pointer.forward, out RaycastHit hit, Mathf.Infinity, layers))
        {

            forward = hit.point - transform.position;
            forward.Normalize();
        }
        else
            forward = _pointer.forward;

        transform.forward = forward;
    }
}
