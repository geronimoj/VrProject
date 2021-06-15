using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReticle : Reticle
{
    /// <summary>
    /// The lock on rockets
    /// </summary>
    public LockOnRockets rockets = null;
    /// <summary>
    /// The default texture for the reticle
    /// </summary>
    [Tooltip("The default texture for the reticle")]
    public Texture m_defaultTexture = null;
    /// <summary>
    /// The texture for when the taget is locked by the weapon
    /// </summary>
    public Texture m_targetedTexture = null;

    private Material _targetMat = null;

    private bool _inTarget = false;

    private void Update()
    {   //Make sure both rockets and targets is valid
        if (!rockets || !Target || !_targetMat)
            return;
    }

    protected override void OnAssignTarget()
    {
        if (!rockets)
        {
            Debug.LogError("LockOnRockets not assigned to Reticle.");
            return;
        }
        //Get the render
        Renderer rend = GetComponent<Renderer>();
        //Get the material of the renderer
        if (rend)
        {
            _targetMat = rend.material;
            //Get the default texture
            if (_targetMat)
                m_defaultTexture = _targetMat.mainTexture;
        }

        _inTarget = false;
    }
}
