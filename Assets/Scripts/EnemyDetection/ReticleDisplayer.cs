using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleDisplayer : MonoBehaviour
{
    /// <summary>
    /// The shape of the reticle
    /// </summary>
    [System.Serializable]
    public enum ReticleShape
    {
        Quad = 0,
        Circle
    }

    public LayerMask m_quadLayers = 0;
    /// <summary>
    /// The transform the raycasts should perform from when determining if an enemy is in view
    /// </summary>
    [Tooltip("The transform the raycasts should perform from when determining if an enemy is in view. Imagine firing a ray from this transform to the enemy, if it hits the quad, draw a reticle")]
    [SerializeField]
    protected Transform _reticleOrigin = null;
    /// <summary>
    /// The shape of the reticle system. Only draws reticles within the shape.
    /// If the shape is circle, compare against radius
    /// </summary>
    [Tooltip("The shape of the reticle system. Only draws reticles within the shape")]
    [SerializeField]
    protected ReticleShape _shape = ReticleShape.Quad;
    /// <summary>
    /// The shape of the reticle
    /// </summary>
    public ReticleShape Shape => _shape;
    /// <summary>
    /// The radius of the reticle system when the shape is circle
    /// </summary>
    [Tooltip("The radius of the reticle system when the shape is circle")]
    [SerializeField]
    protected float _radius = 0;
    /// <summary>
    /// The radius of the reticle system when the shape is circle
    /// </summary>
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            ReSize();
        }
    }
    /// <summary>
    /// Should the reticles look at the player
    /// </summary>
    [SerializeField]
    protected bool _billboardReticles = false;
    /// <summary>
    /// The prefab for the reticle
    /// </summary>
    [Tooltip("The reticles prefab")]
    [SerializeField]
    protected GameObject _reticlePrefab = null;
    /// <summary>
    /// The scale of the reticles
    /// </summary>
    [Tooltip("The size of the reticles")]
    [SerializeField]
    protected float _reticleScale = 1;
    /// <summary>
    /// The reticles with assigned enemies
    /// </summary>
    protected readonly Dictionary<Transform, Transform> _assignReticles = new Dictionary<Transform, Transform>();
    /// <summary>
    /// The unassigned reticles
    /// </summary>
    protected static readonly List<Transform> s_reticles = new List<Transform>();
    /// <summary>
    /// Rescales the quad to be the correct radius
    /// </summary>
    public void ReSize()
    {
        transform.localScale = new Vector3(_radius * 2, _radius * 2, _radius * 2);
    }
    /// <summary>
    /// Re-sizes all the reticles
    /// </summary>
    public void ReSizeReticle()
    {
        foreach (Transform enemy in _assignReticles.Values)
            enemy.localScale = new Vector3(_reticleScale, _reticleScale, _reticleScale);
    }
    /// <summary>
    /// Update the reticles
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR
        if (_shape == ReticleShape.Circle)
            ReSize();
#endif
        UpdateReticles();
    }
    /// <summary>
    /// Updates the reticles position and scale
    /// </summary>
    public virtual void UpdateReticles()
    {   //Loop over the enemies and update their reticles
        foreach (Transform enemy in _assignReticles.Keys)
        {   //Get a vector to the enemies
            Vector3 camToEnemy = enemy.position - _reticleOrigin.position;
            //Check if the enemy is in view                                                                                                                 
            if (!Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, m_quadLayers))
                //If we don't hit the quad, continue
                continue;
            //Otherwise, update the positions of the reticles
            _assignReticles[enemy].position = hit.point - transform.forward * 0.01f;
            //If the reticles are billboarded, rotate them to the camera
            if (_billboardReticles)
                _assignReticles[enemy].rotation = Quaternion.LookRotation(camToEnemy.normalized);
            else
                _assignReticles[enemy].rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        //Update the reticle scale
        ReSizeReticle();
    }
    /// <summary>
    /// Call to force an enemy to gain a reticle
    /// </summary>
    /// <param name="enemy">The enemy to give the reticle for</param>
    public virtual void EnterReticleView(Transform enemy)
    {   //If the enemy alerady has a reticle, return
        if (_assignReticles.ContainsKey(enemy))
            return;
        //Get a vector from the reticles origin to the enemy
        Vector3 camToEnemy = enemy.position - _reticleOrigin.position;
        //Raycast to the enemy onto the reticle quad. It is assumed that this will succeed
        if (!Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, m_quadLayers))
        {   //If the raycast fails, log an error
            Debug.LogError("Raycast failed to hit reticle quad for enemy " + enemy.name + ". Try using TrackEnemy instead.");
            return;
        }
        //Check if we can re-use a reticle or have to create a new one
        if (s_reticles.Count > 0)
        {   //Re-use a reticle
            _assignReticles.Add(enemy, s_reticles[0]);
            //Re-enable the reticle
            s_reticles[0].gameObject.SetActive(true);
            //Set the rotation to be the same as the quads
            s_reticles[0].rotation = transform.rotation;
            //Set the position of the reticle
            s_reticles[0].position = hit.point;
            //Remove the re-used reticle
            s_reticles.RemoveAt(0);
        }
        //Create a new one
        else
        {   //Create the reticle
            GameObject reticle = Instantiate(_reticlePrefab, hit.point, transform.rotation);
            //Scale the reticle
            reticle.transform.localScale = new Vector3(_reticleScale, _reticleScale, _reticleScale);
            //Store the reticle
            _assignReticles.Add(enemy, reticle.transform);
        }
    }
    /// <summary>
    /// Stop the enemy from having a reticle
    /// </summary>
    /// <param name="enemy">The enemy to remove the reticle of</param>
    /// <param name="leaveImmediately">Should it be removed immediately. For getting around foreach iteration</param>
    public virtual void LeaveReticleView(Transform enemy, bool leaveImmediately = true)
    {   //Make sure the enemy has a reticle
        if (!_assignReticles.ContainsKey(enemy))
            return;
        //If its already dead, give up
        if (!_assignReticles[enemy])
            return;
        //Store the reticle
        s_reticles.Add(_assignReticles[enemy]);
        //Disable the reticle
        _assignReticles[enemy].gameObject.SetActive(false);
        //This is done to get around foreach iteration
        if (leaveImmediately)
            _assignReticles.Remove(enemy);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Debugging information
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}
