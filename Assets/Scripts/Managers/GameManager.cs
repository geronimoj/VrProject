using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
/// Tracks gameover stat and player death
/// </summary>
[RequireComponent(typeof(WaveManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager s_instance = null;
    /// <summary>
    /// Called when you win
    /// </summary>
    public UnityEvent OnWin;
    /// <summary>
    /// Called when you lose
    /// </summary>
    public UnityEvent OnLose;
    /// <summary>
    /// Reference to the WaveManager for loading scenes
    /// </summary>
    [Tooltip("The wave manager")]
    [SerializeField]
    protected WaveManager _waveManager = null;
    /// <summary>
    /// The waves
    /// </summary>
    private List<Wave> _waves = new List<Wave>();
    /// <summary>
    /// The enemies that are alive
    /// </summary>
    private List<Enemy> _enemiesAlive = new List<Enemy>();
    /// <summary>
    /// Is the gameover scene open
    /// </summary>
    private bool _gameOverSceneIsOpen = false;
    /// <summary>
    /// Has the game finished
    /// </summary>
    private static bool _gameIsOver = false;
    /// <summary>
    /// Returns true if the game has finished. This is different from GameHasStarted which tracks when the game begins
    /// </summary>
    public static bool GameIsOver => _gameIsOver;
    /// <summary>
    /// Pauses waves from spawning and enemies from moving
    /// </summary>
    /// <param name="paused">The paused stat</param>
    public static void Pause(bool paused)
    {
        Debug.Log("Paused State: " + paused);
        Wave.paused = paused;
        BezRunner.paused = paused;
    }
    /// <summary>
    /// Pauses new waves from spawning
    /// </summary>
    /// <param name="paused">The pause state</param>
    public static void PauseSpawning(bool paused)
    {
        Debug.Log("Wave Paused State: " + paused);
        Wave.paused = paused;
        //Make sure the runners aren't paused
        BezRunner.paused = false;
    }
    /// <summary>
    /// Pauses the game until everything is ready
    /// </summary>
    private void Awake()
    {   //Pause the game while everything loads
        //Pause(true);
        s_instance = this;
#if !UNITY_EDITOR
        _waveManager.LoadLevel("MainMenu");
#endif  
    }
    /// <summary>
    /// Prepares the GameManager
    /// </summary>
    private void Start()
    {   //Set up listeners
        PlayerShip.s_instance.OnDeath.AddListener(GameOver);
        _waveManager.OnLevelChange.AddListener(StartGame);
        //If the wave manager is null, try and find it
        if (!_waveManager)
        {   //Attempt to get it
            _waveManager = GetComponent<WaveManager>();
            //If its still null, log an error
            if (!_waveManager)
                Debug.LogError("Could not find WaveManager as GameManager");
        }
    }
    /// <summary>
    /// Core game loop
    /// </summary>
    private void Update()
    {   //Wait for the game to start
        if (_gameIsOver)
            return;
        //If there are waves that have not completed, check if they completed
        if (_waves.Count > 0)
            for (int i = 0; i < _waves.Count; i++)
            {
                //If a wave has not spawned, skip
                if (!_waves[i].spawned)
                    break;
                //Remove the wave from being tracked since its finsihed
                _waves.RemoveAt(i);
                //Decrement i so we don't skip a wave
                i--;
            }
        //If all the waves have finished, check if all the enemies are dead.
        else
        {   //Loop over the enemies
            for (int i = 0; i < _enemiesAlive.Count; i++)
                //Check if the enemy is dead
                if (!_enemiesAlive[i])
                {   //Remove the dead enemies from the list
                    _enemiesAlive.RemoveAt(i);
                    //Don't skip an enemy
                    i--;
                }
            //If there are no more alive enemies, you win
            if (_enemiesAlive.Count == 0)
                Win();
        }
    }
    /// <summary>
    /// Called when the game ends
    /// </summary>
    private void GameOver()
    {
        //Load the gameOver scene
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
        //Toggle it to be open
        _gameOverSceneIsOpen = true;
        //The game is over
        _gameIsOver = true;

        OnLose.Invoke();
    }
    /// <summary>
    /// Called when the player wins
    /// </summary>
    public void Win()
    {   //Call the win unityevent
        OnWin.Invoke();
        //The game has ended
        _gameIsOver = true;

        Debug.LogError("Win code not done");
    }
    /// <summary>
    /// Called when the game starts
    /// </summary>
    public void StartGame()
    {   //Reset the player
        PlayerShip.s_instance.Reset();
        //Reset a bunch of values
        _waves.Clear();
        _enemiesAlive.Clear();
        //Enable the waves to spawn
        Pause(false);
        //Cloes the gameOver menu
        CloseGameOverMenu();
    }
    /// <summary>
    /// Unloads the GameOver scene if its open
    /// </summary>
    public void CloseGameOverMenu()
    {
        //Unload the GameOver scene if its open
        if (_gameOverSceneIsOpen)
        {
            SceneManager.UnloadSceneAsync("GameOver");
            _gameIsOver = false;
        }
    }
    /// <summary>
    /// Quits the app
    /// </summary>
    public void ExitApp()
    {   //Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    /// <summary>
    /// Stores a wave
    /// </summary>
    /// <param name="w">The wave</param>
    public void AddWave(Wave w)
    {   //We use the waves adding themself to determine when the next level starts
        _gameIsOver = false;
        _waves.Add(w);
    }
    /// <summary>
    /// Stores an enemy
    /// </summary>
    /// <param name="e">The enemy</param>
    public void AddEnemy(Enemy e)
    {
        _enemiesAlive.Add(e);
    }
}
