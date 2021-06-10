using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Centers the controller on a point with an offset
/// </summary>
public class CenterController : MonoBehaviour
{
    /// <summary>
    /// The distance from the center the controller is
    /// </summary>
    [Tooltip("How far away the RightHandAnchor is from its relative 0,0,0")]
    public float m_controllerLength = 0.5f;
    /// <summary>
    /// The target location that represents where the controller should circle around
    /// </summary>
    public Transform m_targetPosition = null;
    /// <summary>
    /// A fixed offset from targetPosition in global units
    /// </summary>
    public Vector3 m_offset;
#if !UNITY_EDITOR
    /// <summary>
    /// Update the position of the controllres anchor
    /// </summary>
    private void LateUpdate()
    {
        //Make sure we have a position
        if (!m_targetPosition)
            return;
        //Set the position
        transform.position = m_offset +  m_targetPosition.position + transform.forward * m_controllerLength;
    }
#endif
}
