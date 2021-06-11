using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Manages when waves should spawn
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Tooltip("For telling it what the extra current scene is. DEBUGGING ONLY")]
    [SerializeField]
    protected string _currentLevel = "";
    #region OLD
    /*
    /// <summary>
    /// The currently level
    /// </summary>
    public Level m_level = Level.Level1;
    /// <summary>
    /// The current difficulty
    /// </summary>
    public Difficulty m_difficulty = Difficulty.Easy;
    /// <summary>
    /// A global timer, this aint ever going to end, don't worry.
    /// </summary>
    private float _levelTime = 0;
    /// <summary>
    /// The index of the wave we are currently looking to spawn
    /// </summary>
    private int _waveIndex = -1;
    /// <summary>
    /// Is the spawning of enemies paused
    /// </summary>
    //Serialise so we can see it in the inspector
    [SerializeField]
    private bool _pause = false;
    /// <summary>
    /// The waves that should be spawned
    /// </summary>
    [Tooltip("The waves that should be spawned")]
    [SerializeField]
    private List<Wave> _waves = new List<Wave>();
    /// <summary>
    /// The paths that can be chosen from. Dictionary for improved indexing
    /// </summary>
    private readonly Dictionary<string, BezPath> _paths = new Dictionary<string, BezPath>();
    /// <summary>
    /// The formations that can be chosen from. Dictionary for improved indexing
    /// </summary>
    private readonly Dictionary<string, Formation> _formations = new Dictionary<string, Formation>();/// <summary>
    /// Logs that waves are starting to be spawned
    /// </summary>
    private void Start()
    {
        Debug.Log("Starting to spawn waves");
    }
    /// <summary>
    /// Spawns the waves when their spawn time is met
    /// </summary>
    private void Update()
    {   //Don't do anything if we are paused
        if (_pause)
            return;
        //Make sure waveIndex is valid
        if (_waveIndex >= 0)
            //Loop over the waves starting from waveIndex and check if any should spawn
            for (int i = _waveIndex; i < _waves.Count; i++)
            {   //Check that the current index is in range, otherwise break
                if (_waves[i].spawnTime > _levelTime + Time.deltaTime)
                    break;
                //Check that the wave is in range. It should be reguardless
                //The wave has to be in range
                if (_waves[i].spawnTime >= _levelTime && _waves[i].spawnTime < _levelTime + Time.deltaTime)
                {   //Spawn the wave
                    _waves[i].Spawn();
                    //Increment the index so we don't spawn this wave again later
                    _waveIndex++;
                }
            }
        //Permanently increate the level timer
        _levelTime += Time.deltaTime;
    }
    /// <summary>
    /// Load the waves for the current difficulty
    /// </summary>
    public void LoadWaves()
    {   //Load the paths and formations
        //Currently just load all the formations and paths in the respective folders
        Formation[] formations = Resources.LoadAll<Formation>("Formations");
        BezPath[] paths = Resources.LoadAll<BezPath>("Paths");
        //Load the paths into the dictionary
        foreach (BezPath path in paths)
        {   //Make sure the key does not already exist
            if (!_paths.ContainsKey(path.pathName))
                _paths.Add(path.pathName, path);
            //Log an error as names are important
            else
                Debug.LogError("Path: " + path.name + " has same path name as: " + _paths[path.pathName].name);
        }
        //Repeat for formations
        foreach (Formation formation in formations)
        {   //Make sure the key does not already exist
            if (!_formations.ContainsKey(formation.formation))
                _formations.Add(formation.formation, formation);
            //Log an error as names are important
            else
                Debug.LogError("Formation: " + formation.name + " has same formation name as: " + _formations[formation.formation].name);
        }
        //Load the waves
        string wavePath;
        Wave[] waves = null;
        //Load all the waves for up to the difficulty.
        //So...
        //Hard = Easy + Normal + Hard
        //Normal = Easy + Normal
        //Easy = Easy
        for (int i = 0; i <= (int)m_difficulty; i++)
        {   //Get the path to the waves we want to load
            wavePath = m_level.ToString() + "/" + (Difficulty)i;
            //Load all of the waves in the folder
            waves = Resources.LoadAll<Wave>(wavePath);
            //Load all of the waves into the list and initialise them
            foreach (Wave wave in waves)
            {   //Initialise the wave giving it direct access to the formations and paths dictionaries
                // if (wave.Initialise(_formations, _paths))
                    //If the wave successfully initialized, store it otherwise ignore it
                    _waves.Add(wave);
                //else
                //  Debug.LogError("Wave failed to initialise");
            }
        }
        //Check that we have waves
        if (_waves.Count > 0)
            _waveIndex = 0;
        else
        {   //Invalidate waveIndex
            _waveIndex = -1;
            return;
        }
        //Sort the waves by when they should spawn so that index 0 is the smallest time
        _waves.Sort(SortByTime);
    }
    /// <summary>
    /// Loads and starts playing a level
    /// </summary>
    /// <param name="level">The level to spawn</param>
    /// <param name="difficulty">The difficulty to spawn</param>
    public void LoadLevel(Level level, Difficulty difficulty)
    {   //Set the level and difficulty
        m_level = level;
        m_difficulty = difficulty;
        //Load the waves
        LoadWaves();
    }
    /// <summary>
    /// For sorting the Waves by spawn time
    /// </summary>
    /// <param name="a">Wave a</param>
    /// <param name="b">Wave b</param>
    /// <returns>Returns -1 if a should spawn before b</returns>
    private int SortByTime(Wave a, Wave b)
    {   //A is less than b
        if (a.spawnTime < b.spawnTime)
            return -1;
        //B is less than A
        if (a.spawnTime > b.spawnTime)
            return 1;
        //They are equal
        return 0;
    }
    /// <summary>
    /// The difficulty
    /// </summary>
    public enum Difficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }*/
    #endregion
    /// <summary>
    /// For resetting the level
    /// </summary>
    public void ResetLevel()
    {   //Reset the level time
        //_levelTime = 0;
        GameObject waveManager = GameObject.Find("WaveManager");
        //If we found it, reset the waves on it
        if (waveManager)
        {   //Get the waves
            Wave[] waves = waveManager.GetComponents<Wave>();
            //Reset the wave
            foreach (Wave wave in waves)
                wave.Reset();
        }
        //Kill the enemies and destroy projectiles
        Enemy.KillEnemies.Invoke();
        Projectile.DestroyProjectiles.Invoke();
        //Kill all active enemies and Wave related events
        Debug.LogError("Reset Level does not kill all projectiles or kill all enemies.");
    }
    /// <summary>
    /// Loads a level and unloads any previous levels
    /// </summary>
    /// <param name="level">The level to load</param>
    public void LoadLevel(string levelSceneName)
    {   //Unload the current level
        UnloadLevel();
        //Load the new level
        SceneManager.LoadScene(levelSceneName, LoadSceneMode.Additive);
        _currentLevel = levelSceneName;
    }
    /// <summary>
    /// Unloads the current level
    /// </summary>
    public void UnloadLevel()
    {
        //Unload the current level
        if (_currentLevel != "")
        {   //Kill the enemies and destroy the projectiles
            Enemy.KillEnemies.Invoke();
            Projectile.DestroyProjectiles.Invoke();
            //Unload the scene
            SceneManager.UnloadSceneAsync(_currentLevel);
            _currentLevel = "";
        }
    }
    /// <summary>
    /// The level to spawn for
    /// </summary>
    public enum Level
    {
        Level1 = 1,
    }
}
