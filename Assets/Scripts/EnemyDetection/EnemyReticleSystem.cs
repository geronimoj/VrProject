using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The system that controls how the reticles that surround enemies appear
/// </summary>
public class EnemyReticleSystem : MonoBehaviour
{
    public static EnemyReticleSystem s_instance = null;
    /// <summary>
    /// The shape of the reticle
    /// </summary>
    [System.Serializable]
    public enum ReticleShape
    {
        Quad = 0,
        Circle
    }
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

    public ReticleShape Shape => _shape;
    /// <summary>
    /// The radius of the reticle system when the shape is circle
    /// </summary>
    [Tooltip("The radius of the reticle system when the shape is circle")]
    [SerializeField]
    private float _radius = 0;
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
    /// The reticles with assigned enemies
    /// </summary>
    private readonly Dictionary<Transform, Transform> _assignReticles = new Dictionary<Transform, Transform>();
    /// <summary>
    /// The unassigned reticles
    /// </summary>
    private readonly List<Transform> _reticles = new List<Transform>();
    /// <summary>
    /// The enemies without assigned reticles that we need to track
    /// </summary>
    private List<Transform> _enemiesToTrack = new List<Transform>();
    /// <summary>
    /// The enemies that need to be removed from _assignedReticles
    /// </summary>
    private readonly List<Transform> _enemiesToRemove = new List<Transform>();

    private void Awake()
    {
        s_instance = this;
    }
    /// <summary>
    /// Updates the reticles and debugging information
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
    /// Updates the reticle
    /// </summary>
    public void UpdateReticles()
    {   //Update who is in view and how isn't
        //Check if any of the tracked enemies are now in view
        for (int i = 0; i < _enemiesToTrack.Count; i++)
        {   //Remove any destroyed enemies
            if (_enemiesToTrack[i] == null)
            {
                _enemiesToTrack.RemoveAt(i);
                i--;
                continue;
            }
            //This is here from old code because I'm too lazy to rename all enemies from the previously used foreach statement
            Transform enemy = _enemiesToTrack[i];
            //Get a vector from the reticles origin to the enemy
            Vector3 camToEnemy = enemy.position - _reticleOrigin.position;
            //Check if the enemy is in view
            if (Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, LayerMask.GetMask("ReticleQuad")))
            {   //Perform an additional check if its a circle to make sure they are inside
                if (_shape == ReticleShape.Circle && (hit.point - transform.position).magnitude > _radius)
                    //If they are outside, continue
                    continue;
                //Otherwise, enter the view
                EnterReticleView(enemy);
            }
        }
        //Start with the enemies already in view
        foreach (Transform enemy in _assignReticles.Keys)
        {   //Make sure the enemy is still valid
            if (enemy == null)
            {   //Log an error
                Debug.LogError("Enemy destroyed without releasing assigned reticles");
                Debug.Break();
                //Since the key is now invalid, we don't know which reticle to release so all we can do is report an error
                continue;
            }
            //Get a vector from the camera to the enemy
            Vector3 camToEnemy = enemy.position - _reticleOrigin.position;
            //Check if the enemy is in view                                                                                                                 
            if (!Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, LayerMask.GetMask("ReticleQuad")) || (_shape == ReticleShape.Circle && (hit.point - transform.position).magnitude > _radius))
            {   
                //Otherwise remove it from view
                LeaveReticleView(enemy, false);
                continue;
            }
            //Otherwise, update the positions of the reticles
            _assignReticles[enemy].position = hit.point;
            //If the reticles are billboarded, rotate them to the camera
            if (_billboardReticles)
                _assignReticles[enemy].rotation = Quaternion.LookRotation(camToEnemy.normalized);
        }
        //Remove any enemies that should be removed
        while (_enemiesToRemove.Count > 0)
        {   //Remove it from the reticle system
            _assignReticles.Remove(_enemiesToRemove[0]);
            //And remove the enemy from the enemies we need to remove
            _enemiesToRemove.RemoveAt(0);
        }

        ReSizeReticle();
    }
    /// <summary>
    /// Call to force an enemy to gain a reticle
    /// </summary>
    /// <param name="enemy">The enemy to give the reticle for</param>
    public void EnterReticleView(Transform enemy)
    {   //If the enemy alerady has a reticle, return
        if (_assignReticles.ContainsKey(enemy))
            return;
        //Get a vector from the reticles origin to the enemy
        Vector3 camToEnemy = enemy.position - _reticleOrigin.position;
        //Raycast to the enemy onto the reticle quad. It is assumed that this will succeed
        if (!Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, LayerMask.GetMask("ReticleQuad")))
        {   //If the raycast fails, log an error
            Debug.LogError("Raycast failed to hit reticle quad for enemy " + enemy.name + ". Try using TrackEnemy instead.");
            return;
        }
        //Check if we can re-use a reticle or have to create a new one
        if (_reticles.Count > 0)
        {   //Re-use a reticle
            _assignReticles.Add(enemy, _reticles[0]);
            //Re-enable the reticle
            _reticles[0].gameObject.SetActive(true);
            //Set the rotation to be the same as the quads
            _reticles[0].rotation = transform.rotation;
            //Set the position of the reticle
            _reticles[0].position = hit.point;
            //Remove the re-used reticle
            _reticles.RemoveAt(0);
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
        //Remove the enemy from being tracked
        if (_enemiesToTrack.Contains(enemy))
            _enemiesToTrack.Remove(enemy);
    }
    /// <summary>
    /// Stop the enemy from having a reticle
    /// </summary>
    /// <param name="enemy">The enemy to remove the reticle of</param>
    /// <param name="leaveImmediately">Should it be removed immediately. For getting around foreach iteration</param>
    public void LeaveReticleView(Transform enemy, bool leaveImmediately = true)
    {   //Make sure the enemy has a reticle
        if (!_assignReticles.ContainsKey(enemy))
            return;
        //Store the reticle
        _reticles.Add(_assignReticles[enemy]);
        //Disable the reticle
        _assignReticles[enemy].gameObject.SetActive(false);
        //This is done to get around foreach iteration
        if (leaveImmediately)
            _assignReticles.Remove(enemy);
        else
            _enemiesToRemove.Add(enemy);
        //Start tracking the enemy
        _enemiesToTrack.Add(enemy);
    }
    /// <summary>
    /// Start tracking an enemy
    /// </summary>
    /// <param name="enemy">The enemy to track</param>
    public void TrackEnemy(Transform enemy)
    {   //Make sure it does not already exist/being tracked
        if (_assignReticles.ContainsKey(enemy) || _enemiesToTrack.Contains(enemy))
            return;
        //Start tracking that enemy
        _enemiesToTrack.Add(enemy);
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
