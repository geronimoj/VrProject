using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how the radar works and displays itself
/// </summary>
public class RadarSystem : MonoBehaviour
{
    /// <summary>
    /// The size of the radar
    /// </summary>
    [Tooltip("The size of the radar")]
    [SerializeField]
    private float m_radarRadius = 1;
    /// <summary>
    /// The range of the radar
    /// </summary>
    public float m_radarRange = 10;
    /// <summary>
    /// The size of the radar
    /// </summary>
    public float RadarRadius
    {
        get => m_radarRadius;
        set
        {
            m_radarRadius = value;
            UpdateRadius();
        }
    }
    /// <summary>
    /// The sphere that represents the radar
    /// </summary>
    [Tooltip("The sphere that represents the radar")]
    public GameObject m_radarObject = null;
    /// <summary>
    /// The equivalent point in the world that the center of the radar represents
    /// </summary>
    [Tooltip("The equivalent point in the world that the center of the radar represents")]
    public Transform m_worldReferencePoint = null;
    /// <summary>
    /// The prefab that is spawned to represent the enemies on the radar
    /// </summary>
    [Tooltip("The prefab that is spawned to represent the enemies on the radar")]
    public GameObject m_blipPrefab = null;
    /// <summary>
    /// The size of the enemyPrefabs
    /// </summary>
    [Tooltip("The size of the enemyPrefabs")]
    [SerializeField]
    private float _radarBlipSize = 0.1f;
    /// <summary>
    /// The size of the blips on the radar
    /// </summary>
    public float RadarBlipSize
    {
        get => m_radarRadius;
        set
        {
            m_radarRadius = value;
            UpdateBlipSize();
        }
    }
    /// <summary>
    /// Stores the enemy ships transform as a key to a blip gameObject
    /// </summary>
    private readonly Dictionary<Transform, Transform> _enemyBlips = new Dictionary<Transform, Transform>();
    /// <summary>
    /// Stores all the blips that are not currently being used
    /// </summary>
    private readonly List<Transform> _inactiveBlips = new List<Transform>();

    [SerializeField]
    private List<Transform> _enemiesToTrack = new List<Transform>();

    private readonly List<Transform> _enemiesToRemove = new List<Transform>();
    /// <summary>
    /// Set the scale of the radar object
    /// </summary>
    private void Start()
    {   //Make sure the radar is there and scaled correctly
        if (m_radarObject)
            UpdateRadius();
        else
        {
            Debug.LogError("The radar object has not been assigned");
            Debug.Break();
        }
        //Make sure the blip prefab is set
        if (!m_blipPrefab)
        {
            Debug.LogError("The blip prefab for the radar is not assigned!");
            Debug.Break();
        }
        //Make sure the reference point is set
        if (!m_worldReferencePoint)
        {
            Debug.LogError("The world reference point for the radar is not assigned");
            Debug.Break();
        }
    }
    /// <summary>
    /// Updates the radar
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR
        //If we are in the editor, keep updating the scale for debugging
        UpdateRadius();
#endif
        UpdateBlips();
    }
    /// <summary>
    /// Updates the radius of the sphere
    /// </summary>
    private void UpdateRadius()
    {
        m_radarObject.transform.localScale = new Vector3(m_radarRadius, m_radarRadius, m_radarRadius);
    }
    /// <summary>
    /// Updates the position, rotation and scale of the blips
    /// </summary>
    private void UpdateBlips()
    {
        UpdateBlipSize();
        //Loop over the enemies to track
        for (int i = 0; i < _enemiesToTrack.Count; i++)
        {
            //If they are null or destroyed, remove them
            if (!_enemiesToTrack[i])
            {   //Remove them
                _enemiesToTrack.RemoveAt(i);
                //Reduce the indexer so we don't skip an enemy
                i--;
            }
            //Check if the enemy is close enough
            else if (Vector3.Distance(_enemiesToTrack[i].position, m_worldReferencePoint.position) <= m_radarRange)
                //If they are, add them to the radar
                EnterRadar(_enemiesToTrack[i]);
        }
        //Loop over the enemy transforms and check if any have exited the radius
        foreach (Transform t in _enemyBlips.Keys)
        {
            //Check if they have gone to far away from the radar
            if (Vector3.Distance(t.position, m_worldReferencePoint.position) > m_radarRange)
            {
                //If so, remove them
                LeaveRadar(t, false);
                continue;
            }
            //Otherwise update the position of the blip relative to the radar
            Transform blip = _enemyBlips[t];
            //Set the local position of the blip
            blip.localPosition = (t.position - m_worldReferencePoint.position) / (m_radarRange * 2);
            blip.localPosition *= m_radarRadius;
        }
        //Because we can't remove the enemies from _enemyBlips in the foreach loop, we need to remove them now
        for (int i = 0; i < _enemiesToRemove.Count; i++)
        {   //Now we remove the enemy from the blips
            _enemyBlips.Remove(_enemiesToRemove[0]);
            //And remove the enemy from the enemies we need to remove
            _enemiesToRemove.RemoveAt(0);
        }
    }
    /// <summary>
    /// Updates the scale of each individual blip
    /// </summary>
    private void UpdateBlipSize()
    {   //Scale each of the blips to their correct size
        foreach (Transform t in _enemyBlips.Values)
            t.localScale = new Vector3(_radarBlipSize, _radarBlipSize, _radarBlipSize);
    }
    /// <summary>
    /// Removes the blip from the radar
    /// </summary>
    /// <param name="enemy">The enemy to create it for</param>
    /// <param name="removeImmediately">Should the enemy be removed immediately. This should be used to avoid crashing a foreach iteration</param>
    private void LeaveRadar(Transform enemy, bool removeImmediately = true)
    {   //If they didn't have a blip in the first place, don't remove theirs
        if (!_enemyBlips.ContainsKey(enemy))
            return;
        //Store the now inactive blip
        _inactiveBlips.Add(_enemyBlips[enemy]);
        //Dissable the blip
        _enemyBlips[enemy].gameObject.SetActive(false);
        //Check if the enemies should be removed immediately
        if (removeImmediately)
            //Remove this enemy from the enemy blips
            _enemyBlips.Remove(enemy);
        //Because we can't remove the enemies from _enemyBlips in the foreach loop, we need to remove them later
        else
            _enemiesToRemove.Add(enemy);
        //Keep tracking the enemy even after they leave
        _enemiesToTrack.Add(enemy);
    }
    /// <summary>
    /// Creates a blip on the radar
    /// </summary>
    /// <param name="enemy">The enemy to create it for</param>
    public void EnterRadar(Transform enemy)
    {   //Make sure they are not already in the radar
        if (_enemyBlips.ContainsKey(enemy))
            return;
        //Check if we have a blip we can use
        if (_inactiveBlips.Count > 0)
        {
            //Use the first blip in the list
            _enemyBlips.Add(enemy, _inactiveBlips[0]);
            //Enable the blip
            _inactiveBlips[0].gameObject.SetActive(true);
            //Make sure the scale is up to date
            _inactiveBlips[0].localScale = new Vector3(_radarBlipSize, _radarBlipSize, _radarBlipSize);
            //Remove the blip from blips
            _inactiveBlips.RemoveAt(0);
        }
        else
        {   //Otherwise we need to create a new blip
            GameObject obj = Instantiate(m_blipPrefab);
            //Scale the blip
            obj.transform.localScale = new Vector3(_radarBlipSize, _radarBlipSize, _radarBlipSize);
            //Store the blip
            _enemyBlips.Add(enemy, obj.transform);
        }
        //Remove the enemy from being tracked
        if (_enemiesToTrack.Contains(enemy))
            _enemiesToTrack.Remove(enemy);
    }
    /// <summary>
    /// Adds an enemy to track. Once they leave the radar, they are gone for good
    /// </summary>
    /// <param name="enemy"></param>
    public void TrackEnemy(Transform enemy)
    {
        _enemiesToTrack.Add(enemy);
    }
}
