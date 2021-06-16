using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Tracks gameover stat and player death
/// </summary>
[RequireComponent(typeof(WaveManager))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the WaveManager for loading scenes
    /// </summary>
    [Tooltip("The wave manager")]
    [SerializeField]
    protected WaveManager _waveManager = null;
    /// <summary>
    /// Is the gameover scene open
    /// </summary>
    private bool _gameOverSceneIsOpen = false;
    /// <summary>
    /// Has the game finished
    /// </summary>
    private static bool _gameIsOver = false;
    /// <summary>
    /// Has the game started
    /// </summary>
    protected static bool _gameHasStarted = false;
    /// <summary>
    /// Returns true if the game has started. This is different from GameIsOver which tracks when the game ends.
    /// </summary>
    public bool GameHasStarted
    {
        get => _gameHasStarted;
        set
        {
            _gameHasStarted = value;
            //Pause or unpause the game depending on the vlaue
            Pause(!value);
        }
    }
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
        if (!_gameHasStarted || _gameIsOver)
            return;
        //Check if the game is over
        _gameIsOver = PlayerShip.PlayerIsDead;
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
    }
    /// <summary>
    /// Called when the game starts
    /// </summary>
    public void StartGame()
    {   //Reset the player
        PlayerShip.s_instance.Reset();
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
}
