using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterController : MonoBehaviour
{
    [Tooltip("How far away the RightHandAnchor is from its relative 0,0,0")]
    public float m_controllerLength = 0.5f;

    public Transform m_targetPosition = null;

    public Vector3 m_offset;

    private void LateUpdate()
    {
        if (!m_targetPosition)
            return;

        transform.position = m_offset +  m_targetPosition.position + transform.forward * m_controllerLength;
    }
}
