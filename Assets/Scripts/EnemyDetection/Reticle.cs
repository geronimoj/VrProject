using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    /// <summary>
    /// The reticles target
    /// </summary>
    [SerializeField]
    private Transform target;
    /// <summary>
    /// For getting the target
    /// </summary>
    protected Transform Target
    {
        get => target;
    }
    /// <summary>
    /// Set the target. 
    /// </summary>
    /// <param name="t">The target</param>
    public void SetTarget(Transform t)
    {
        target = t;
        OnAssignTarget();
    }
    /// <summary>
    /// Dissables the target
    /// </summary>
    private void OnDisable()
    {
        OnLoseTarget();
        target = null;
    }
    /// <summary>
    /// Called when the target is assigned
    /// </summary>
    protected virtual void OnAssignTarget() { }
    /// <summary>
    /// Called when the target is lost
    /// </summary>
    protected virtual void OnLoseTarget() { }
}
