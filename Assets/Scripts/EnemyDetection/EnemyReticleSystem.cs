using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReticleSystem : MonoBehaviour
{
    [System.Serializable]
    public enum RectileShape
    {
        Quad = 0,
        Circle
    }
    /// <summary>
    /// The shape of the reticle system. Only draws reticles within the shape.
    /// If the shape is circle, compare against radius
    /// </summary>
    [Tooltip("The shape of the reticle system. Only draws reticles within the shape")]
    [SerializeField]
    protected RectileShape _shape = RectileShape.Quad;

    protected float _radius = 0;
}
