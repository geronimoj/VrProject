using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Wave.paused = paused;
        BezRunner.paused = paused;
    }
    /// <summary>
    /// Pauses new waves from spawning
    /// </summary>
    /// <param name="paused">The pause state</param>
    public static void PauseSpawning(bool paused)
    {
        Wave.paused = paused;
        //Make sure the runners aren't paused
        BezRunner.paused = false;
    }
    /// <summary>
    /// Pauses the game until everything is ready
    /// </summary>
    private void Awake()
    {   //Pause the game while everything loads
        Pause(true);
#if !UNITY_EDITOR
        _waveManager.LoadLevel("TutorialLevel");
#endif
    }
    /// <summary>
    /// Prepares the GameManager
    /// </summary>
    private void Start()
    {
        PlayerShip.s_instance.OnDeath.AddListener(GameOver);
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
        Debug.LogError("Game over logic not implemented.");
    }
    /// <summary>
    /// Called when the game starts
    /// </summary>
    public void StartGame()
    {   //Reset the player
        PlayerShip.s_instance.Reset();
        //Enable the waves to spawn
        Pause(false);
        //Reset the score
        PlayerShip.s_instance.m_currentScore = 0;
    }
    /// <summary>
    /// Quits the app
    /// </summary>
    public void ExitApp()
    {   //Quit the application
        Application.Quit();
    }
}
