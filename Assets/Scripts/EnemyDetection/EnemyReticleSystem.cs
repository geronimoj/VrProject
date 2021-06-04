using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The system that controls how the reticles that surround enemies appear
/// </summary>
public class EnemyReticleSystem : ReticleDisplayer
{
    public static EnemyReticleSystem s_instance = null;
    /// <summary>
    /// The additional displays to display enemies from
    /// </summary>
    [Tooltip("The additional displays to display enemies from")]
    [SerializeField]
    protected ReticleDisplayer[] _additionalDisplays = new ReticleDisplayer[0];
    /// <summary>
    /// Should the ReticleSystem be used as a display
    /// </summary>
    [Tooltip("Should the ReticleSystem be used as a display")]
    [SerializeField]
    private bool _useAsDisplay = true;
    /// <summary>
    /// Should the ReticleSystem use its stats on all additional displays
    /// </summary>
    [Tooltip("Should the ReticleSystem use its stats on all additional displays")]
    [SerializeField]
    private bool _changeDisplaysToThis = true;
    /// <summary>
    /// The enemies without assigned reticles that we need to track
    /// </summary>
    private readonly List<Transform> _enemiesToTrack = new List<Transform>();
    /// <summary>
    /// The enemies that need to be removed from _assignedReticles
    /// </summary>
    private readonly List<Transform> _enemiesToRemove = new List<Transform>();
    /// <summary>
    /// Store an instance of this system
    /// </summary>
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
    public override void UpdateReticles()
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
        }
        //Remove any enemies that should be removed
        while (_enemiesToRemove.Count > 0)
        {   //Remove it from the reticle system
            _assignReticles.Remove(_enemiesToRemove[0]);
            //And remove the enemy from the enemies we need to remove
            _enemiesToRemove.RemoveAt(0);
        }
        //Update their position and scale
        base.UpdateReticles();
    }
    /// <summary>
    /// Tells each of the reticle displayers to stop displaying a reticel.
    /// </summary>
    /// <param name="enemy">The target enemy</param>
    /// <param name="leaveImmediately">Should the enemy be removed immedaitely</param>
    public override void LeaveReticleView(Transform enemy, bool leaveImmediately = true)
    {   //Should use the reticle system as a display
        if (_useAsDisplay)
        {
            base.LeaveReticleView(enemy, leaveImmediately);
            //If the enemy is not being removed immediately, put them in the toRemove list
            if (!leaveImmediately)
                _enemiesToRemove.Add(enemy);
            //Start tracking the enemy
            _enemiesToTrack.Add(enemy);
        }
        //Tell each of the displayers to stop displaying this reticle
        foreach (ReticleDisplayer display in _additionalDisplays)
            //Null catch
            if (display)
                display.LeaveReticleView(enemy, true);
    }
    /// <summary>
    /// Tells each of the reticle displayers to display a reticle and only display using this system is useAsDisplay is true
    /// </summary>
    /// <param name="enemy">The enemy to display</param>
    public override void EnterReticleView(Transform enemy)
    {   //Should we use the reticle system as a display
        if (_useAsDisplay)
        {
            base.EnterReticleView(enemy);
            //Remove the enemy from being tracked
            if (_enemiesToTrack.Contains(enemy))
                _enemiesToTrack.Remove(enemy);
        }
        //Tell each of the displayers to start displaying reticles for this enemy
        foreach (ReticleDisplayer display in _additionalDisplays)
            //Null catch
            if (display)
                display.EnterReticleView(enemy);
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
