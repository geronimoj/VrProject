using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The system that controls how the reticles that surround enemies appear
/// </summary>
public class EnemyReticleSystem : ReticleDisplayer
{
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
    /// Should the enemies be continued to be tracked after they have left the view
    /// </summary>
    [Tooltip("Should the ReticleSystem continue to track an enemy after it has been removed from view")]
    public bool _trackRemovedEnemies = true;
    /// <summary>
    /// The enemies without assigned reticles that we need to track
    /// </summary>
    private readonly List<Transform> _enemiesToTrack = new List<Transform>();
    /// <summary>
    /// The enemies that need to be removed from _assignedReticles
    /// </summary>
    private readonly List<Transform> _enemiesToRemove = new List<Transform>();
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
            if (Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, m_quadLayers))
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
            if (!Physics.Raycast(_reticleOrigin.position, camToEnemy.normalized, out RaycastHit hit, camToEnemy.magnitude, m_quadLayers) || (_shape == ReticleShape.Circle && (hit.point - transform.position).magnitude > _radius))
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
        //If this is a display, update the reticles
        if (_useAsDisplay)
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
        }
        else
        {   //Make sure the reticles are not edited in the foreach loop this may or may not be called from
            if (leaveImmediately)
                //Still need to use assignReticles for tracking enemies
                _assignReticles.Remove(enemy);
            //If its been called from the foreach loop, remove it afterwards
            else
                _enemiesToRemove.Add(enemy);
        }
        //Tell each of the displayers to stop displaying this reticle
        foreach (ReticleDisplayer display in _additionalDisplays)
            //Null catch
            if (display)
                display.LeaveReticleView(enemy, true);

        if (_trackRemovedEnemies)
            //Start tracking the enemy
            _enemiesToTrack.Add(enemy);
    }
    /// <summary>
    /// Tells each of the reticle displayers to display a reticle and only display using this system is useAsDisplay is true
    /// </summary>
    /// <param name="enemy">The enemy to display</param>
    public override void EnterReticleView(Transform enemy)
    {   //Should we use the reticle system as a display
        if (_useAsDisplay)
            base.EnterReticleView(enemy);
        else
            //We still need to use _assignedReticles to store which enemies are in view
            _assignReticles.Add(enemy, null);
        //Tell each of the displayers to start displaying reticles for this enemy
        foreach (ReticleDisplayer display in _additionalDisplays)
            //Null catch
            if (display)
                display.EnterReticleView(enemy);
        //Remove the enemy from being tracked
        if (_enemiesToTrack.Contains(enemy))
            _enemiesToTrack.Remove(enemy);
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
    /// <summary>
    /// Stops tracking and removes an enemy from view
    /// </summary>
    /// <param name="enemy">The enemy to stop tracking</param>
    public void StopTracking(Transform enemy)
    {   //If the enemy has a reticle, remove the reticle
        if (_assignReticles.ContainsKey(enemy))
            LeaveReticleView(enemy);
        //If the enemy is being tracked, stop tracking it
        if (_enemiesToTrack.Contains(enemy))
            _enemiesToTrack.Remove(enemy);
    }
}
